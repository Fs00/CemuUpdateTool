using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    public partial class MigrationForm : OperationsForm
    {
        Progress<long> progressHandler;             // used to send callbacks to UI thread
        bool progressBarMaxDivided = false;         /* if overall size to copy > int.MaxValue (maximum possible ProgressBar value), progress bar maximum
                                                       is divided by 1000 and therefore the same applies to every increment to its value */
        bool srcFolderTxtBoxValidated = false,
             destFolderTxtBoxValidated = false;     // true when content of the textbox is verified to be correct
        VersionNumber oldCemuExeVer, newCemuExeVer;

        public bool DownloadMode { get; }           // if true, newer version of Cemu will be downloaded before migrating

        public MigrationForm(OptionsManager opts, bool downloadMode) : base(opts)
        {
            InitializeComponent();
            DownloadMode = downloadMode;
            progressHandler = new Progress<long>(UpdateProgressBarsAndLog);

            // Set title according to the mode chosen
            if (DownloadMode)
                lblTitle.Text = "Download & Migrate";
            else
                lblTitle.Text = "Migrate";

            // Remove default (and useless) menu strips
            txtBoxSrcFolder.ContextMenuStrip = new ContextMenuStrip();
            txtBoxDestFolder.ContextMenuStrip = new ContextMenuStrip();
        }

        /*
         *  Folder selection using FolderBrowserDialog for source Cemu folder
         */
        private void SelectSrcCemuFolder(object sender, EventArgs e)
        {
            // Open folder picker
            string chosenFolder = ChooseFolder(txtBoxSrcFolder.Text);

            // Check whether result is different to the other selected folder (it mustn't be equal)
            if (chosenFolder != null)
            {
                if (chosenFolder != txtBoxDestFolder.Text)
                    txtBoxSrcFolder.Text = chosenFolder;
                else
                    MessageBox.Show("Source and destination folder must be different.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /*
         *  Folder selection using FolderBrowserDialog for destination Cemu folder
         */
        private void SelectDestCemuFolder(object sender, EventArgs e)
        {
            // Open folder picker
            string chosenFolder = ChooseFolder(txtBoxDestFolder.Text);

            // Check whether result is different to the other selected folder (it mustn't be equal)
            if (chosenFolder != null)
            {
                if (chosenFolder != txtBoxSrcFolder.Text)
                    txtBoxDestFolder.Text = chosenFolder;
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

                lblSrcCemuVersion.Visible = false;
                lblSrcVersionNr.Text = "";
            }
            // Display Cemu version label and verify if all user inputs are OK
            else
            {
                errProviderFolders.SetError(txtBoxSrcFolder, "");
                srcFolderTxtBoxValidated = true;

                oldCemuExeVer = new VersionNumber(FileVersionInfo.GetVersionInfo(Path.Combine(txtBoxSrcFolder.Text, "Cemu.exe")), 3);
                lblSrcCemuVersion.Visible = true;
                lblSrcVersionNr.Text = oldCemuExeVer.ToString();

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

                lblDestCemuVersion.Visible = false;
                lblDestVersionNr.Text = "";
            }
            // Display Cemu version label (only if the form is not in download mode) and verify if all user inputs are OK
            else
            {
                errProviderFolders.SetError(txtBoxDestFolder, "");
                destFolderTxtBoxValidated = true;

                if (!DownloadMode)
                {
                    newCemuExeVer = new VersionNumber(FileVersionInfo.GetVersionInfo(Path.Combine(txtBoxDestFolder.Text, "Cemu.exe")), 3);
                    lblDestCemuVersion.Visible = true;
                    lblDestVersionNr.Text = newCemuExeVer.ToString();
                }

                if ((srcFolderTxtBoxValidated && destFolderTxtBoxValidated) && (txtBoxSrcFolder.Text != txtBoxDestFolder.Text))
                    btnStart.Enabled = true;
                else
                    btnStart.Enabled = false;
            }
        }

        protected override async void DoOperationsAsync(object sender, EventArgs e)
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
                    newCemuExeVer = await Task.Run(() => worker.PerformDownloadOperations(opts.Download, ChangeProgressLabelText, HandleDownloadProgress));

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

            ShowWorkResultDialog(result);
            // Reset form controls to their original state
            ResetEverything();
        }

        /*
         *  Resets the GUI and all Worker-related variables in order for the form to be ready for another task
         */
        protected override void ResetEverything()
        {
            base.ResetEverything();

            // Reset Cemu version label
            lblSrcCemuVersion.Visible = false;
            lblDestCemuVersion.Visible = false;
            lblSrcVersionNr.Text = "";
            lblDestVersionNr.Text = "";

            // Reset textboxes (I need to detach & reattach event handlers otherwise errorProviders will be triggered) and buttons
            txtBoxSrcFolder.TextChanged -= CheckSrcFolderTextboxContent;
            txtBoxSrcFolder.Text = "";
            txtBoxSrcFolder.TextChanged += CheckSrcFolderTextboxContent;
            txtBoxDestFolder.TextChanged -= CheckDestFolderTextboxContent;
            txtBoxDestFolder.Text = "";
            txtBoxDestFolder.TextChanged += CheckDestFolderTextboxContent;

            // Reset textboxes' validated state
            srcFolderTxtBoxValidated = false;
            destFolderTxtBoxValidated = false;
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

        private void OpenOptionsForm(object sender, EventArgs e)
        {
            new OptionsForm(opts).ShowDialog();
        }

        // These "fake overrides" are needed to avoid VS designer errors
        protected override void ResizeFormOnLogTextboxVisibleChanged(object sender, EventArgs e)  { base.ResizeFormOnLogTextboxVisibleChanged(sender, e); }
        protected override void TextboxDragDrop(object sender, DragEventArgs e)  { base.TextboxDragDrop(sender, e); }
        protected override void TextboxDragEnter(object sender, DragEventArgs e)  { base.TextboxDragEnter(sender, e); }
    }
}
