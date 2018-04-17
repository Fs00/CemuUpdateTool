using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    public partial class MigrationForm : Form
    {
        Worker worker;
        OptionsManager opts;
        Progress<long> progressHandler;
        CancellationTokenSource ctSource;
        bool progressBarMaxDivided = false;
        bool srcFolderTxtBoxValidated = false,
             destFolderTxtBoxValidated = false;
        VersionNumber oldCemuExeVer, newCemuExeVer;
        Stopwatch stopwatch;
        StringBuilder logBuffer;
        Dispatcher logUpdater;

        public bool DownloadMode { get; }   // value that determinates if newer version of Cemu must be downloaded before migrating

        public MigrationForm(bool downloadMode)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            DownloadMode = downloadMode;
            opts = new OptionsManager();
            stopwatch = new Stopwatch();
            logBuffer = new StringBuilder(1000);
            progressHandler = new Progress<long>(UpdateProgressBarsAndLog);
        }

        private void Exit(object sender, EventArgs e)
        {
            Close();
        }

        private void OpenHelpForm(object sender, EventArgs e)
        {
            new HelpForm().Show();
        }

        private void OpenAboutForm(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog();
        }

        private void OpenOptionsForm(object sender, EventArgs e)
        {
            new OptionsForm(opts).ShowDialog();
        }

        /*
         *  Folder selection using FolderBrowserDialog for old Cemu folder
         */
        private void SelectOldCemuFolder(object sender, EventArgs e)
        {
            // Open folder picker in %UserProfile% folder and save the selected path
            var folderPicker = new FolderBrowserDialog();
            folderPicker.RootFolder = Environment.SpecialFolder.UserProfile;
            DialogResult result = folderPicker.ShowDialog();

            // Check whether result is OK
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderPicker.SelectedPath))
            {
                if (folderPicker.SelectedPath != txtBoxNewFolder.Text)
                    txtBoxOldFolder.Text = folderPicker.SelectedPath;
                else
                    MessageBox.Show("Source and destination folder must be different.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /*
         *  Folder selection using FolderBrowserDialog for new Cemu folder
         */
        private void SelectNewCemuFolder(object sender, EventArgs e)
        {
            // Open folder picker in %UserProfile% folder and save the selected path
            var folderPicker = new FolderBrowserDialog();
            folderPicker.RootFolder = Environment.SpecialFolder.UserProfile;
            DialogResult result = folderPicker.ShowDialog();

            // Check whether result is OK
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderPicker.SelectedPath))
            {
                if (folderPicker.SelectedPath != txtBoxOldFolder.Text)
                    txtBoxNewFolder.Text = folderPicker.SelectedPath;
                else
                    MessageBox.Show("Source and destination folder must be different.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CheckOldFolderTextboxContent(object sender, EventArgs e)
        {
            // Check if input directory exists
            if (txtBoxOldFolder.Text == "" || !FileUtils.DirectoryExists(txtBoxOldFolder.Text))
            {
                errProviderOldFolder.SetError(txtBoxOldFolder, "Directory does not exist");
                srcFolderTxtBoxValidated = false;
                btnStart.Enabled = false;

                lblOldCemuVersion.Visible = false;
                lblOldVersionNr.Text = "";
            }
            // Check if it's a valid Cemu installation ONLY IF the form is not in download mode
            else if (!DownloadMode && !FileUtils.FileExists(Path.Combine(txtBoxOldFolder.Text, "Cemu.exe")))
            {
                errProviderOldFolder.SetError(txtBoxOldFolder, "Not a valid Cemu installation (Cemu.exe is missing)");
                srcFolderTxtBoxValidated = false;
                btnStart.Enabled = false;

                lblOldCemuVersion.Visible = false;
                lblOldVersionNr.Text = "";
            }
            // Display Cemu version label and verify if all user inputs are OK
            else
            {
                errProviderOldFolder.SetError(txtBoxOldFolder, "");
                srcFolderTxtBoxValidated = true;

                oldCemuExeVer = new VersionNumber(FileVersionInfo.GetVersionInfo(Path.Combine(txtBoxOldFolder.Text, "Cemu.exe")), 3);
                lblOldCemuVersion.Visible = true;
                lblOldVersionNr.Text = oldCemuExeVer.ToString();

                if ((srcFolderTxtBoxValidated && destFolderTxtBoxValidated) && (txtBoxOldFolder.Text != txtBoxNewFolder.Text))
                    btnStart.Enabled = true;
                else
                    btnStart.Enabled = false;
            }
        }

        private void CheckNewFolderTextboxContent(object sender, EventArgs e)
        {
            // Check if input directory exists
            if (txtBoxNewFolder.Text == "" || !FileUtils.DirectoryExists(txtBoxNewFolder.Text))
            {
                errProviderNewFolder.SetError(txtBoxNewFolder, "Directory does not exist");
                destFolderTxtBoxValidated = false;
                btnStart.Enabled = false;

                lblNewCemuVersion.Visible = false;
                lblNewVersionNr.Text = "";
            }
            // Check if it's a valid Cemu installation ONLY IF the form is not in download mode
            else if (!DownloadMode && !FileUtils.FileExists(Path.Combine(txtBoxNewFolder.Text, "Cemu.exe")))
            {
                errProviderNewFolder.SetError(txtBoxNewFolder, "Not a valid Cemu installation (Cemu.exe is missing)");
                destFolderTxtBoxValidated = false;
                btnStart.Enabled = false;

                lblNewCemuVersion.Visible = false;
                lblNewVersionNr.Text = "";
            }
            // Display Cemu version label (only if the form is not in download mode) and verify if all user inputs are OK
            else
            {
                errProviderNewFolder.SetError(txtBoxNewFolder, "");
                destFolderTxtBoxValidated = true;

                if (!DownloadMode)
                {
                    newCemuExeVer = new VersionNumber(FileVersionInfo.GetVersionInfo(Path.Combine(txtBoxNewFolder.Text, "Cemu.exe")), 3);
                    lblNewCemuVersion.Visible = true;
                    lblNewVersionNr.Text = newCemuExeVer.ToString();
                }

                if ((srcFolderTxtBoxValidated && destFolderTxtBoxValidated) && (txtBoxOldFolder.Text != txtBoxNewFolder.Text))
                    btnStart.Enabled = true;
                else
                    btnStart.Enabled = false;
            }
        }

        private async void DoOperationsAsync(object sender, EventArgs e)
        {           
            // Check if Cemu versions are ok, if not warn the user
            if (oldCemuExeVer > newCemuExeVer)
            {
                DialogResult choice = MessageBox.Show("You're trying to migrate from a newer Cemu version to an older one. " +
                    "This may cause severe incompatibility issues. Do you want to continue?", "Unsafe operation requested", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (choice == DialogResult.No)
                    return;
            }

            // Get the list of folders to copy telling the method if source Cemu version is >= 1.10..
            List<string> foldersToCopy = opts.GetFoldersToCopy(oldCemuExeVer.Major > 1 || oldCemuExeVer.Minor >= 10);

            // ... and check if it's is empty (no folders to copy)
            if (foldersToCopy.Count == 0)
            {
                MessageBox.Show("It seems that there are no folders to copy. Probably you set up options incorrectly.",
                    "Empty folders list", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Start preparing
            txtBoxLog.Clear();
            lblCurrentTask.Text = "Preparing...";
            AppendLogMessage("Preparing...");
            btnStart.Enabled = false;
            WorkOutcome result = WorkOutcome.Success;

            // Start the dispatcher used to update log textbox
            StartLogTextboxDispatcher();
            Debug.Assert(logUpdater != null, "Failed to start dispatcher!");

            // Create a new Worker instance and pass it all needed data
            ctSource = new CancellationTokenSource();
            worker = new Worker(txtBoxOldFolder.Text, txtBoxNewFolder.Text, foldersToCopy, ctSource.Token, AppendLogMessage);

            stopwatch.Start();

            // Set overall progress bar according to overall size
            long overallSize = worker.GetOverallSizeToCopy();      // TODO: se si è in DownloadMode, va sommata la dimensione del file da scaricare
            if (overallSize > Int32.MaxValue)
            {
                overallSize /= 1000;
                progressBarMaxDivided = true;
            }
            overallProgressBar.Maximum = Convert.ToInt32(overallSize);

            try
            {
                // Start operations in a secondary thread and enable cancel button once operations have started
                var operationsTask = Task.Run(() => worker.PerformMigrationOperations(opts.migrationOptions, ChangeProgressLabelText,
                    progressHandler));
                btnCancel.Enabled = true;

                // Yield control to the form
                await operationsTask;
                stopwatch.Stop();

                // If there have been errors during operations, update result
                if (worker.ErrorsEncountered)
                    result = WorkOutcome.CompletedWithErrors;
            }
            catch (Exception taskExc)   // task cancelled or aborted due to an error
            {
                stopwatch.Stop();
                try
                {
                    // Ask if the user wants to remove files that have been created
                    if (worker.CreatedFiles.Count > 0 || worker.CreatedDirectories.Count > 0)
                    {
                        DialogResult choice = MessageBox.Show("Do you want to delete files that have already been created?", "Operation stopped", MessageBoxButtons.YesNo);
                        if (choice == DialogResult.Yes)
                            worker.PerformCleanup();
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
                    AppendLogMessage($"\r\nOperation aborted due to unrecoverable error: {taskExc.Message}", false);
                    result = WorkOutcome.Aborted;
                }
            }

            // Once work has completed, ask if user wants to create Cemu desktop shortcut...
            if (result != WorkOutcome.Aborted && result != WorkOutcome.CancelledByUser && opts.migrationOptions["askForDesktopShortcut"] == true)
            {
                DialogResult choice = MessageBox.Show("Do you want to create a desktop shortcut?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                bool isNewCemuVersionAtLeast110 = newCemuExeVer.Major > 1 || newCemuExeVer.Minor >= 10;
                if (choice == DialogResult.Yes)     // mlc01 folder external path is passed only if needed
                    worker.CreateDesktopShortcut(newCemuExeVer.ToString(),
                      (isNewCemuVersionAtLeast110 && opts.migrationOptions["dontCopyMlcFolderFor1.10+"] == true) ? opts.mlcFolderExternalPath : null);
            }

            AppendLogMessage($"\r\nOperations terminated after {(float) stopwatch.ElapsedMilliseconds / 1000} seconds.");

            // ... and reset form controls to their original state
            ResetEverything(result);
        }

        /*
         *  Resets the GUI and all Worker-related variables in order for the form to be ready for another task
         */
        private async void ResetEverything(WorkOutcome outcome)
        {
            // Show a MessageBox with the final result of the task
            switch (outcome)
            {
                case WorkOutcome.Success:
                    MessageBox.Show("Operation successfully terminated.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case WorkOutcome.Aborted:
                    MessageBox.Show("Operation aborted due to an unexpected error.", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    break;
                case WorkOutcome.CancelledByUser:
                    MessageBox.Show("Operation cancelled by user.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case WorkOutcome.CompletedWithErrors:
                    MessageBox.Show("Operation terminated with errors.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }

            // Tell the log textbox dispatcher to stop after finishing all work queued
            var dispatcherShutdown = logUpdater.InvokeAsync(() => Dispatcher.CurrentDispatcher.InvokeShutdown());

            // Reset progress bars
            overallProgressBar.Value = 0;
            lblPercent.Text = "0%";
            lblCurrentTask.Text = "Waiting for operations to start...";

            // Reset Cemu version label
            lblOldCemuVersion.Visible = false;
            lblNewCemuVersion.Visible = false;
            lblOldVersionNr.Text = "";
            lblNewVersionNr.Text = "";

            // Reset textboxes (I need to detach & reattach event handlers otherwise errorProviders will be triggered) and Cancel button
            txtBoxOldFolder.TextChanged -= CheckOldFolderTextboxContent;
            txtBoxOldFolder.Text = "";
            txtBoxOldFolder.TextChanged += CheckOldFolderTextboxContent;
            txtBoxNewFolder.TextChanged -= CheckNewFolderTextboxContent;
            txtBoxNewFolder.Text = "";
            txtBoxNewFolder.TextChanged += CheckNewFolderTextboxContent;
            btnCancel.Enabled = false;

            // Reset textboxes' validated state
            srcFolderTxtBoxValidated = false;
            destFolderTxtBoxValidated = false;

            // Reset stopwatch
            stopwatch.Reset();

            // Once the log dispatcher has been shut down, print all buffer content before it gets deleted
            await dispatcherShutdown;
            if (logBuffer.Length > 0)
                txtBoxLog.AppendText(logBuffer.ToString());
            logBuffer.Clear();
        }

        private void CancelOperations(object sender, EventArgs e)
        {
            lblCurrentTask.Text = "Cancelling...";
            ctSource.Cancel();
            AppendLogMessage("\r\nUser requested cancellation.");
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

            UpdateLogTextbox();
        }

        /*
         *  Callback method that prints log buffer content asynchronously and flushes it
         *  Lock avoids race conditions with AppendLogMessage.
         */
        private void UpdateLogTextbox()
        {
            if (txtBoxLog.Visible)
            {
                lock (logBuffer)
                {
                    string log = logBuffer.ToString();
                    logUpdater.InvokeAsync(() => txtBoxLog.AppendText(log));
                    logBuffer.Clear();
                }
            }
        }

        /*
         *  Callback method that updates current task label according to the next task
         *  It also updates log textbox in order to prevent messages not being written if progress bar isn't updated during a task
         */
        private void ChangeProgressLabelText(string newLabelText)
        {
            lblCurrentTask.Text = $"{newLabelText}...";
            AppendLogMessage($"{newLabelText}...");
            UpdateLogTextbox();
        }

        /*
         *  Callback method that appends a message to be written in Details textbox
         */
        private void AppendLogMessage(string message, bool newLine = true)
        {
            // Lock avoids race conditions with UpdateProgressBarsAndLog
            lock (logBuffer)
            {
                logBuffer.Append(message);
                if (newLine)
                    logBuffer.Append("\r\n");
            }
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
            if (ActiveForm != null)     // avoid triggering the event before the form is active
            {
                if (txtBoxLog.Visible)
                    this.Height += txtBoxLog.Height;
                else
                    this.Height -= txtBoxLog.Height;
            }
        }

        private void StartLogTextboxDispatcher()
        {
            // Create and start the thread on which the dispatcher will run
            Thread uiDispatcherThread = new Thread(() => Dispatcher.Run());
            uiDispatcherThread.IsBackground = true;
            uiDispatcherThread.Name = "LogTextboxUpdater";
            uiDispatcherThread.Start();

            // Wait until dispatcher is running
            int maxWaitingCycles = 100;
            int cycleIndex = 0;
            do
            {
                Thread.Sleep(10);
                logUpdater = Dispatcher.FromThread(uiDispatcherThread);
                Debug.WriteLine((logUpdater == null) ? "Couldn't get dispatcher. Retrying..." : "Dispatcher obtained.");
                cycleIndex++;
            }
            while (logUpdater == null && cycleIndex < maxWaitingCycles);

            if (logUpdater == null)
                uiDispatcherThread.Abort();
        }

        private void ShutdownDispatcherOnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!(logUpdater == null || logUpdater.HasShutdownStarted))
                logUpdater.Invoke(() => Dispatcher.CurrentDispatcher.InvokeShutdown());
        }
    }
}
