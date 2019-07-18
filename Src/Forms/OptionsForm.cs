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
     *  OptionsForm
     *  Allows the user to edit the options of the program
     */
    public partial class OptionsForm : Form
    {
        bool optionsFileLocationChanged;

        // These will contain the unsaved changes made to custom files/folders if the user edits them
        Dictionary<string, bool> updatedCustomFolders,
                                 updatedCustomFiles;

        public OptionsForm()
        {
            InitializeComponent();
            SetCheckboxesAccordingToOptions();

            // Draw the icon in the help button according to its size
            var iconBitmap = new Bitmap(btnHelp.Width-6, btnHelp.Height-6);
            using (Graphics g = Graphics.FromImage(iconBitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(SystemIcons.Question.ToBitmap(), new Rectangle(Point.Empty, iconBitmap.Size));
            }
            btnHelp.Image = iconBitmap;

            // Remove default (and useless) menu strips
            txtBoxCustomMlc01Path.ContextMenuStrip = new ContextMenuStrip();
            txtBoxUrlSuffix.ContextMenuStrip = new ContextMenuStrip();
            txtBoxBaseUrl.ContextMenuStrip = new ContextMenuStrip();
        }

        public void SetCheckboxesAccordingToOptions()
        {
            // FOLDER OPTIONS
            chkBoxControllerProfiles.Checked = Options.FoldersToMigrate.IsEnabled(FolderOption.ControllerProfiles);
            chkBoxGameProfiles.Checked = Options.FoldersToMigrate.IsEnabled(FolderOption.GameProfiles);
            chkBoxGfxPacks.Checked = Options.FoldersToMigrate.IsEnabled(FolderOption.GraphicPacks);
            // The program sets the saves' checkbox as indeterminate if only one of the two save folders is set to true
            if (Options.FoldersToMigrate.IsEnabled(FolderOption.OldSavegames))
                chkBoxSavegames.CheckState = CheckState.Indeterminate;
            if (Options.FoldersToMigrate.IsEnabled(FolderOption.Savegames))
            {
                if (chkBoxSavegames.CheckState == CheckState.Indeterminate)
                    chkBoxSavegames.CheckState = CheckState.Checked;
                else
                    chkBoxSavegames.CheckState = CheckState.Indeterminate;
            }
            chkBoxDLCUpds.Checked = Options.FoldersToMigrate.IsEnabled(FolderOption.DLCUpdates);
            chkBoxShaderCaches.Checked = Options.FoldersToMigrate.IsEnabled(FolderOption.TransferableCaches);
            RefreshCustomEntriesStats();

            // FILE OPTIONS
            // As above, the settings' checkbox is set as indeterminate if only one of the two options files is set to true
            if (Options.FilesToMigrate.IsEnabled(FileOption.SettingsBin))
                chkBoxCemuSettings.CheckState = CheckState.Indeterminate;
            if (Options.FilesToMigrate.IsEnabled(FileOption.SettingsXml))
            {
                if (chkBoxCemuSettings.CheckState == CheckState.Indeterminate)
                    chkBoxCemuSettings.CheckState = CheckState.Checked;
                else
                    chkBoxCemuSettings.CheckState = CheckState.Indeterminate;
            }

            // MIGRATION OPTIONS
            chkBoxDeletePrevContent.Checked = Options.Migration[OptionKey.DeleteDestinationFolderContents];
            chkBoxDesktopShortcut.Checked = Options.Migration[OptionKey.AskForDesktopShortcut];
            // Custom mlc01 folder
            chkBoxCustomMlc01Path.Checked = Options.Migration[OptionKey.UseCustomMlcFolderIfSupported];
            if (!string.IsNullOrWhiteSpace(Options.CustomMlcFolderPath))
                txtBoxCustomMlc01Path.Text = Options.CustomMlcFolderPath;
            UpdateCustomMlc01PathTextboxState();   // make sure that textbox state is correct in relation to the checkbox
            // Compatibility options
            chkBoxCompatOpts.Checked = Options.Migration[OptionKey.SetCompatibilityOptions];
            chkBoxRunAsAdmin.Checked = Options.Migration[OptionKey.CompatibilityRunAsAdmin];
            chkBoxNoFullscreenOptimiz.Checked = Options.Migration[OptionKey.CompatibilityNoFullscreenOptimizations];
            chkBoxOverrideHiDPIBehaviour.Checked = Options.Migration[OptionKey.CompatibilityOverrideHiDPIBehaviour];
            UpdateCompatOptionsCheckboxesState();

            // SETTINGS FILE LOCATION
            if (Options.CurrentOptionsFilePath == "")
            {
                chkBoxSettingsOnFile.Checked = false;
                UpdateOptionsFileLocationRadioButtons();    // disables file location radio buttons
            }
            else
            {
                chkBoxSettingsOnFile.Checked = true;
                if (Options.CurrentOptionsFilePath == Options.LocalOptionsFilePath)
                    radioBtnExecFolder.Checked = true;
                else if (Options.CurrentOptionsFilePath == Options.AppDataOptionsFilePath)
                    radioBtnAppDataFolder.Checked = true;
            }

            // DOWNLOAD OPTIONS
            txtBoxBaseUrl.Text = Options.Download[OptionKey.CemuBaseUrl].Remove(0,7);    // remove "http://" because there's a label for it
            txtBoxUrlSuffix.Text = Options.Download[OptionKey.CemuUrlSuffix];
        }

        private void SetOptionsAccordingToCheckboxes()
        {
            // FOLDER OPTIONS
            Options.FoldersToMigrate[FolderOption.ControllerProfiles] = chkBoxControllerProfiles.Checked;
            Options.FoldersToMigrate[FolderOption.GameProfiles] = chkBoxGameProfiles.Checked;
            Options.FoldersToMigrate[FolderOption.GraphicPacks] = chkBoxGfxPacks.Checked;
            if (chkBoxSavegames.CheckState != CheckState.Indeterminate)
            {
                Options.FoldersToMigrate[FolderOption.OldSavegames] = chkBoxSavegames.Checked;
                Options.FoldersToMigrate[FolderOption.Savegames] = chkBoxSavegames.Checked;
            }
            Options.FoldersToMigrate[FolderOption.DLCUpdates] = chkBoxDLCUpds.Checked;
            Options.FoldersToMigrate[FolderOption.TransferableCaches] = chkBoxShaderCaches.Checked;

            // FILE OPTIONS
            if (chkBoxCemuSettings.CheckState != CheckState.Indeterminate)
            {
                Options.FilesToMigrate[FileOption.SettingsBin] = chkBoxCemuSettings.Checked;
                Options.FilesToMigrate[FileOption.SettingsXml] = chkBoxCemuSettings.Checked;
            }

            // MIGRATION OPTIONS
            Options.Migration[OptionKey.DeleteDestinationFolderContents] = chkBoxDeletePrevContent.Checked;
            Options.Migration[OptionKey.AskForDesktopShortcut] = chkBoxDesktopShortcut.Checked;

            // Custom mlc01 path
            Options.Migration[OptionKey.UseCustomMlcFolderIfSupported] = chkBoxCustomMlc01Path.Checked;
            if (!string.IsNullOrWhiteSpace(txtBoxCustomMlc01Path.Text) && errProviderMlcFolder.GetError(txtBoxCustomMlc01Path) == "")
                Options.CustomMlcFolderPath = txtBoxCustomMlc01Path.Text;
            else
                Options.CustomMlcFolderPath = "";

            // Compatibility options
            Options.Migration[OptionKey.SetCompatibilityOptions] = chkBoxCompatOpts.Checked;
            Options.Migration[OptionKey.CompatibilityRunAsAdmin] = chkBoxRunAsAdmin.Checked;
            Options.Migration[OptionKey.CompatibilityNoFullscreenOptimizations] = chkBoxNoFullscreenOptimiz.Checked;
            Options.Migration[OptionKey.CompatibilityOverrideHiDPIBehaviour] = chkBoxOverrideHiDPIBehaviour.Checked;

            // SETTINGS FILE LOCATION
            try
            {
                if (chkBoxSettingsOnFile.Checked)
                {
                    if (optionsFileLocationChanged)
                    {
                        Options.DeleteOptionsFile();

                        // Apply requested setting
                        if (radioBtnExecFolder.Checked)
                            Options.CurrentOptionsFilePath = Options.LocalOptionsFilePath;
                        else if (radioBtnAppDataFolder.Checked)
                            Options.CurrentOptionsFilePath = Options.AppDataOptionsFilePath;

                        // If another settings file has been found in the selected directory, ask if the user wants to load it
                        if (File.Exists(Options.CurrentOptionsFilePath))
                        {
                            DialogResult choice = MessageBox.Show("Another options file has been found in the folder you selected. Do you want to load it?",
                                                                  "Settings file found", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                            if (choice == DialogResult.Yes)
                            {
                                try
                                {
                                    Options.LoadFromCurrentlySelectedFile();
                                }
                                catch
                                {
                                    MessageBox.Show("An error occurred when parsing options file. Previous settings will be kept.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Options.DeleteOptionsFile();
                    Options.CurrentOptionsFilePath = "";
                }
            }
            catch (Exception exc)       // handles exceptions in DeleteOptionsFile()
            {
                MessageBox.Show($"Unexpected error when deleting options file: {exc.Message}", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // DOWNLOAD OPTIONS
            if (!lblUriError.Visible)
            {
                Options.Download[OptionKey.CemuBaseUrl] = "http://" + txtBoxBaseUrl.Text;
                Options.Download[OptionKey.CemuUrlSuffix] = txtBoxUrlSuffix.Text;
            }
        }

        private void RefreshCustomEntriesStats()
        {
            lblCustomFoldersCnt.Text = Options.AllCustomFoldersToMigrate().Count().ToString();
            lblCustomFilesCnt.Text = Options.AllCustomFilesToMigrate().Count().ToString();
        }

        private void DeleteSettingsFile(object sender, EventArgs e)
        {
            string msgBoxMessage = "";
            try
            {
                bool optionsFileFound = Options.DeleteOptionsFile();

                // Build message telling if options file has been found and deleted or not
                msgBoxMessage += optionsFileFound ? $"Deleted options file in {Options.CurrentOptionsFilePath}.\r\n\r\n" : "No options file found.\r\n\r\n";
            }
            catch (Exception exc)
            {
                msgBoxMessage += $"An unexpected error occurred when deleting options file: {exc.Message}\r\n\r\n";
            }
            msgBoxMessage += $"Please take note that unless you uncheck \"{chkBoxSettingsOnFile.Text}\", options file will be recreated when you click on \"{btnSaveOpts.Text}\".";

            MessageBox.Show(msgBoxMessage, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnDeleteSettingsFile.Enabled = false;
        }

        private void RestoreDefaultOptions(object sender, EventArgs e)
        {
            DialogResult choice = MessageBox.Show("Are you sure you want to restore the default settings?\r\nThis cannot be undone.",
                                                  "Confirmation requested", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

            try
            {
                if (chkBoxSettingsOnFile.Checked)
                    Options.WriteOptionsToCurrentlySelectedFile();
                else
                    MessageBox.Show("Please take note that if you don't store options in a file, they'll be lost as soon as you exit the application.",
                                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Unexpected error when saving options on file: {exc.Message} Changes won't be preserved after closing the program.",
                                 "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Close();
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

        private void DiscardAndClose(object sender, EventArgs e)
        {
            Close();
        }

        /*
         *  Triggered when the user changes one of the options file location radio buttons.
         *  Checks if the selected path is different than the one set in Options and if so,
         *  set the corresponding flag.
         */
        private void CheckIfOptionsFileLocationHasChanged(object sender, EventArgs e)
        {
            // Evaluate whether settings file location has been changed looking at its current value
            if (Options.CurrentOptionsFilePath == Options.LocalOptionsFilePath)
            {
                if (!radioBtnExecFolder.Checked)
                    optionsFileLocationChanged = true;
                else
                    optionsFileLocationChanged = false;
            }
            else if (Options.CurrentOptionsFilePath == Options.AppDataOptionsFilePath)
            {
                if (!radioBtnAppDataFolder.Checked)
                    optionsFileLocationChanged = true;
                else
                    optionsFileLocationChanged = false;
            }
        }

        /*
         *  Triggered when the user selects/deselects the "Save settings on file" checkbox.
         *  Enables/disables options file location radio buttons and "Delete settings file" button.
         */
        private void UpdateOptionsFileLocationRadioButtons(object sender = null, EventArgs e = null)
        {
            if (!chkBoxSettingsOnFile.Checked)
            {
                lblFileLocation.Enabled = false;
                radioBtnExecFolder.Enabled = false;
                radioBtnAppDataFolder.Enabled = false;
                btnDeleteSettingsFile.Enabled = false;
            }
            else
            {
                lblFileLocation.Enabled = true;
                radioBtnExecFolder.Enabled = true;
                radioBtnAppDataFolder.Enabled = true;
                btnDeleteSettingsFile.Enabled = true;

                // Obviously, if current options file path is not set, tell the program to change it before writing the file on disk
                if (Options.CurrentOptionsFilePath == "")
                    optionsFileLocationChanged = true;
            }
        }

        private void UpdateCustomMlc01PathTextboxState(object sender = null, EventArgs e = null)
        {
            if (chkBoxCustomMlc01Path.Checked)
                txtBoxCustomMlc01Path.Enabled = true;
            else
                txtBoxCustomMlc01Path.Enabled = false;
        }

        private void UpdateCompatOptionsCheckboxesState(object sender = null, EventArgs e = null)
        {
            if (chkBoxCompatOpts.Checked)
            {
                chkBoxRunAsAdmin.Enabled = true;
                chkBoxNoFullscreenOptimiz.Enabled = true;
                chkBoxOverrideHiDPIBehaviour.Enabled = true;
            }
            else
            {
                chkBoxRunAsAdmin.Enabled = false;
                chkBoxNoFullscreenOptimiz.Enabled = false;
                chkBoxOverrideHiDPIBehaviour.Enabled = false;
            }
        }

        private void CheckCustomMlc01PathForInvalidChars(object sender, EventArgs e)
        {
            // Check if the textbox contains any invalid character for paths
            if (txtBoxCustomMlc01Path.Text.IndexOfAny(System.IO.Path.GetInvalidPathChars()) != -1)
                errProviderMlcFolder.SetError(txtBoxCustomMlc01Path, "The path contains some invalid characters, thus won't be saved.");
            else
                errProviderMlcFolder.SetError(txtBoxCustomMlc01Path, "");
        }

        /*
         *  Checks if Cemu download URL is valid
         */
        private void CheckIfDownloadUrlIsValid(object sender, EventArgs e)
        {
            if (!Uri.IsWellFormedUriString("http://" + txtBoxBaseUrl.Text + lblSampleVersion.Text + txtBoxUrlSuffix.Text, UriKind.Absolute))
                lblUriError.Visible = true;
            else
                lblUriError.Visible = false;
        }

        /*
         *  Open custom folders/files management dialog
         */
        private void OpenManageFoldersDialog(object sender, EventArgs e)
        {
            // Initialize the updated dictionary if it's never been edited
            if (updatedCustomFolders == null)
            {
                updatedCustomFolders = new Dictionary<string, bool>();
                foreach (string folder in Options.AllCustomFoldersToMigrate())
                    updatedCustomFolders.Add(folder, Options.FoldersToMigrate[folder]);
            }

            new DictionaryEditingForm(updatedCustomFolders, Options.AllDefaultFoldersToMigrate()).ShowDialog();
            lblCustomFoldersCnt.Text = updatedCustomFolders.Count.ToString();     // update custom folders counter
        }

        private void OpenManageFilesDialog(object sender, EventArgs e)
        {
            // Initialize the updated dictionary if it's never been edited
            if (updatedCustomFiles == null)
            {
                updatedCustomFiles = new Dictionary<string, bool>();
                foreach (string file in Options.AllCustomFilesToMigrate())
                    updatedCustomFiles.Add(file, Options.FilesToMigrate[file]);
            }

            new DictionaryEditingForm(updatedCustomFiles, Options.AllDefaultFilesToMigrate()).ShowDialog();
            lblCustomFilesCnt.Text = updatedCustomFiles.Count.ToString();         // update custom files counter
        }

        private void OpenHelpForm(object sender, EventArgs e)
        {
            new HelpForm(this).Show();
        }
    }
}
