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
        VersionNumber srcCemuExeVersion, destCemuExeVersion;

        public bool DownloadMode { get; }           // if true, newer version of Cemu will be downloaded before migrating

        public MigrationForm(OptionsManager opts, bool downloadMode) : base(opts)
        {
            InitializeComponent();
            DownloadMode = downloadMode;
            progressHandler = new Progress<long>(UpdateProgressBarsAndLog);

            // Set controls according to the mode chosen
            if (DownloadMode)
            {
                lblTitle.Text = "Download & Migrate";
                lblDestCemuVersion.Visible = true;
                comboBoxVersion.SelectedIndex = 0;      // display "Latest" in combobox

                // Place destination version label at the left of the combobox (otherwise the label would be covered by it)
                var newLocation = lblDestCemuVersion.Location;
                newLocation.X = comboBoxVersion.Location.X - lblDestCemuVersion.Size.Width - 3;
                lblDestCemuVersion.Location = newLocation;
            }
            else
            {
                lblTitle.Text = "Migrate";
                comboBoxVersion.Visible = false;
            }

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

                srcCemuExeVersion = new VersionNumber(FileVersionInfo.GetVersionInfo(Path.Combine(txtBoxSrcFolder.Text, "Cemu.exe")), 3);
                lblSrcCemuVersion.Visible = true;
                lblSrcVersionNr.Text = srcCemuExeVersion.ToString();

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

                if (!DownloadMode)
                {
                    lblDestCemuVersion.Visible = false;
                    lblDestVersionNr.Text = "";
                }
            }
            // Display Cemu version label (only if the form is not in download mode) and verify if all user inputs are OK
            else
            {
                errProviderFolders.SetError(txtBoxDestFolder, "");
                destFolderTxtBoxValidated = true;

                if (!DownloadMode)
                {
                    destCemuExeVersion = new VersionNumber(FileVersionInfo.GetVersionInfo(Path.Combine(txtBoxDestFolder.Text, "Cemu.exe")), 3);
                    lblDestCemuVersion.Visible = true;
                    lblDestVersionNr.Text = destCemuExeVersion.ToString();
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
                if (srcCemuExeVersion > destCemuExeVersion)
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
            List<string> foldersToCopy = opts.GetFoldersToCopy(srcCemuExeVersion.Major > 1 || srcCemuExeVersion.Minor >= 10);
            List<string> filesToCopy = opts.GetFilesToCopy();

            // ... and check if both lists are empty (no folders and files to copy)
            if (foldersToCopy.Count == 0 && filesToCopy.Count == 0)
            {
                MessageBox.Show("It seems that there are neither folders nor single files to copy. Probably you set up options incorrectly.",
                                "Empty folders and files lists", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            WorkOutcome result;
            PrepareControlsForOperations();

            // Create a new Worker instance and pass it all needed data
            ctSource = new CancellationTokenSource();
            worker = new Worker(txtBoxSrcFolder.Text, txtBoxDestFolder.Text, foldersToCopy, filesToCopy, ctSource.Token, logUpdater.AppendLogMessage);

            stopwatch.Start();
            try
            {
                // Perform download operations if we are in download mode
                if (DownloadMode)
                {
                    destCemuExeVersion = await Task.Run(() => worker.PerformDownloadOperations(opts.Download, ChangeProgressLabelText, HandleDownloadProgress, destCemuExeVersion));

                    // Update settings file with the new value of lastKnownCemuVersion (if it's changed)
                    VersionNumber.TryParse(opts.Download[OptionsKeys.LastKnownCemuVersion], out VersionNumber previousLastKnownCemuVersion);
                    if (previousLastKnownCemuVersion != destCemuExeVersion)
                    {
                        opts.Download[OptionsKeys.LastKnownCemuVersion] = destCemuExeVersion.ToString();
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
                {
                    logUpdater.AppendLogMessage($"\r\nOperations terminated with {worker.ErrorsEncountered} errors after {(float)stopwatch.ElapsedMilliseconds / 1000} seconds.", false);
                    result = WorkOutcome.CompletedWithErrors;
                }
                else
                {
                    logUpdater.AppendLogMessage($"\r\nOperations terminated without errors after {(float)stopwatch.ElapsedMilliseconds / 1000} seconds.", false);
                    result = WorkOutcome.Success;
                }
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
                {
                    logUpdater.AppendLogMessage($"\r\nOperations cancelled due to user request.", false);
                    result = WorkOutcome.CancelledByUser;
                }
                else
                {
                    logUpdater.AppendLogMessage($"\r\nOperation aborted due to unrecoverable error: {taskExc.Message}", false);
                    result = WorkOutcome.Aborted;
                }
            }

            ResetState();

            // Ask if user wants to create Cemu desktop shortcut
            if (result != WorkOutcome.Aborted && result != WorkOutcome.CancelledByUser && opts.Migration[OptionsKeys.AskForDesktopShortcut])
            {
                DialogResult choice = MessageBox.Show("Do you want to create a desktop shortcut?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                bool isNewCemuVersionAtLeast110 = destCemuExeVersion.Major > 1 || destCemuExeVersion.Minor >= 10;
                if (choice == DialogResult.Yes)     // mlc01 folder external path is passed only if needed
                    worker.CreateDesktopShortcut(destCemuExeVersion.ToString(),
                      (isNewCemuVersionAtLeast110 && opts.Migration[OptionsKeys.UseCustomMlcFolderIfSupported] == true) ? opts.CustomMlcFolderPath : null);
            }

            ShowWorkResultDialog(result);
        }

        /*
         *  Resets the GUI and all Worker-related variables in order for the form to be ready for another task
         */
        protected override void ResetState()
        {
            base.ResetState();

            // Reset Cemu version labels
            lblSrcVersionNr.Text = "";
            lblDestVersionNr.Text = "";
            lblSrcCemuVersion.Visible = false;
            if (!DownloadMode)
                lblDestCemuVersion.Visible = false;

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

        private void ParseSuppliedVersionInCombobox(object sender, EventArgs e)
        {
            // If the parsing goes wrong, the combobox sets automatically its value to "Latest"
            // And if the user selects "Latest", the parsing will obviously fail resetting destCemuExeVersion to null
            if (!VersionNumber.TryParse(comboBoxVersion.Text, out destCemuExeVersion))
                comboBoxVersion.SelectedIndex = 0;
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
