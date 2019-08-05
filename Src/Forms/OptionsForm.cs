using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using CemuUpdateTool.Utils;
using CemuUpdateTool.Settings;
using System.IO;

namespace CemuUpdateTool.Forms
{
    /*
     *  Window that allows the user to edit the options of the program
     */
    public partial class OptionsForm : Form
    {
        private bool optionsFileLocationChanged;

        // These will contain the unsaved changes made to custom files/folders if the user edits them
        private Dictionary<string, bool> updatedCustomFolders, updatedCustomFiles;

        public OptionsForm()
        {
            InitializeComponent();
            SetCheckboxesAccordingToOptions();
            DrawIconInsideHelpButton();
            RemoveDefaultTextBoxContextMenus();
        }
        
        private void DrawIconInsideHelpButton()
        {
            var iconBitmap = new Bitmap(btnHelp.Width - 6, btnHelp.Height - 6);
            using (Graphics gfx = Graphics.FromImage(iconBitmap))
            {
                gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                gfx.DrawImage(SystemIcons.Question.ToBitmap(), new Rectangle(Point.Empty, iconBitmap.Size));
            }
            btnHelp.Image = iconBitmap;
        }
        
        private void RemoveDefaultTextBoxContextMenus()
        {
            txtBoxCustomMlcPath.ContextMenuStrip = new ContextMenuStrip();
            txtBoxUrlSuffix.ContextMenuStrip = new ContextMenuStrip();
            txtBoxBaseUrl.ContextMenuStrip = new ContextMenuStrip();
        }

        private void SetCheckboxesAccordingToOptions()
        {
            SetFoldersCheckboxes();
            RefreshCustomEntriesStats();
            SetCemuSettingsFileCheckbox();
            SetMigrationOptionsControls();
            SetSettingsFileOptionsControls();
            PopulateDownloadOptionsTextBoxes();
        }

        private void SetFoldersCheckboxes()
        {
            chkBoxControllerProfiles.Checked = Options.FoldersToMigrate.IsEnabled(FolderOption.ControllerProfiles);
            chkBoxGameProfiles.Checked = Options.FoldersToMigrate.IsEnabled(FolderOption.GameProfiles);
            chkBoxGraphicPacks.Checked = Options.FoldersToMigrate.IsEnabled(FolderOption.GraphicPacks);
            chkBoxDLCAndUpdates.Checked = Options.FoldersToMigrate.IsEnabled(FolderOption.DLCAndUpdates);
            chkBoxShaderCaches.Checked = Options.FoldersToMigrate.IsEnabled(FolderOption.TransferableCaches);
            SetGameSavesFolderCheckbox();
        }

        private void SetGameSavesFolderCheckbox()
        {
            // Saves folder checkbox as indeterminate if only one of the two save folders is set to true
            if (Options.FoldersToMigrate.IsEnabled(FolderOption.GameSavesBeforeCemu1_11))
                chkBoxSavegames.CheckState = CheckState.Indeterminate;
            if (Options.FoldersToMigrate.IsEnabled(FolderOption.GameSavesAfterCemu1_11))
            {
                if (chkBoxSavegames.CheckState == CheckState.Indeterminate)
                    chkBoxSavegames.CheckState = CheckState.Checked;
                else
                    chkBoxSavegames.CheckState = CheckState.Indeterminate;
            }
        }

        private void SetCemuSettingsFileCheckbox()
        {
            // Settings file checkbox is set as indeterminate if only one of the two options files is set to true
            if (Options.FilesToMigrate.IsEnabled(FileOption.SettingsBin))
                chkBoxCemuSettings.CheckState = CheckState.Indeterminate;
            if (Options.FilesToMigrate.IsEnabled(FileOption.SettingsXml))
            {
                if (chkBoxCemuSettings.CheckState == CheckState.Indeterminate)
                    chkBoxCemuSettings.CheckState = CheckState.Checked;
                else
                    chkBoxCemuSettings.CheckState = CheckState.Indeterminate;
            }
        }

        private void SetMigrationOptionsControls()
        {
            chkBoxDeletePrevContent.Checked = Options.Migration[OptionKey.DeleteDestinationFolderContents];
            chkBoxDesktopShortcut.Checked = Options.Migration[OptionKey.AskForDesktopShortcut];
            chkBoxCustomMlcPath.Checked = Options.Migration[OptionKey.UseCustomMlcFolderIfSupported];
            txtBoxCustomMlcPath.Text = Options.CustomMlcFolderPath;
            SetCompatibilityOptionsCheckboxes();
        }

        private void SetCompatibilityOptionsCheckboxes()
        {
            chkBoxCompatOptions.Checked = Options.Migration[OptionKey.SetCompatibilityOptions];
            chkBoxRunAsAdmin.Checked = Options.Migration[OptionKey.CompatibilityRunAsAdmin];
            chkBoxNoFullscreenOptimiz.Checked = Options.Migration[OptionKey.CompatibilityNoFullscreenOptimizations];
            chkBoxOverrideHiDPIBehaviour.Checked = Options.Migration[OptionKey.CompatibilityOverrideHiDPIBehaviour];
        }

        private void SetSettingsFileOptionsControls()
        {
            if (string.IsNullOrEmpty(Options.CurrentOptionsFilePath))
                chkBoxSettingsOnFile.Checked = false;
            else
            {
                chkBoxSettingsOnFile.Checked = true;
                if (Options.CurrentOptionsFilePath == Options.LocalOptionsFilePath)
                    radioBtnLocalFolder.Checked = true;
                else if (Options.CurrentOptionsFilePath == Options.AppDataOptionsFilePath)
                    radioBtnAppDataFolder.Checked = true;
            }
        }
        
        private void PopulateDownloadOptionsTextBoxes()
        {
            // Remove "http://" from the URL because there's a label for it
            txtBoxBaseUrl.Text = Options.Download[OptionKey.CemuBaseUrl].Remove(0, 7);
            txtBoxUrlSuffix.Text = Options.Download[OptionKey.CemuUrlSuffix];
        }
        
        private void SetOptionsAccordingToCheckboxes()
        {
            SetFoldersOptions();
            SetCemuSettingsFilesOptions();
            SetMigrationOptions();

            if (chkBoxSettingsOnFile.Checked && optionsFileLocationChanged)
                UpdateOptionsFileLocation();
            else
                TryDeleteOptionsFile();

            if (!lblCemuDownloadUrlInvalid.Visible)
                SetDownloadOptions();
        }
        
        private static void TryDeleteOptionsFile()
        {
            try
            {
                Options.DeleteOptionsFile();
            }
            catch (Exception exc)
            {
                MessageBox.Show($"An error occurred when deleting previous options file: {exc.Message}", "",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetFoldersOptions()
        {
            Options.FoldersToMigrate[FolderOption.ControllerProfiles] = chkBoxControllerProfiles.Checked;
            Options.FoldersToMigrate[FolderOption.GameProfiles] = chkBoxGameProfiles.Checked;
            Options.FoldersToMigrate[FolderOption.GraphicPacks] = chkBoxGraphicPacks.Checked;
            if (chkBoxSavegames.CheckState != CheckState.Indeterminate)
            {
                Options.FoldersToMigrate[FolderOption.GameSavesBeforeCemu1_11] = chkBoxSavegames.Checked;
                Options.FoldersToMigrate[FolderOption.GameSavesAfterCemu1_11] = chkBoxSavegames.Checked;
            }
            Options.FoldersToMigrate[FolderOption.DLCAndUpdates] = chkBoxDLCAndUpdates.Checked;
            Options.FoldersToMigrate[FolderOption.TransferableCaches] = chkBoxShaderCaches.Checked;
        }

        private void SetCemuSettingsFilesOptions()
        {
            if (chkBoxCemuSettings.CheckState != CheckState.Indeterminate)
            {
                Options.FilesToMigrate[FileOption.SettingsBin] = chkBoxCemuSettings.Checked;
                Options.FilesToMigrate[FileOption.SettingsXml] = chkBoxCemuSettings.Checked;
            }
        }

        private void SetMigrationOptions()
        {
            Options.Migration[OptionKey.DeleteDestinationFolderContents] = chkBoxDeletePrevContent.Checked;
            Options.Migration[OptionKey.AskForDesktopShortcut] = chkBoxDesktopShortcut.Checked;
            SetCustomMlcPath();
            SetCompatibilityOptions();
        }

        private void SetCustomMlcPath()
        {
            Options.Migration[OptionKey.UseCustomMlcFolderIfSupported] = chkBoxCustomMlcPath.Checked;
            if (!string.IsNullOrWhiteSpace(txtBoxCustomMlcPath.Text) &&
                errProviderMlcFolder.GetError(txtBoxCustomMlcPath) == "")
                Options.CustomMlcFolderPath = txtBoxCustomMlcPath.Text;
            else
                Options.CustomMlcFolderPath = "";
        }

        private void SetCompatibilityOptions()
        {
            Options.Migration[OptionKey.SetCompatibilityOptions] = chkBoxCompatOptions.Checked;
            Options.Migration[OptionKey.CompatibilityRunAsAdmin] = chkBoxRunAsAdmin.Checked;
            Options.Migration[OptionKey.CompatibilityNoFullscreenOptimizations] = chkBoxNoFullscreenOptimiz.Checked;
            Options.Migration[OptionKey.CompatibilityOverrideHiDPIBehaviour] = chkBoxOverrideHiDPIBehaviour.Checked;
        }

        private void UpdateOptionsFileLocation()
        {
            TryDeleteOptionsFile();
            if (radioBtnLocalFolder.Checked)
                Options.CurrentOptionsFilePath = Options.LocalOptionsFilePath;
            else if (radioBtnAppDataFolder.Checked)
                Options.CurrentOptionsFilePath = Options.AppDataOptionsFilePath;

            if (File.Exists(Options.CurrentOptionsFilePath))
                TryLoadExistingOptionsFileIfUserWantsTo();
        }

        private static void TryLoadExistingOptionsFileIfUserWantsTo()
        {
            DialogResult choice = MessageBox.Show(
                "Another options file has been found in the folder you selected. Do you want to load it?",
                "Settings file found", MessageBoxButtons.YesNo, MessageBoxIcon.Information
            );
            
            if (choice == DialogResult.Yes)
            {
                try
                {
                    Options.LoadFromCurrentlySelectedFile();
                }
                catch
                {
                    MessageBox.Show("An error occurred when parsing options file. Previous settings will be kept.",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SetDownloadOptions()
        {
            Options.Download[OptionKey.CemuBaseUrl] = "http://" + txtBoxBaseUrl.Text;
            Options.Download[OptionKey.CemuUrlSuffix] = txtBoxUrlSuffix.Text;
        }

        private void RefreshCustomEntriesStats()
        {
            lblCustomFoldersCount.Text = Options.AllCustomFoldersToMigrate().Count().ToString();
            lblCustomFilesCount.Text = Options.AllCustomFilesToMigrate().Count().ToString();
        }

        private void RestoreDefaultOptions(object sender, EventArgs e)
        {
            DialogResult choice = MessageBox.Show(
                "Are you sure you want to restore the default settings?\r\nThis cannot be undone.",
                "Confirmation requested", MessageBoxButtons.YesNo, MessageBoxIcon.Warning
            );
            if (choice == DialogResult.Yes)
            {
                Options.ApplyDefaultOptions();
                SetCheckboxesAccordingToOptions();
            }
        }

        private void SaveOptionsAndClose(object sender, EventArgs e)
        {
            SetOptionsAccordingToCheckboxes();
            UpdateCustomOptions();
            TryWriteOptionsToFile();
            Close();
        }

        private void TryWriteOptionsToFile()
        {
            try
            {
                if (chkBoxSettingsOnFile.Checked)
                    Options.WriteOptionsToCurrentlySelectedFile();
                else
                    MessageBox.Show("Please take note that if you don't store options in a file, " +
                                    "they'll be lost as soon as you exit the application.", "Warning",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Unexpected error when saving options on file: {exc.Message} " +
                                "Changes won't be preserved after closing the program.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateCustomOptions()
        {
            if (updatedCustomFolders != null)
            {
                // Delete the old custom folder options from the dictionary
                List<string> oldCustomFolders = Options.AllCustomFoldersToMigrate().ToList();
                foreach (string folder in oldCustomFolders)
                    Options.FoldersToMigrate.Remove(folder);

                // ... and add the updated ones
                foreach (var folder in updatedCustomFolders)
                {
                    Options.FoldersToMigrate.Add(folder.Key);
                    Options.FoldersToMigrate[folder.Key] = folder.Value;
                }
            }

            if (updatedCustomFiles != null)
            {
                // Delete the old custom files options from the dictionary
                List<string> oldCustomFiles = Options.AllCustomFilesToMigrate().ToList();
                foreach (string file in oldCustomFiles)
                    Options.FilesToMigrate.Remove(file);

                // ... and add the updated ones
                foreach (var file in updatedCustomFiles)
                {
                    Options.FilesToMigrate.Add(file.Key);
                    Options.FilesToMigrate[file.Key] = file.Value;
                }
            }
        }
        
        private void OpenManageCustomFoldersDialog(object sender, EventArgs e)
        {
            // Initialize the updated dictionary if it's never been edited
            if (updatedCustomFolders == null)
            {
                updatedCustomFolders = Options.AllCustomFoldersToMigrate()
                    .ToDictionary(folder => folder, folder => Options.FoldersToMigrate[folder]);
            }

            using (var form = new OptionsDictionaryEditingForm("Manage custom folders", updatedCustomFolders, Options.AllDefaultFoldersToMigrate()))
                form.ShowDialog();
            lblCustomFoldersCount.Text = updatedCustomFolders.Count.ToString();
        }

        private void OpenManageCustomFilesDialog(object sender, EventArgs e)
        {
            // Initialize the updated dictionary if it's never been edited
            if (updatedCustomFiles == null)
            {
                updatedCustomFiles = Options.AllCustomFilesToMigrate()
                    .ToDictionary(file => file, file => Options.FilesToMigrate[file]);
            }

            using (var form = new OptionsDictionaryEditingForm("Manage custom files", updatedCustomFiles, Options.AllDefaultFilesToMigrate()))
                form.ShowDialog();
            lblCustomFilesCount.Text = updatedCustomFiles.Count.ToString();
        }

        /*
         *  Triggered when the user changes one of the options file location radio buttons.
         */
        private void CheckIfOptionsFileLocationHasChanged(object sender, EventArgs e)
        {
            if (Options.CurrentOptionsFilePath == Options.LocalOptionsFilePath)
                optionsFileLocationChanged = !radioBtnLocalFolder.Checked;
            else if (Options.CurrentOptionsFilePath == Options.AppDataOptionsFilePath)
                optionsFileLocationChanged = !radioBtnAppDataFolder.Checked;
        }

        /*
         *  Triggered when the user selects/deselects the "Save settings on file" checkbox.
         */
        private void UpdateOptionsFileLocationControlsEnabledState(object sender, EventArgs e)
        {
            lblFileLocation.Enabled = chkBoxSettingsOnFile.Checked;
            radioBtnLocalFolder.Enabled = chkBoxSettingsOnFile.Checked;
            radioBtnAppDataFolder.Enabled = chkBoxSettingsOnFile.Checked;
            
            if (chkBoxSettingsOnFile.Checked && string.IsNullOrEmpty(Options.CurrentOptionsFilePath))
                optionsFileLocationChanged = true;
        }

        private void UpdateCustomMlcPathTextBoxEnabledState(object sender, EventArgs e)
        {
            txtBoxCustomMlcPath.Enabled = chkBoxCustomMlcPath.Checked;
        }

        private void UpdateCompatOptionsCheckboxesEnabledState(object sender, EventArgs e)
        {
            chkBoxRunAsAdmin.Enabled = chkBoxCompatOptions.Checked;
            chkBoxNoFullscreenOptimiz.Enabled = chkBoxCompatOptions.Checked;
            chkBoxOverrideHiDPIBehaviour.Enabled = chkBoxCompatOptions.Checked;
        }

        private void CheckCustomMlcPathForInvalidChars(object sender, EventArgs e)
        {
            // Check if the text box contains any invalid character for paths
            if (txtBoxCustomMlcPath.Text.IndexOfAny(Path.GetInvalidPathChars()) != -1)
                errProviderMlcFolder.SetError(txtBoxCustomMlcPath, "The path contains some invalid characters, thus won't be saved.");
            else
                errProviderMlcFolder.SetError(txtBoxCustomMlcPath, "");
        }
        
        private void CheckIfCemuDownloadUrlIsValid(object sender, EventArgs e)
        {
            if (!Uri.IsWellFormedUriString("http://" + txtBoxBaseUrl.Text + lblSampleVersion.Text + txtBoxUrlSuffix.Text, UriKind.Absolute))
                lblCemuDownloadUrlInvalid.Visible = true;
            else
                lblCemuDownloadUrlInvalid.Visible = false;
        }

        private void OpenHelpForm(object sender, EventArgs e)
        {
            new HelpForm(this).Show();
        }
        
        private void DiscardAndClose(object sender, EventArgs e)
        {
            Close();
        }
    }
}
