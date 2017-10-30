using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    public partial class MainForm : Form
    {
        FileWorker worker;
        OptionsManager opts;
        bool overallProgressBarMaxDivided = false;
        bool singleProgressBarMaxDivided = false;
        bool oldFolderValidated = false;
        bool newFolderValidated = false;
        FileVersionInfo oldCemuExe, newCemuExe;

        public MainForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            opts = new OptionsManager();
            progressBarSingle.DisplayStyle = ProgressBarDisplayText.CustomText;
            progressBarOverall.DisplayStyle = ProgressBarDisplayText.CustomText;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            new HelpForm().Show();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            new AboutForm().Show();
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            new OptionsForm(opts).Show();
        }

        private void btnSelectOldFolder_Click(object sender, EventArgs e)
        {
            // Folder selection using FolderBrowserDialog for old Cemu folder
            var folderPicker = new FolderBrowserDialog();
            folderPicker.RootFolder = Environment.SpecialFolder.UserProfile;
            DialogResult result = folderPicker.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderPicker.SelectedPath))
            {
                if (folderPicker.SelectedPath != txtBoxNewFolder.Text)
                    txtBoxOldFolder.Text = folderPicker.SelectedPath;
                else
                    MessageBox.Show("Source and destination folder must be different.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSelectNewFolder_Click(object sender, EventArgs e)
        {
            // Folder selection using FolderBrowserDialog for new Cemu folder
            var folderPicker = new FolderBrowserDialog();
            folderPicker.RootFolder = Environment.SpecialFolder.UserProfile;
            DialogResult result = folderPicker.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderPicker.SelectedPath))
            {
                if (folderPicker.SelectedPath != txtBoxOldFolder.Text)
                    txtBoxNewFolder.Text = folderPicker.SelectedPath;
                else
                    MessageBox.Show("Source and destination folder must be different.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool OldVersionCheck()
        {
            // Checks if source Cemu version is actually older than destination one
            if (oldCemuExe.FileMajorPart > newCemuExe.FileMajorPart)
                return false;
            else if (oldCemuExe.FileMajorPart == newCemuExe.FileMajorPart)
            {
                if (oldCemuExe.FileMinorPart > newCemuExe.FileMinorPart)
                    return false;
                else if (oldCemuExe.FileMinorPart == newCemuExe.FileMinorPart)
                {
                    if (oldCemuExe.FileBuildPart > newCemuExe.FileBuildPart)
                        return false;
                    else
                        return true;
                }
                else
                    return true;
            }
            else
                return true; 
        }

        private void StartOperations(object sender, EventArgs e)
        {
            // Check if Cemu versions are ok, if not warn the user
            if (OldVersionCheck() == false)
            {
                DialogResult choice = MessageBox.Show("You're trying to migrate from a newer Cemu version to an older one. " +
                    "This may cause severe incompatibility issues. Do you want to continue?", "Unsafe operation requested", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (choice == DialogResult.No)
                    return;
            }

            lblSingleProgress.Text = "Preparing...";

            // Set Cemu folders in the class
            worker = new FileWorker(txtBoxOldFolder.Text, txtBoxNewFolder.Text);

            // Get the list of folders to copy 
            List<string> foldersToCopy = opts.GetFoldersToCopy();

            // Check if the list is empty (no folders to copy)
            if (foldersToCopy.Count == 0)
            {
                MessageBox.Show("It seems that there are no folders to copy. Probably you set up options incorrectly.", "Empty folders list", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Set overall progress bar according to overall size
            long overallSize = FileOperations.CalculateFoldersSizes(foldersToCopy, worker);
            if (overallSize > Int32.MaxValue)
            {
                overallSize /= 1000;
                overallProgressBarMaxDivided = true;
            }
            progressBarOverall.Maximum = Convert.ToInt32(overallSize);
            
            // Start operations in a secondary thread
            Thread operationsThread = new Thread(() => worker.PerformOperations(foldersToCopy, ResetSingleProgressBar, UpdateCurrentFileText, UpdateProgressBars, ResetEverything));
            operationsThread.Start();

            // Enable/disable buttons
            btnCancel.Enabled = true;
            btnStart.Enabled = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            lblSingleProgress.Text = "Cancelling...";
            worker.CancelWork();
        }

        private void txtBoxOldFolder_TextChanged(object sender, EventArgs e)
        {
            if (!FileOperations.DirectoryExists(txtBoxOldFolder.Text))
            {
                // Check if input directory exists
                errProviderOldFolder.SetError(txtBoxOldFolder, "Directory does not exist");
                oldFolderValidated = false;
                btnStart.Enabled = false;

                lblOldCemuVersion.Visible = false;
                lblOldVersionNr.Text = "";
            }
            else if (!FileOperations.FileExists(Path.Combine(txtBoxOldFolder.Text, "Cemu.exe")))
            {
                // Check if it's a valid Cemu installation
                errProviderOldFolder.SetError(txtBoxOldFolder, "Not a valid Cemu installation (Cemu.exe is missing)");
                oldFolderValidated = false;
                btnStart.Enabled = false;

                lblOldCemuVersion.Visible = false;
                lblOldVersionNr.Text = "";
            }
            else
            {
                // Display Cemu version label and verify if all user inputs are OK
                errProviderOldFolder.SetError(txtBoxOldFolder, "");
                oldFolderValidated = true;

                oldCemuExe = FileVersionInfo.GetVersionInfo(Path.Combine(txtBoxOldFolder.Text, "Cemu.exe"));
                lblOldCemuVersion.Visible = true;
                lblOldVersionNr.Text = oldCemuExe.FileMajorPart + "." + oldCemuExe.FileMinorPart + "." + oldCemuExe.FileBuildPart;

                if ((oldFolderValidated && newFolderValidated) && (txtBoxOldFolder.Text != txtBoxNewFolder.Text))
                    btnStart.Enabled = true;
                else
                    btnStart.Enabled = false;
            }
        }

        private void txtBoxNewFolder_TextChanged(object sender, EventArgs e)
        {
            if (!FileOperations.DirectoryExists(txtBoxNewFolder.Text))
            {
                // Check if input directory exists
                errProviderNewFolder.SetError(txtBoxNewFolder, "Directory does not exist");
                newFolderValidated = false;
                btnStart.Enabled = false;

                lblNewCemuVersion.Visible = false;
                lblNewVersionNr.Text = "";
            }
            else if (!FileOperations.FileExists(Path.Combine(txtBoxNewFolder.Text, "Cemu.exe")))
            {
                // Check if it's a valid Cemu installation
                errProviderNewFolder.SetError(txtBoxNewFolder, "Not a valid Cemu installation (Cemu.exe is missing)");
                newFolderValidated = false;
                btnStart.Enabled = false;

                lblNewCemuVersion.Visible = false;
                lblNewVersionNr.Text = "";
            }
            else
            {
                // Display Cemu version label and verify if all user inputs are OK
                errProviderNewFolder.SetError(txtBoxNewFolder, "");
                newFolderValidated = true;

                newCemuExe = FileVersionInfo.GetVersionInfo(Path.Combine(txtBoxNewFolder.Text, "Cemu.exe"));
                lblNewCemuVersion.Visible = true;
                lblNewVersionNr.Text = newCemuExe.FileMajorPart + "." + newCemuExe.FileMinorPart + "." + newCemuExe.FileBuildPart;

                if ((oldFolderValidated && newFolderValidated) && (txtBoxOldFolder.Text != txtBoxNewFolder.Text))
                    btnStart.Enabled = true;
                else
                    btnStart.Enabled = false;
            }
        }

        private void UpdateProgressBars(long dim)
        {
            // Callback that updates progress bars after a file has been copied
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

        private void ResetSingleProgressBar(string copyingFolder, long currentFolderSize)
        {
            // Callback that resets single progress bar after a folder has been copied and there's another to copy
            singleProgressBarMaxDivided = false;
            progressBarSingle.Value = 0;
            lblPercentSingle.Text = "0%";

            lblSingleProgress.Text = "Copying " + copyingFolder + "...";    // TO BE IMPROVED

            if (currentFolderSize > Int32.MaxValue)
            {
                currentFolderSize /= 1000;
                singleProgressBarMaxDivided = true;
            }
            progressBarSingle.Maximum = Convert.ToInt32(currentFolderSize);
        }

        private void ResetEverything(WorkOutcome outcome)
        {
            // Callback that tells the form the outcome of the task, resets the GUI and FileOperations class
            if (outcome == WorkOutcome.Success)
                MessageBox.Show("Operation successfully terminated.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else if (outcome == WorkOutcome.Aborted)
                MessageBox.Show("Operation aborted due to an unexpected error.", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            else if (outcome == WorkOutcome.CancelledByUser)
                MessageBox.Show("Operation cancelled by user.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else if (outcome == WorkOutcome.CompletedWithErrors)
                MessageBox.Show("Operation terminated with errors.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

            progressBarSingle.SetCustomText("");
            progressBarSingle.Value = 0;
            progressBarOverall.Value = 0;
            lblPercentSingle.Text = "0%";
            lblPercentOverall.Text = "0%";
            lblSingleProgress.Text = "Waiting for operations to start...";

            txtBoxOldFolder.Text = "";
            txtBoxNewFolder.Text = "";
            btnCancel.Enabled = false;

            worker = null;
        }

        private void UpdateCurrentFileText(string name)
        {
            // Callback that updates the single progress bar's current file text every time there's a file to copy
            if (name.Length > 50)
                name = name.Substring(0,49) + "...";
            progressBarSingle.SetCustomText("Current file: " + name);
        }
    }
}
