using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    public partial class MigrationForm : Form
    {
        OptionsManager opts;
        Worker worker;

        CancellationTokenSource ctSource;           // handles task cancellation
        Progress<long> progressHandler;             // used to send callbacks to UI thread
        bool progressBarMaxDivided = false;         /* if overall size to copy > int.MaxValue (maximum possible ProgressBar value), progress bar maximum
                                                       is divided by 1000 and therefore the same applies to every increment to its value */
        bool srcFolderTxtBoxValidated = false,
             destFolderTxtBoxValidated = false;     // true when content of the textbox is verified to be correct
        VersionNumber oldCemuExeVer, newCemuExeVer;

        Stopwatch stopwatch;                        // used to measure how much time the task took to complete
        TextBoxLogger logUpdater;                   // used to update txtBoxLog asynchronously

        public bool DownloadMode { get; }           // if true, newer version of Cemu will be downloaded before migrating

        public MigrationForm(OptionsManager opts, bool downloadMode)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            DownloadMode = downloadMode;
            this.opts = opts;

            stopwatch = new Stopwatch();
            progressHandler = new Progress<long>(UpdateProgressBarsAndLog);

            // Set title according to the mode chosen
            if (DownloadMode)
                lblTitle.Text = "Download & Migrate";
            else
                lblTitle.Text = "Migrate";

            // Remove default (and useless) menu strips
            txtBoxLog.ContextMenuStrip = new ContextMenuStrip();
            txtBoxSrcFolder.ContextMenuStrip = new ContextMenuStrip();
            txtBoxDestFolder.ContextMenuStrip = new ContextMenuStrip();
        }

        private void Back(object sender, EventArgs e)
        {
            ContainerForm.ShowHomeForm();
        }

        private void OpenHelpForm(object sender, EventArgs e)
        {
            new HelpForm(this).Show();
        }

        private void OpenOptionsForm(object sender, EventArgs e)
        {
            new OptionsForm(opts).ShowDialog();
        }

        /*
         *  Folder selection using FolderBrowserDialog for source Cemu folder
         */
        private void SelectSrcCemuFolder(object sender, EventArgs e)
        {
            // Open folder picker in Computer or in the currently selected folder (if it exists)
            var folderPicker = new FolderBrowserDialog();
            folderPicker.RootFolder = Environment.SpecialFolder.MyComputer;
            if (!string.IsNullOrEmpty(txtBoxSrcFolder.Text) && FileUtils.DirectoryExists(txtBoxSrcFolder.Text))
                folderPicker.SelectedPath = txtBoxSrcFolder.Text;

            DialogResult result = folderPicker.ShowDialog();
            // Check whether result is different to the other selected folder (it mustn't be equal)
            if (result == DialogResult.OK)
            {
                if (folderPicker.SelectedPath != txtBoxDestFolder.Text)
                    txtBoxSrcFolder.Text = folderPicker.SelectedPath;
                else
                    MessageBox.Show("Source and destination folder must be different.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /*
         *  Folder selection using FolderBrowserDialog for destination Cemu folder
         */
        private void SelectDestCemuFolder(object sender, EventArgs e)
        {
            // Open folder picker in Computer or in the currently selected folder (if it exists)
            var folderPicker = new FolderBrowserDialog();
            folderPicker.RootFolder = Environment.SpecialFolder.MyComputer;
            if (!string.IsNullOrEmpty(txtBoxDestFolder.Text) && FileUtils.DirectoryExists(txtBoxDestFolder.Text))
                folderPicker.SelectedPath = txtBoxDestFolder.Text;

            DialogResult result = folderPicker.ShowDialog();
            // Check whether result is different to the other selected folder (it mustn't be equal)
            if (result == DialogResult.OK)
            {
                if (folderPicker.SelectedPath != txtBoxSrcFolder.Text)
                    txtBoxDestFolder.Text = folderPicker.SelectedPath;
                else
                    MessageBox.Show("Source and destination folder must be different.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CheckSrcFolderTextboxContent(object sender, EventArgs e)
        {
            // Check if it's a valid Cemu installation
            if (!FileUtils.IsValidCemuInstallation(txtBoxSrcFolder.Text, out string reason))
            {
                errProviderFolders.SetError(txtBoxSrcFolder, reason);
                srcFolderTxtBoxValidated = false;
                btnStart.Enabled = false;

                lblOldCemuVersion.Visible = false;
                lblOldVersionNr.Text = "";
            }
            // Display Cemu version label and verify if all user inputs are OK
            else
            {
                errProviderFolders.SetError(txtBoxSrcFolder, "");
                srcFolderTxtBoxValidated = true;

                oldCemuExeVer = new VersionNumber(FileVersionInfo.GetVersionInfo(Path.Combine(txtBoxSrcFolder.Text, "Cemu.exe")), 3);
                lblOldCemuVersion.Visible = true;
                lblOldVersionNr.Text = oldCemuExeVer.ToString();

                if ((srcFolderTxtBoxValidated && destFolderTxtBoxValidated) && (txtBoxSrcFolder.Text != txtBoxDestFolder.Text))
                    btnStart.Enabled = true;
                else
                    btnStart.Enabled = false;
            }
        }

        private void CheckDestFolderTextboxContent(object sender, EventArgs e)
        {
            bool contentOk;
            string reason;

            // If we're in DownloadMode, check if the directory exists
            // Otherwise, check if the folder is also a valid Cemu installation
            if (DownloadMode)
            {
                contentOk = txtBoxDestFolder.Text != "" && FileUtils.DirectoryExists(txtBoxDestFolder.Text);
                reason = "Directory does not exist";
            }
            else
                contentOk = FileUtils.IsValidCemuInstallation(txtBoxDestFolder.Text, out reason);

            if (!contentOk)
            {
                errProviderFolders.SetError(txtBoxDestFolder, reason);
                destFolderTxtBoxValidated = false;
                btnStart.Enabled = false;

                lblNewCemuVersion.Visible = false;
                lblNewVersionNr.Text = "";
            }
            // Display Cemu version label (only if the form is not in download mode) and verify if all user inputs are OK
            else
            {
                errProviderFolders.SetError(txtBoxDestFolder, "");
                destFolderTxtBoxValidated = true;

                if (!DownloadMode)
                {
                    newCemuExeVer = new VersionNumber(FileVersionInfo.GetVersionInfo(Path.Combine(txtBoxDestFolder.Text, "Cemu.exe")), 3);
                    lblNewCemuVersion.Visible = true;
                    lblNewVersionNr.Text = newCemuExeVer.ToString();
                }

                if ((srcFolderTxtBoxValidated && destFolderTxtBoxValidated) && (txtBoxSrcFolder.Text != txtBoxDestFolder.Text))
                    btnStart.Enabled = true;
                else
                    btnStart.Enabled = false;
            }
        }

        private async void DoOperationsAsync(object sender, EventArgs e)
        {
            // If not in download mode, warn the user if old Cemu version is not older than new one
            if (!DownloadMode)
            {
                if (oldCemuExeVer > newCemuExeVer)
                {
                    DialogResult choice = MessageBox.Show("You're trying to migrate from a newer Cemu version to an older one. " +
                        "This may cause severe incompatibility issues. Do you want to continue?", "Unsafe operation requested",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (choice == DialogResult.No)
                        return;
                }
            }
            // If in download mode, warn the user if the destination folder contains a Cemu installation
            else
            {
                if (FileUtils.FileExists(Path.Combine(txtBoxDestFolder.Text, "Cemu.exe")))
                {
                    DialogResult choice = MessageBox.Show("The chosen destination folder already contains a Cemu installation. " +
                        "Do you want to overwrite it?", "Cemu installation already present",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (choice == DialogResult.No)
                        return;
                }
            }

            // Get the list of folders and files to copy telling the method if source Cemu version is >= 1.10..
            List<string> foldersToCopy = opts.GetFoldersToCopy(oldCemuExeVer.Major > 1 || oldCemuExeVer.Minor >= 10);
            List<string> filesToCopy = opts.GetFilesToCopy();

            // ... and check if both lists are empty (no folders and files to copy)
            if (foldersToCopy.Count == 0 && filesToCopy.Count == 0)
            {
                MessageBox.Show("It seems that there are neither folders nor single files to copy. Probably you set up options incorrectly.",
                                "Empty folders and files lists", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Start the textbox logger
            logUpdater = new TextBoxLogger(txtBoxLog);

            // Start preparing
            txtBoxLog.Clear();
            ChangeProgressLabelText("Preparing");
            btnStart.Enabled = false;
            btnBack.Enabled = false;
            WorkOutcome result = WorkOutcome.Success;

            // Create a new Worker instance and pass it all needed data
            ctSource = new CancellationTokenSource();
            worker = new Worker(txtBoxSrcFolder.Text, txtBoxDestFolder.Text, foldersToCopy, filesToCopy, ctSource.Token, logUpdater.AppendLogMessage);

            // Starting from now, we can safely cancel operations without having problems
            btnCancel.Enabled = true;

            stopwatch.Start();
            try
            {
                // Perform download operations if we are in download mode
                if (DownloadMode)
                {
                    var downloadTask = Task.Run(() => worker.PerformDownloadOperations(opts.Download, ChangeProgressLabelText,
                                                      (o, evtArgs) => {
                                                          // Set maximum progress bar value according to file size
                                                          if (overallProgressBar.Maximum != evtArgs.TotalBytesToReceive)
                                                              overallProgressBar.Maximum = (int)evtArgs.TotalBytesToReceive;
                                                       
                                                          // Update percent label and progress bar
                                                          lblPercent.Text = evtArgs.ProgressPercentage + "%";
                                                          overallProgressBar.Value = (int)evtArgs.BytesReceived;
                                                       
                                                          // Refresh log textbox
                                                          logUpdater.UpdateTextBox();
                                                      })
                                                );
                    newCemuExeVer = await downloadTask;

                    // Update settings file with the new value of lastKnownCemuVersion (if it's changed)
                    VersionNumber.TryParse(opts.Download[OptionsKeys.LastKnownCemuVersion], out VersionNumber previousLastKnownCemuVersion);
                    if (previousLastKnownCemuVersion != newCemuExeVer)
                    {
                        opts.Download[OptionsKeys.LastKnownCemuVersion] = newCemuExeVer.ToString();
                        try
                        {
                            opts.WriteOptionsToFile();
                        }
                        catch (Exception optionsUpdateExc)
                        {
                            logUpdater.AppendLogMessage($"WARNING: Unable to update settings file with the latest known Cemu version: {optionsUpdateExc.Message}");
                        }
                    }
                }

                // Set maximum progress bar value according to overall size to copy
                long overallSize = worker.GetOverallSizeToCopy();
                if (overallSize > int.MaxValue)
                {
                    overallSize /= 1000;
                    progressBarMaxDivided = true;
                }
                overallProgressBar.Value = 0;       // reset progress bar
                overallProgressBar.Maximum = (int)overallSize;

                // Start migration operations in a secondary thread
                var migrationTask = Task.Run(() => worker.PerformMigrationOperations(opts.Migration, ChangeProgressLabelText, progressHandler));
                await migrationTask;

                stopwatch.Stop();

                // If there have been errors during operations, update result
                if (worker.ErrorsEncountered > 0)
                    result = WorkOutcome.CompletedWithErrors;
            }
            catch (Exception taskExc)   // task cancelled or aborted due to an error
            {
                stopwatch.Stop();
                try
                {
                    if (worker.CreatedFiles.Count > 0 || worker.CreatedDirectories.Count > 0)
                    {
                        // Ask if the user wants to remove files that have been created
                        DialogResult choice = MessageBox.Show("Do you want to delete files that have already been created?", "Operation stopped", MessageBoxButtons.YesNo);
                        if (choice == DialogResult.Yes)
                            worker.DeleteCreatedFiles();
                    }
                }
                catch (Exception cleanupExc)
                {
                    MessageBox.Show($"An error occurred when deleting created files: {cleanupExc.Message}", "Unexpected error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Update result according to caught exception type
                if (taskExc is OperationCanceledException)
                    result = WorkOutcome.CancelledByUser;
                else
                {
                    logUpdater.AppendLogMessage($"\r\nOperation aborted due to unrecoverable error: {taskExc.Message}", false);
                    result = WorkOutcome.Aborted;
                }
            }

            // Ask if user wants to create Cemu desktop shortcut
            if (result != WorkOutcome.Aborted && result != WorkOutcome.CancelledByUser && opts.Migration[OptionsKeys.AskForDesktopShortcut])
            {
                DialogResult choice = MessageBox.Show("Do you want to create a desktop shortcut?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                bool isNewCemuVersionAtLeast110 = newCemuExeVer.Major > 1 || newCemuExeVer.Minor >= 10;
                if (choice == DialogResult.Yes)     // mlc01 folder external path is passed only if needed
                    worker.CreateDesktopShortcut(newCemuExeVer.ToString(),
                      (isNewCemuVersionAtLeast110 && opts.Migration[OptionsKeys.UseCustomMlcFolderIfSupported] == true) ? opts.CustomMlcFolderPath : null);
            }

            // Show a MessageBox with the final result of the task
            switch (result)
            {
                case WorkOutcome.Success:
                    MessageBox.Show("Operation successfully terminated.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    logUpdater.AppendLogMessage($"\r\nOperations terminated without errors after {(float)stopwatch.ElapsedMilliseconds / 1000} seconds.", false);
                    break;
                case WorkOutcome.Aborted:
                    MessageBox.Show("Operation aborted due to an unexpected error.", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    break;
                case WorkOutcome.CancelledByUser:
                    MessageBox.Show("Operation cancelled by user.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    logUpdater.AppendLogMessage($"\r\nOperations cancelled due to user request.", false);
                    break;
                case WorkOutcome.CompletedWithErrors:
                    MessageBox.Show("Operation terminated with errors.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    logUpdater.AppendLogMessage($"\r\nOperations terminated with {worker.ErrorsEncountered} errors after {(float)stopwatch.ElapsedMilliseconds / 1000} seconds.", false);
                    break;
            }

            // Reset form controls to their original state
            ResetEverything();
        }

        /*
         *  Resets the GUI and all Worker-related variables in order for the form to be ready for another task
         */
        private void ResetEverything()
        {
            // Reset progress bars
            overallProgressBar.Value = 0;
            lblPercent.Text = "0%";
            lblCurrentTask.Text = "Waiting for operations to start...";

            // Reset Cemu version label
            lblOldCemuVersion.Visible = false;
            lblNewCemuVersion.Visible = false;
            lblOldVersionNr.Text = "";
            lblNewVersionNr.Text = "";

            // Reset textboxes (I need to detach & reattach event handlers otherwise errorProviders will be triggered) and buttons
            txtBoxSrcFolder.TextChanged -= CheckSrcFolderTextboxContent;
            txtBoxSrcFolder.Text = "";
            txtBoxSrcFolder.TextChanged += CheckSrcFolderTextboxContent;
            txtBoxDestFolder.TextChanged -= CheckDestFolderTextboxContent;
            txtBoxDestFolder.Text = "";
            txtBoxDestFolder.TextChanged += CheckDestFolderTextboxContent;
            btnCancel.Enabled = false;
            btnBack.Enabled = true;

            // Reset textboxes' validated state
            srcFolderTxtBoxValidated = false;
            destFolderTxtBoxValidated = false;

            // Reset stopwatch
            stopwatch.Reset();

            // Tell the textbox logger to stop after printing all queued messages
            logUpdater.StopAndWaitShutdown();
        }

        private void CancelOperations(object sender = null, EventArgs e = null)
        {
            lblCurrentTask.Text = "Cancelling...";
            ctSource.Cancel();
        }
        
        /*
         *  Callback method that updates progress bars after a file has been copied
         *  Since it's the callback used by Progress<T>, it's queued on the main UI thread
         */
        private void UpdateProgressBarsAndLog(long dim)
        {
            // Update progress bar and percent label
            if (progressBarMaxDivided)
                overallProgressBar.Value += Convert.ToInt32(dim/1000);
            else
                overallProgressBar.Value += Convert.ToInt32(dim);

            lblPercent.Text = Math.Floor(overallProgressBar.Value / (double)overallProgressBar.Maximum * 100) + "%";

            logUpdater.UpdateTextBox();
        }

        /*
         *  Callback method that updates current task label according to the next task
         *  It also updates log textbox in order to prevent messages not being written if progress bar isn't updated during a task
         */
        private void ChangeProgressLabelText(string newLabelText)
        {
            lblCurrentTask.Text = $"{newLabelText}...";
            logUpdater.AppendLogMessage($"-- {newLabelText} --");
            logUpdater.UpdateTextBox();
        }

        /*
         *  Methods for handling drag & drop into folder textboxes
         */
        private void TextboxDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text) || e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void TextboxDragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                (sender as TextBox).Text = (e.Data.GetData(DataFormats.FileDrop) as string[])[0];
            else if (e.Data.GetDataPresent(DataFormats.Text))
                (sender as TextBox).Text = e.Data.GetData(DataFormats.Text).ToString();
        }

        /*
         *  Shows/hides log textbox when clicking on Details label
         */
        private void ShowHideDetailsTextbox(object sender, EventArgs e)
        {
            if (txtBoxLog.Visible)  // arrow down -> arrow right
                lblDetails.Text = lblDetails.Text.Replace((char)9661, (char)9655);
            else                    // arrow right -> arrow down
                lblDetails.Text = lblDetails.Text.Replace((char)9655, (char)9661);

            txtBoxLog.Visible = !txtBoxLog.Visible;
        }

        /*
         *  Resizes the form when txtBoxLog's visible state changes
         */
        private void ResizeFormOnLogTextboxVisibleChanged(object sender, EventArgs e)
        {
            if (ContainerForm.IsCurrentDisplayingForm(this))     // avoid triggering the event before the form is shown
            {
                if (txtBoxLog.Visible)
                    this.Height += txtBoxLog.Height;
                else
                    this.Height -= txtBoxLog.Height;
            }
        }

        /*
         *  If there's a job running, cancel window closing and ask the user if he wants to cancel operations
         */
        private void PreventClosingIfOperationInProgress(object sender, FormClosingEventArgs e)
        {
            if (logUpdater != null && logUpdater.IsReady)
            {
                e.Cancel = true;
                DialogResult choice = MessageBox.Show("You can't close this window while an operation is in progress. Do you want to cancel the job?",
                                                      "Exit not allowed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (choice == DialogResult.Yes)
                    CancelOperations();
            }
        }
    }
}
