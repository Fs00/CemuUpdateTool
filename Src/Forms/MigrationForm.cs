using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CemuUpdateTool.Settings;
using CemuUpdateTool.Utils;
using CemuUpdateTool.Workers;

namespace CemuUpdateTool.Forms
{
    /*
     *  Window that provides Migrate and Download & Migrate functionality.
     */
    sealed partial class MigrationForm : OperationsForm
    {
        private bool sourceFolderTxtBoxValidated = false,
                     destinationFolderTxtBoxValidated = false;
        private VersionNumber sourceCemuVersion, destinationCemuVersion;
        
        private Migrator migrator;
        private bool migrationOperationsStarted;
        private readonly bool downloadMode;    // if true, newer version of Cemu will be downloaded before migrating

        public MigrationForm(bool downloadMode) : base()
        {
            InitializeComponent();
            this.downloadMode = downloadMode;

            if (this.downloadMode)
                SetControlsForDownloadMode();
            else
                SetControlsForMigrateMode();

            RemoveDefaultTextBoxContextMenus();
        }
        
        private void SetControlsForDownloadMode()
        {
            lblTitle.Text = "Download & Migrate";
            lblDestinationCemuVersion.Visible = true;
            flowPanelDestination.Controls.Remove(lblDestinationCemuVersionNumber);
            DisplayLatestTextInDestinationVersionCombobox();
        }

        private void SetControlsForMigrateMode()
        {
            lblTitle.Text = "Migrate";
            flowPanelDestination.Controls.Remove(comboBoxDestinationVersion);
        }

        protected override void RemoveDefaultTextBoxContextMenus()
        {
            base.RemoveDefaultTextBoxContextMenus();
            txtBoxSourceFolder.ContextMenuStrip = new ContextMenuStrip();
            txtBoxDestinationFolder.ContextMenuStrip = new ContextMenuStrip();
        }
        
        private void SelectCemuFolder(object sender, EventArgs e)
        {
            var cemuFolderTextBox = GetTextBoxNearTo((Button) sender);
            string chosenFolder = ChooseFolderWithPicker(cemuFolderTextBox.Text);
            if (chosenFolder == null)
                return;
            
            if (chosenFolder != GetOtherCemuFolderTextBox(cemuFolderTextBox).Text)
                cemuFolderTextBox.Text = chosenFolder;
            else
                MessageBox.Show("Source and destination folder must be different.", "Error!", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private TextBox GetTextBoxNearTo(Button button)
        {
            if (button == btnSelectSourceFolder)
                return txtBoxSourceFolder;
            if (button == btnSelectDestinationFolder)
                return txtBoxDestinationFolder;
            
            throw new ArgumentException("The button has no text boxes near itself.");
        }

        private TextBox GetOtherCemuFolderTextBox(TextBox thisTextBox)
        {
            if (thisTextBox == txtBoxSourceFolder)
                return txtBoxDestinationFolder;
            if (thisTextBox == txtBoxDestinationFolder)
                return txtBoxSourceFolder;
            
            throw new ArgumentException($"Argument {nameof(thisTextBox)} is not a Cemu folder text box.");
        }

        private void CheckSourceFolderTextBoxContent(object sender, EventArgs e)
        {
            if (!DirectoryContainsACemuInstallation(txtBoxSourceFolder.Text, out string reason))
            {
                errProviderFolders.SetError(txtBoxSourceFolder, reason);
                sourceFolderTxtBoxValidated = false;
                btnStart.Enabled = false;
                HideSourceCemuVersionLabel();
            }
            else
            {
                errProviderFolders.SetError(txtBoxSourceFolder, "");
                sourceFolderTxtBoxValidated = true;
                DisplaySourceCemuVersion();
                EnableStartButtonIfAllInputIsValid();
            }
        }

        private void HideSourceCemuVersionLabel()
        {
            lblSourceCemuVersion.Visible = false;
            lblSourceCemuVersionNumber.Text = "";
        }

        private void DisplaySourceCemuVersion()
        {
            sourceCemuVersion = FileUtils.RetrieveExecutableVersionNumber(Path.Combine(txtBoxSourceFolder.Text, "Cemu.exe"));
            lblSourceCemuVersion.Visible = true;
            lblSourceCemuVersionNumber.Text = sourceCemuVersion.ToString();
        }

        private void CheckDestinationFolderTextBoxContent(object sender, EventArgs e)
        {
            if (!DestinationFolderTextBoxContentIsValid(out string reason))
            {
                errProviderFolders.SetError(txtBoxDestinationFolder, reason);
                destinationFolderTxtBoxValidated = false;
                btnStart.Enabled = false;
                if (!downloadMode)
                    HideDestinationCemuVersionLabel();
            }
            else
            {
                errProviderFolders.SetError(txtBoxDestinationFolder, "");
                destinationFolderTxtBoxValidated = true;
                if (!downloadMode)
                    DisplayDestinationCemuVersion();

                EnableStartButtonIfAllInputIsValid();
            }
        }

        private bool DestinationFolderTextBoxContentIsValid(out string reason)
        {
            reason = null;
            if (downloadMode)
            {
                if (txtBoxDestinationFolder.Text != "" && Directory.Exists(txtBoxDestinationFolder.Text))
                    return true;
                
                reason = "Directory does not exist";
                return false;
            }

            return DirectoryContainsACemuInstallation(txtBoxDestinationFolder.Text, out reason);
        }
        
        private void HideDestinationCemuVersionLabel()
        {
            lblDestinationCemuVersion.Visible = false;
            lblDestinationCemuVersionNumber.Text = "";
        }
        
        private void DisplayDestinationCemuVersion()
        {
            destinationCemuVersion =
                FileUtils.RetrieveExecutableVersionNumber(Path.Combine(txtBoxDestinationFolder.Text, "Cemu.exe"));
            lblDestinationCemuVersion.Visible = true;
            lblDestinationCemuVersionNumber.Text = destinationCemuVersion.ToString();
        }

        private void EnableStartButtonIfAllInputIsValid()
        {
            if (sourceFolderTxtBoxValidated && destinationFolderTxtBoxValidated &&
                txtBoxSourceFolder.Text != txtBoxDestinationFolder.Text)
                btnStart.Enabled = true;
            else
                btnStart.Enabled = false;
        }

        protected override async Task<WorkOutcome> PerformOperationsAsync()
        {
            if (downloadMode)
                await PerformDownloadOperationsAsync();
            await PerformMigrationOperationsAsync();

            if (Options.Migration[OptionKey.AskForDesktopShortcut])
                CreateDesktopShortcutIfUserWantsTo();

            if (migrator.ErrorsEncountered > 0)
                return WorkOutcome.CompletedWithErrors;
            
            return WorkOutcome.Success;
        }

        private async Task PerformDownloadOperationsAsync()
        {
            var downloader = new Downloader(txtBoxDestinationFolder.Text, cTokenSource.Token);
            AttachProgressEventHandlersToWorker(downloader);
            destinationCemuVersion = await Task.Run(() => downloader.PerformDownloadOperations(destinationCemuVersion));

            UpdateLastKnownCemuVersionOption(destinationCemuVersion);
            TryUpdateOptionsFile();
        }
        
        private async Task PerformMigrationOperationsAsync()
        {
            migrator = new Migrator(txtBoxSourceFolder.Text, txtBoxDestinationFolder.Text, sourceCemuVersion, cTokenSource.Token);
            AttachProgressEventHandlersToWorker(migrator);
            migrationOperationsStarted = true;
            await Task.Run(migrator.PerformMigrationOperations);
        }
        
        private void CreateDesktopShortcutIfUserWantsTo()
        {
            DialogResult choice = MessageBox.Show("Do you want to create a desktop shortcut?", "",
                                                  MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (choice == DialogResult.Yes)
                migrator.CreateDesktopShortcut(destinationCemuVersion);
        }

        protected override bool ArePreliminaryChecksSuccessful()
        {
            if (downloadMode)
            {
                if (DirectoryContainsACemuInstallation(txtBoxDestinationFolder.Text, out string _) &&
                    !UserWantsToOverwriteExistingCemuInstallation())
                    return false;
            }
            else
            {
                if (sourceCemuVersion > destinationCemuVersion && !UserWantsToMigrateFromNewerToOlderVersion())
                    return false;
            }

            if (IsThereNothingToMigrate())
                return false;

            return true;
        }
        
        private static bool UserWantsToOverwriteExistingCemuInstallation()
        {
            DialogResult choice = MessageBox.Show("The chosen destination folder already contains a Cemu installation. " +
                                                  "Do you want to overwrite it?", "Cemu installation already present",
                                                  MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            return choice == DialogResult.Yes;
        }
        
        private static bool UserWantsToMigrateFromNewerToOlderVersion()
        {
            DialogResult choice = MessageBox.Show("You're trying to migrate from a newer Cemu version to an older one. " +
                                                  "This may cause severe incompatibility issues. Do you want to continue?",
                                                  "Unsafe operation requested",
                                                  MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            return choice == DialogResult.Yes;
        }

        private static bool IsThereNothingToMigrate()
        {
            if (!Options.FoldersToMigrate.GetAllEnabled().Any() && !Options.FilesToMigrate.GetAllEnabled().Any())
            {
                MessageBox.Show(
                    "It seems that there are neither folders nor single files to copy. " +
                    "Probably you set up options incorrectly.",
                    "Empty folders and files lists", MessageBoxButtons.OK, MessageBoxIcon.Error
                );
                return true;
            }
            
            return false;
        }

        protected override void HandleOperationsError()
        {
            try
            {
                // We must check if migration operations are started because otherwise accessing migrator would throw
                // a NullReferenceException (not yet instantiated) or we would read data from an old Migrator instance
                if (migrationOperationsStarted && (migrator.CreatedFiles.Count > 0 || migrator.CreatedDirectories.Count > 0))
                {
                    DialogResult choice = MessageBox.Show("Do you want to delete files that have already been created?",
                                                          "Operation stopped", MessageBoxButtons.YesNo);
                    if (choice == DialogResult.Yes)
                    {
                        migrator.DeleteCreatedFilesAndFolders();
                        if (downloadMode)
                            FileUtils.RemoveDirectoryContents(txtBoxDestinationFolder.Text);
                    }
                }
            }
            catch (Exception cleanupExc)
            {
                MessageBox.Show($"An error occurred when deleting created files: {cleanupExc.Message}", "Unexpected error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        protected override void PrepareControlsForOperations()
        {
            base.PrepareControlsForOperations();
            btnOptions.Enabled = false;
            LockFolderTextBoxesAndSelectionButtons();
            migrationOperationsStarted = false;
        }

        private void LockFolderTextBoxesAndSelectionButtons()
        {
            txtBoxSourceFolder.ReadOnly = true;
            txtBoxDestinationFolder.ReadOnly = true;
            btnSelectSourceFolder.Enabled = false;
            btnSelectDestinationFolder.Enabled = false;
        }

        protected override void ResetControls()
        {
            base.ResetControls();
            btnOptions.Enabled = true;
            ResetCemuVersionLabels();
            ResetCemuFolderTextBoxes();
            UnlockFolderTextBoxesAndSelectionButtons();
        }

        private void ResetCemuVersionLabels()
        {
            lblSourceCemuVersionNumber.Text = "";
            lblDestinationCemuVersionNumber.Text = "";
            lblSourceCemuVersion.Visible = false;
            if (!downloadMode)
                lblDestinationCemuVersion.Visible = false;
        }

        private void ResetCemuFolderTextBoxes()
        {
            txtBoxSourceFolder.TextChanged -= CheckSourceFolderTextBoxContent;
            txtBoxSourceFolder.Text = "";
            txtBoxSourceFolder.TextChanged += CheckSourceFolderTextBoxContent;
            txtBoxDestinationFolder.TextChanged -= CheckDestinationFolderTextBoxContent;
            txtBoxDestinationFolder.Text = "";
            txtBoxDestinationFolder.TextChanged += CheckDestinationFolderTextBoxContent;
            
            sourceFolderTxtBoxValidated = false;
            destinationFolderTxtBoxValidated = false;
        }
        
        private void UnlockFolderTextBoxesAndSelectionButtons()
        {
            txtBoxSourceFolder.ReadOnly = false;
            txtBoxDestinationFolder.ReadOnly = false;
            btnSelectSourceFolder.Enabled = true;
            btnSelectDestinationFolder.Enabled = true;
        }

        private void ParseSuppliedVersionInCombobox(object sender, EventArgs e)
        {
            // If the parsing goes wrong, the combobox sets automatically its value to "Latest"
            // And if the user selects "Latest", version number parsing will fail resetting destinationCemuVersion to null
            if (!VersionNumber.TryParse(comboBoxDestinationVersion.Text, out destinationCemuVersion))
                DisplayLatestTextInDestinationVersionCombobox();
        }

        private void DisplayLatestTextInDestinationVersionCombobox()
        {
            comboBoxDestinationVersion.SelectedIndex = 0;
        }

        private void OpenOptionsForm(object sender, EventArgs e)
        {
            using (var form = new OptionsForm())
                form.ShowDialog();
        }

        // These "fake overrides" are needed on Visual Studio to avoid form designer errors
        /*protected override void ResizeFormOnLogTextBoxVisibleChanged(object sender, EventArgs e)  { base.ResizeFormOnLogTextBoxVisibleChanged(sender, e); }
        protected override void PasteContentIntoTextBoxOnDragDrop(object sender, DragEventArgs e)  { base.PasteContentIntoTextBoxOnDragDrop(sender, e); }
        protected override void ChangeCursorEffectOnTextBoxDragEnter(object sender, DragEventArgs e)  { base.ChangeCursorEffectOnTextBoxDragEnter(sender, e); }*/
    }
}
