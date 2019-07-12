using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using CemuUpdateTool.Utils;

namespace CemuUpdateTool.Forms
{
    /*
     *  OptionsForm
     *  Allows the user to edit the options of the program
     */
    public partial class OptionsForm : Form
    {
        OptionsManager opts;
        bool optionsFileLocationChanged;

        // These will contain the unsaved changes made to custom files/folders if the user edits them
        Dictionary<string, bool> updatedCustomFolders,
                                 updatedCustomFiles;

        public OptionsForm(OptionsManager optsInstance)
        {
            InitializeComponent();
            opts = optsInstance;
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

        public void SetCheckboxesAccordingToOptions()   // Name is self-explanatory
        {
            // FOLDER OPTIONS
            chkBoxControllerProfiles.Checked = opts.Folders[OptionsKeys.ControllerProfiles];
            chkBoxGameProfiles.Checked = opts.Folders[OptionsKeys.GameProfiles];
            chkBoxGfxPacks.Checked = opts.Folders[OptionsKeys.GraphicPacks];
            // The program sets the saves' checkbox as indeterminate if only one of the two save folders is set to true
            if (opts.Folders[OptionsKeys.OldSavegames] == true)
                chkBoxSavegames.CheckState = CheckState.Indeterminate;
            if (opts.Folders[OptionsKeys.Savegames] == true)
            {
                if (chkBoxSavegames.CheckState == CheckState.Indeterminate)
                    chkBoxSavegames.CheckState = CheckState.Checked;
                else
                    chkBoxSavegames.CheckState = CheckState.Indeterminate;
            }
            chkBoxDLCUpds.Checked = opts.Folders[OptionsKeys.DLCUpdates];
            chkBoxShaderCaches.Checked = opts.Folders[OptionsKeys.TransferableCaches];
            RefreshCustomEntriesStats();

            // FILE OPTIONS
            // As above, the settings' checkbox is set as indeterminate if only one of the two options files is set to true
            if (opts.Files[OptionsKeys.SettingsBin] == true)
                chkBoxCemuSettings.CheckState = CheckState.Indeterminate;
            if (opts.Files[OptionsKeys.SettingsXml] == true)
            {
                if (chkBoxCemuSettings.CheckState == CheckState.Indeterminate)
                    chkBoxCemuSettings.CheckState = CheckState.Checked;
                else
                    chkBoxCemuSettings.CheckState = CheckState.Indeterminate;
            }

            // MIGRATION OPTIONS
            chkBoxDeletePrevContent.Checked = opts.Migration[OptionsKeys.DeleteDestinationFolderContents];
            chkBoxDesktopShortcut.Checked = opts.Migration[OptionsKeys.AskForDesktopShortcut];
            // Custom mlc01 folder
            chkBoxCustomMlc01Path.Checked = opts.Migration[OptionsKeys.UseCustomMlcFolderIfSupported];
            if (!string.IsNullOrWhiteSpace(opts.CustomMlcFolderPath))
                txtBoxCustomMlc01Path.Text = opts.CustomMlcFolderPath;
            UpdateCustomMlc01PathTextboxState();   // make sure that textbox state is correct in relation to the checkbox
            // Compatibility options
            chkBoxCompatOpts.Checked = opts.Migration[OptionsKeys.SetCompatibilityOptions];
            chkBoxRunAsAdmin.Checked = opts.Migration[OptionsKeys.CompatibilityRunAsAdmin];
            chkBoxNoFullscreenOptimiz.Checked = opts.Migration[OptionsKeys.CompatibilityNoFullscreenOptimizations];
            chkBoxOverrideHiDPIBehaviour.Checked = opts.Migration[OptionsKeys.CompatibilityOverrideHiDPIBehaviour];
            UpdateCompatOptionsCheckboxesState();

            // SETTINGS FILE LOCATION
            if (opts.OptionsFilePath == "")
            {
                chkBoxSettingsOnFile.Checked = false;
                UpdateOptionsFileLocationRadioButtons();    // disables file location radio buttons
            }
            else
            {
                chkBoxSettingsOnFile.Checked = true;
                if (opts.OptionsFilePath == OptionsManager.LocalFilePath)
                    radioBtnExecFolder.Checked = true;
                else if (opts.OptionsFilePath == OptionsManager.AppDataFilePath)
                    radioBtnAppDataFolder.Checked = true;
            }

            // DOWNLOAD OPTIONS
            txtBoxBaseUrl.Text = opts.Download[OptionsKeys.CemuBaseUrl].Remove(0,7);    // remove "http://" because there's a label for it
            txtBoxUrlSuffix.Text = opts.Download[OptionsKeys.CemuUrlSuffix];
        }

        private void SetOptionsAccordingToCheckboxes()
        {
            // FOLDER OPTIONS
            opts.Folders[OptionsKeys.ControllerProfiles] = chkBoxControllerProfiles.Checked;
            opts.Folders[OptionsKeys.GameProfiles] = chkBoxGameProfiles.Checked;
            opts.Folders[OptionsKeys.GraphicPacks] = chkBoxGfxPacks.Checked;
            if (chkBoxSavegames.CheckState != CheckState.Indeterminate)
            {
                opts.Folders[OptionsKeys.OldSavegames] = chkBoxSavegames.Checked;
                opts.Folders[OptionsKeys.Savegames] = chkBoxSavegames.Checked;
            }
            opts.Folders[OptionsKeys.DLCUpdates] = chkBoxDLCUpds.Checked;
            opts.Folders[OptionsKeys.TransferableCaches] = chkBoxShaderCaches.Checked;

            // FILE OPTIONS
            if (chkBoxCemuSettings.CheckState != CheckState.Indeterminate)
            {
                opts.Files[OptionsKeys.SettingsBin] = chkBoxCemuSettings.Checked;
                opts.Files[OptionsKeys.SettingsXml] = chkBoxCemuSettings.Checked;
            }

            // MIGRATION OPTIONS
            opts.Migration[OptionsKeys.DeleteDestinationFolderContents] = chkBoxDeletePrevContent.Checked;
            opts.Migration[OptionsKeys.AskForDesktopShortcut] = chkBoxDesktopShortcut.Checked;

            // Custom mlc01 path
            opts.Migration[OptionsKeys.UseCustomMlcFolderIfSupported] = chkBoxCustomMlc01Path.Checked;
            if (!string.IsNullOrWhiteSpace(txtBoxCustomMlc01Path.Text) && errProviderMlcFolder.GetError(txtBoxCustomMlc01Path) == "")
                opts.CustomMlcFolderPath = txtBoxCustomMlc01Path.Text;
            else
                opts.CustomMlcFolderPath = "";

            // Compatibility options
            opts.Migration[OptionsKeys.SetCompatibilityOptions] = chkBoxCompatOpts.Checked;
            opts.Migration[OptionsKeys.CompatibilityRunAsAdmin] = chkBoxRunAsAdmin.Checked;
            opts.Migration[OptionsKeys.CompatibilityNoFullscreenOptimizations] = chkBoxNoFullscreenOptimiz.Checked;
            opts.Migration[OptionsKeys.CompatibilityOverrideHiDPIBehaviour] = chkBoxOverrideHiDPIBehaviour.Checked;

            // SETTINGS FILE LOCATION
            try
            {
                if (chkBoxSettingsOnFile.Checked)
                {
                    if (optionsFileLocationChanged)
                    {
                        opts.DeleteOptionsFile();        // delete current settings file

                        // Apply requested setting
                        if (radioBtnExecFolder.Checked)
                            opts.OptionsFilePath = OptionsManager.LocalFilePath;
                        else if (radioBtnAppDataFolder.Checked)
                            opts.OptionsFilePath = OptionsManager.AppDataFilePath;

                        // If another settings file has been found in the selected directory, ask if the user wants to load it
                        if (FileUtils.FileExists(opts.OptionsFilePath))
                        {
                            DialogResult choice = MessageBox.Show("Another options file has been found in the folder you selected. Do you want to load it?",
                                                                  "Settings file found", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                            if (choice == DialogResult.Yes)
                            {
                                try
                                {
                                    opts.ReadOptionsFromFile();
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
                    opts.DeleteOptionsFile();
                    opts.OptionsFilePath = "";
                }
            }
            catch (Exception exc)       // handles exceptions in DeleteOptionsFile()
            {
                MessageBox.Show($"Unexpected error when deleting options file: {exc.Message}", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // DOWNLOAD OPTIONS
            if (!lblUriError.Visible)
            {
                opts.Download[OptionsKeys.CemuBaseUrl] = "http://" + txtBoxBaseUrl.Text;
                opts.Download[OptionsKeys.CemuUrlSuffix] = txtBoxUrlSuffix.Text;
            }
        }

        private void RefreshCustomEntriesStats()
        {
            lblCustomFoldersCnt.Text = opts.CustomFolders().Count().ToString();
            lblCustomFilesCnt.Text = opts.CustomFiles().Count().ToString();
        }

        private void DeleteSettingsFile(object sender, EventArgs e)
        {
            string msgBoxMessage = "";
            try
            {
                bool optionsFileFound = opts.DeleteOptionsFile();

                // Build message telling if options file has been found and deleted or not
                msgBoxMessage += optionsFileFound ? $"Deleted options file in {opts.OptionsFilePath}.\r\n\r\n" : "No options file found.\r\n\r\n";
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
                opts.SetDefaultOptions();
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
                    opts.WriteOptionsToFile();
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
                List<string> oldCustomFolders = opts.CustomFolders().ToList();
                foreach (string folder in oldCustomFolders)
                    opts.Folders.Remove(folder);

                // ... and add the updated ones
                foreach (var folder in updatedCustomFolders)
                    opts.Folders.Add(folder.Key, folder.Value);
            }

            if (updatedCustomFiles != null)
            {
                // Delete the old custom files options from the dictionary
                List<string> oldCustomFiles = opts.CustomFiles().ToList();
                foreach (string file in oldCustomFiles)
                    opts.Files.Remove(file);

                // ... and add the updated ones
                foreach (var file in updatedCustomFiles)
                    opts.Files.Add(file.Key, file.Value);
            }
        }

        private void DiscardAndClose(object sender, EventArgs e)
        {
            Close();
        }

        /*
         *  Triggered when the user changes one of the options file location radio buttons.
         *  Checks if the selected path is different than the one set in OptionsManager and if so,
         *  set the corresponding flag.
         */
        private void CheckIfOptionsFileLocationHasChanged(object sender, EventArgs e)
        {
            // Evaluate whether settings file location has been changed looking at its current value
            if (opts.OptionsFilePath == OptionsManager.LocalFilePath)
            {
                if (!radioBtnExecFolder.Checked)
                    optionsFileLocationChanged = true;
                else
                    optionsFileLocationChanged = false;
            }
            else if (opts.OptionsFilePath == OptionsManager.AppDataFilePath)
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
                if (opts.OptionsFilePath == "")
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
                foreach (string folder in opts.CustomFolders())
                    updatedCustomFolders.Add(folder, opts.Folders[folder]);
            }

            new DictionaryEditingForm(updatedCustomFolders, opts.DefaultFolders()).ShowDialog();
            lblCustomFoldersCnt.Text = updatedCustomFolders.Count.ToString();     // update custom folders counter
        }

        private void OpenManageFilesDialog(object sender, EventArgs e)
        {
            // Initialize the updated dictionary if it's never been edited
            if (updatedCustomFiles == null)
            {
                updatedCustomFiles = new Dictionary<string, bool>();
                foreach (string file in opts.CustomFiles())
                    updatedCustomFiles.Add(file, opts.Files[file]);
            }

            new DictionaryEditingForm(updatedCustomFiles, opts.DefaultFiles()).ShowDialog();
            lblCustomFilesCnt.Text = updatedCustomFiles.Count.ToString();         // update custom files counter
        }

        private void OpenHelpForm(object sender, EventArgs e)
        {
            new HelpForm(this).Show();
        }
    }
}
