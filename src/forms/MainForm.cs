using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    public partial class MainForm : Form
    {
        Worker worker;
        OptionsManager opts;
        bool overallProgressBarMaxDivided = false,
             singleProgressBarMaxDivided = false;
        bool srcFolderTxtBoxValidated = false,
             destFolderTxtBoxValidated = false;
        VersionNumber oldCemuExeVer, newCemuExeVer;

        public MainForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            opts = new OptionsManager();
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
            new AboutForm().ShowDialog();
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            new OptionsForm(opts).ShowDialog();
        }

        /*
         *  Folder selection using FolderBrowserDialog for old Cemu folder
         */
        private void btnSelectOldFolder_Click(object sender, EventArgs e)
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
        private void btnSelectNewFolder_Click(object sender, EventArgs e)
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

            // Set Cemu folders in the class
            worker = new Worker(txtBoxOldFolder.Text, txtBoxNewFolder.Text);

            // Get the list of folders to copy telling the method if source Cemu version is >= 1.10
            List<string> foldersToCopy = opts.GetFoldersToCopy(oldCemuExeVer.Major > 1 || oldCemuExeVer.Minor >= 10);

            // Check if the list is empty (no folders to copy)
            if (foldersToCopy.Count == 0)
            {
                MessageBox.Show("It seems that there are no folders to copy. Probably you set up options incorrectly.",
                    "Empty folders list", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // TODO: perform download operations

            if (!(worker.isAborted || worker.isCancelled))
            {
                // Set overall progress bar according to overall size
                long overallSize = FileOperations.CalculateFoldersSizes(foldersToCopy, worker);
                if (overallSize > Int32.MaxValue)
                {
                    overallSize /= 1000;
                    overallProgressBarMaxDivided = true;
                }
                progressBarOverall.Maximum = Convert.ToInt32(overallSize);

                // Start operations in a secondary thread and enable/disable buttons after operations have started
                var operationsTask = Task.Run(() => worker.PerformMigrationOperations(foldersToCopy, opts.additionalOptions, ResetSingleProgressBar,
                                          UpdateCurrentFileText, UpdateProgressBars));
                btnCancel.Enabled = true;
                btnStart.Enabled = false;

                // Yield control to the form
                result = await operationsTask;
            }

            // Once work has completed, ask if user wants to create Cemu desktop shortcut...
            if (result != WorkOutcome.Aborted && result != WorkOutcome.CancelledByUser && opts.additionalOptions["askForDesktopShortcut"] == true)
            {
                DialogResult choice = MessageBox.Show("Do you want to create a desktop shortcut?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                bool isNewCemuVersionAtLeast110 = newCemuExeVer.Major > 1 || newCemuExeVer.Minor >= 10;
                if (choice == DialogResult.Yes)     // mlc01 folder external path is passed only if needed
                    worker.CreateDesktopShortcut(newCemuExeVer.ToString(),
                      (isNewCemuVersionAtLeast110 && opts.additionalOptions["dontCopyMlcFolderFor1.10+"] == true) ? opts.mlcFolderExternalPath : null);
            }

            // ... and reset form controls to their original state
            ResetEverything(result);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            lblSingleProgress.Text = "Cancelling...";
            worker.StopWork(WorkOutcome.CancelledByUser);
        }

        private void txtBoxOldFolder_TextChanged(object sender, EventArgs e)
        {
            if (!FileOperations.DirectoryExists(txtBoxOldFolder.Text))
            {
                // Check if input directory exists
                errProviderOldFolder.SetError(txtBoxOldFolder, "Directory does not exist");
                srcFolderTxtBoxValidated = false;
                btnStart.Enabled = false;

                lblOldCemuVersion.Visible = false;
                lblOldVersionNr.Text = "";
            }
            else if (!FileOperations.FileExists(Path.Combine(txtBoxOldFolder.Text, "Cemu.exe")))
            {
                // Check if it's a valid Cemu installation
                errProviderOldFolder.SetError(txtBoxOldFolder, "Not a valid Cemu installation (Cemu.exe is missing)");
                srcFolderTxtBoxValidated = false;
                btnStart.Enabled = false;

                lblOldCemuVersion.Visible = false;
                lblOldVersionNr.Text = "";
            }
            else
            {
                // Display Cemu version label and verify if all user inputs are OK
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

        private void txtBoxNewFolder_TextChanged(object sender, EventArgs e)
        {
            if (!FileOperations.DirectoryExists(txtBoxNewFolder.Text))
            {
                // Check if input directory exists
                errProviderNewFolder.SetError(txtBoxNewFolder, "Directory does not exist");
                destFolderTxtBoxValidated = false;
                btnStart.Enabled = false;

                lblNewCemuVersion.Visible = false;
                lblNewVersionNr.Text = "";
            }
            else if (!FileOperations.FileExists(Path.Combine(txtBoxNewFolder.Text, "Cemu.exe")))
            {
                // Check if it's a valid Cemu installation
                errProviderNewFolder.SetError(txtBoxNewFolder, "Not a valid Cemu installation (Cemu.exe is missing)");
                destFolderTxtBoxValidated = false;
                btnStart.Enabled = false;

                lblNewCemuVersion.Visible = false;
                lblNewVersionNr.Text = "";
            }
            else
            {
                // Display Cemu version label and verify if all user inputs are OK
                errProviderNewFolder.SetError(txtBoxNewFolder, "");
                destFolderTxtBoxValidated = true;

                newCemuExeVer = new VersionNumber(FileVersionInfo.GetVersionInfo(Path.Combine(txtBoxNewFolder.Text, "Cemu.exe")), 3);
                lblNewCemuVersion.Visible = true;
                lblNewVersionNr.Text = newCemuExeVer.ToString();

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
            txtBoxOldFolder.TextChanged -= txtBoxOldFolder_TextChanged;
            txtBoxOldFolder.Text = "";
            txtBoxOldFolder.TextChanged += txtBoxOldFolder_TextChanged;
            txtBoxNewFolder.TextChanged -= txtBoxNewFolder_TextChanged;            
            txtBoxNewFolder.Text = "";            
            txtBoxNewFolder.TextChanged += txtBoxNewFolder_TextChanged;
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
