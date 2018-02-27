using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    public partial class MigrationForm : Form
    {
        Worker worker;
        OptionsManager opts;
        bool overallProgressBarMaxDivided = false,
             singleProgressBarMaxDivided = false;
        bool srcFolderTxtBoxValidated = false,
             destFolderTxtBoxValidated = false;
        VersionNumber oldCemuExeVer, newCemuExeVer;

        public bool DownloadMode { get; }   // value that determinates if newer version of Cemu must be downloaded before migrating

        public MigrationForm(bool downloadMode)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            DownloadMode = downloadMode;
            opts = new OptionsManager();
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

        private async void DoOperationsAsync(object sender, EventArgs e)
        {
            WorkOutcome result = WorkOutcome.Undetermined;
            
            // Check if Cemu versions are ok, if not warn the user
            if (oldCemuExeVer > newCemuExeVer)
            {
                DialogResult choice = MessageBox.Show("You're trying to migrate from a newer Cemu version to an older one. " +
                    "This may cause severe incompatibility issues. Do you want to continue?", "Unsafe operation requested", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (choice == DialogResult.No)
                    return;
            }

            lblSingleProgress.Text = "Preparing...";

            // Get the list of folders to copy telling the method if source Cemu version is >= 1.10
            List<string> foldersToCopy = opts.GetFoldersToCopy(oldCemuExeVer.Major > 1 || oldCemuExeVer.Minor >= 10);

            // Check if the list is empty (no folders to copy)
            if (foldersToCopy.Count == 0)
            {
                MessageBox.Show("It seems that there are no folders to copy. Probably you set up options incorrectly.",
                    "Empty folders list", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Set Cemu folders in the class
            worker = new Worker(txtBoxOldFolder.Text, txtBoxNewFolder.Text, foldersToCopy);

            // TODO: perform download operations

            if (!(worker.IsAborted || worker.IsCancelled))
            {
                // Set overall progress bar according to overall size
                long overallSize = worker.CalculateFoldersSizes();
                if (overallSize > Int32.MaxValue)
                {
                    overallSize /= 1000;
                    overallProgressBarMaxDivided = true;
                }
                progressBarOverall.Maximum = Convert.ToInt32(overallSize);

                // Start operations in a secondary thread and enable/disable buttons after operations have started
                var operationsTask = Task.Run(() => worker.PerformMigrationOperations(opts.migrationOptions, ResetSingleProgressBar,
                                          UpdateCurrentFileText, UpdateProgressBars));
                btnCancel.Enabled = true;
                btnStart.Enabled = false;

                // Yield control to the form
                result = await operationsTask;
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

            // ... and reset form controls to their original state
            ResetEverything(result);
        }

        private void CancelOperations(object sender, EventArgs e)
        {
            lblSingleProgress.Text = "Cancelling...";
            worker.StopWork(WorkOutcome.CancelledByUser);
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

        /*
         *  Callback method that updates progress bars after a file has been copied
         */
        private void UpdateProgressBars(long dim)
        {
            if (singleProgressBarMaxDivided)
                progressBarSingle.Value += Convert.ToInt32(dim/1000);
            else
                progressBarSingle.Value += Convert.ToInt32(dim);

            if (overallProgressBarMaxDivided)
                progressBarOverall.Value += Convert.ToInt32(dim/1000);
            else
                progressBarOverall.Value += Convert.ToInt32(dim);

            lblPercentSingle.Text = Math.Floor(progressBarSingle.Value / (double)progressBarSingle.Maximum * 100) + "%";
            lblPercentOverall.Text = Math.Floor(progressBarOverall.Value / (double)progressBarOverall.Maximum * 100) + "%";
        }

        /*
         *  Callback method that resets single progress bar after a folder operation has been completed and
         *  updates labels and progress bars according to the next task
         */
        private void ResetSingleProgressBar(string newLabelText, long newProgressBarSize)
        {
            singleProgressBarMaxDivided = false;
            progressBarSingle.Value = 0;
            lblPercentSingle.Text = "0%";

            lblSingleProgress.Text = $"{newLabelText}...";

            if (newProgressBarSize > Int32.MaxValue)
            {
                newProgressBarSize /= 1000;
                singleProgressBarMaxDivided = true;
            }
            progressBarSingle.Maximum = Convert.ToInt32(newProgressBarSize);
        }

        /*
         *  Callback method that tells the form the outcome of the task, resets the GUI and FileWorker class
         */
        private void ResetEverything(WorkOutcome outcome)
        {
            if (outcome == WorkOutcome.Success)
                MessageBox.Show("Operation successfully terminated.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else if (outcome == WorkOutcome.Aborted)
                MessageBox.Show("Operation aborted due to an unexpected error.", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            else if (outcome == WorkOutcome.CancelledByUser)
                MessageBox.Show("Operation cancelled by user.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else if (outcome == WorkOutcome.CompletedWithErrors)
                MessageBox.Show("Operation terminated with errors.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Reset progress bars
            progressBarSingle.Value = 0;
            progressBarOverall.Value = 0;
            lblPercentSingle.Text = "0%";
            lblPercentOverall.Text = "0%";
            lblSingleProgress.Text = "Waiting for operations to start...";

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

            worker = null;
        }

        /*
         *  Callback method that updates the single progress bar's current file text every time there's a file to copy
         */
        private void UpdateCurrentFileText(string name)
        {
            if (name.Length > 50)
                name = name.Substring(0,49) + "...";
            // TODO: add logging in Details section
        }
    }
}
