using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    public partial class OptionsForm : Form
    {
        OptionsManager handler;
        bool optionsFileLocationChanged;
        bool? newCustomFoldersEnabledState = null;  // null if custom folders must remain untouched, otherwise they're enabled/disabled according to the value

        public OptionsForm(OptionsManager classInstance)
        {
            InitializeComponent();
            handler = classInstance;
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
            chkBoxControllerProfiles.Checked = handler.FolderOptions["controllerProfiles"];
            chkBoxGameProfiles.Checked = handler.FolderOptions["gameProfiles"];
            chkBoxGfxPacks.Checked = handler.FolderOptions["graphicPacks"];
            // The program sets the saves' checkbox as indeterminate if only one of the two save folders is set to true
            if (handler.FolderOptions[@"mlc01\emulatorSave"] == true)
                chkBoxSavegames.CheckState = CheckState.Indeterminate;
            if (handler.FolderOptions[@"mlc01\usr\save"] == true)
            {
                if (chkBoxSavegames.CheckState == CheckState.Indeterminate)
                    chkBoxSavegames.CheckState = CheckState.Checked;
                else
                    chkBoxSavegames.CheckState = CheckState.Indeterminate;
            }
            chkBoxDLCUpds.Checked = handler.FolderOptions[@"mlc01\usr\title"];
            chkBoxShaderCaches.Checked = handler.FolderOptions[@"shaderCache\transferable"];
            RefreshCustomFolderStats();

            // FILE OPTIONS
            // As above, the settings' checkbox is set as indeterminate if only one of the two options files is set to true
            if (handler.FileOptions["settings.bin"] == true)
                chkBoxCemuSettings.CheckState = CheckState.Indeterminate;
            if (handler.FileOptions["settings.xml"] == true)
            {
                if (chkBoxCemuSettings.CheckState == CheckState.Indeterminate)
                    chkBoxCemuSettings.CheckState = CheckState.Checked;
                else
                    chkBoxCemuSettings.CheckState = CheckState.Indeterminate;
            }

            // MIGRATION OPTIONS
            chkBoxDeletePrevContent.Checked = handler.MigrationOptions["deleteDestFolderContents"];
            chkBoxDesktopShortcut.Checked = handler.MigrationOptions["askForDesktopShortcut"];
            // Custom mlc01 folder
            chkBoxCustomMlc01Path.Checked = handler.MigrationOptions["dontCopyMlcFolderFor1.10+"];
            if (!string.IsNullOrWhiteSpace(handler.MlcFolderExternalPath))
                txtBoxCustomMlc01Path.Text = handler.MlcFolderExternalPath;
            UpdateCustomMlc01PathTextboxState();   // make sure that textbox state is correct in relation to the checkbox
            // Compatibility options
            chkBoxCompatOpts.Checked = handler.MigrationOptions["setCompatibilityOptions"];
            chkBoxRunAsAdmin.Checked = handler.MigrationOptions["compatOpts_runAsAdmin"];
            chkBoxNoFullscreenOptimiz.Checked = handler.MigrationOptions["compatOpts_noFullscreenOptimizations"];
            chkBoxOverrideHiDPIBehaviour.Checked = handler.MigrationOptions["compatOpts_overrideHiDPIBehaviour"];
            UpdateCompatOptsCheckboxesState();

            // SETTINGS FILE LOCATION
            if (handler.OptionsFilePath == "")
            {
                chkBoxSettingsOnFile.Checked = false;
                UpdateOptionsFileLocationRadioButtons();    // disables file location radio buttons
            }
            else
            {
                chkBoxSettingsOnFile.Checked = true;
                if (handler.OptionsFilePath == OptionsManager.LocalFilePath)
                    radioBtnExecFolder.Checked = true;
                else if (handler.OptionsFilePath == OptionsManager.AppDataFilePath)
                    radioBtnAppDataFolder.Checked = true;
            }

            // DOWNLOAD OPTIONS
            txtBoxBaseUrl.Text = handler.DownloadOptions["cemuBaseUrl"].Remove(0,7);    // remove "http://" because there's a label for it
            txtBoxUrlSuffix.Text = handler.DownloadOptions["cemuUrlSuffix"];
        }

        private void SetOptionsAccordingToCheckboxes()
        {
            // FOLDER OPTIONS
            handler.FolderOptions["controllerProfiles"] = chkBoxControllerProfiles.Checked;
            handler.FolderOptions["gameProfiles"] = chkBoxGameProfiles.Checked;
            handler.FolderOptions["graphicPacks"] = chkBoxGfxPacks.Checked;
            if (chkBoxSavegames.CheckState != CheckState.Indeterminate)
            {
                handler.FolderOptions[@"mlc01\emulatorSave"] = chkBoxSavegames.Checked;
                handler.FolderOptions[@"mlc01\usr\save"] = chkBoxSavegames.Checked;
            }
            handler.FolderOptions[@"mlc01\usr\title"] = chkBoxDLCUpds.Checked;
            handler.FolderOptions[@"shaderCache\transferable"] = chkBoxShaderCaches.Checked;

            // FILE OPTIONS
            if (chkBoxCemuSettings.CheckState != CheckState.Indeterminate)
            {
                handler.FileOptions["settings.bin"] = chkBoxCemuSettings.Checked;
                handler.FileOptions["settings.xml"] = chkBoxCemuSettings.Checked;
            }

            // MIGRATION OPTIONS
            handler.MigrationOptions["deleteDestFolderContents"] = chkBoxDeletePrevContent.Checked;
            handler.MigrationOptions["askForDesktopShortcut"] = chkBoxDesktopShortcut.Checked;
            // Custom mlc01 path
            handler.MigrationOptions["dontCopyMlcFolderFor1.10+"] = chkBoxCustomMlc01Path.Checked;
            if (!string.IsNullOrWhiteSpace(txtBoxCustomMlc01Path.Text) && errProviderMlcFolder.GetError(txtBoxCustomMlc01Path) == "")
                handler.MlcFolderExternalPath = txtBoxCustomMlc01Path.Text;
            else
                handler.MlcFolderExternalPath = "";
            // Compatibility options
            handler.MigrationOptions["setCompatibilityOptions"] = chkBoxCompatOpts.Checked;
            handler.MigrationOptions["compatOpts_runAsAdmin"] = chkBoxRunAsAdmin.Checked;
            handler.MigrationOptions["compatOpts_noFullscreenOptimizations"] = chkBoxNoFullscreenOptimiz.Checked;
            handler.MigrationOptions["compatOpts_overrideHiDPIBehaviour"] = chkBoxOverrideHiDPIBehaviour.Checked;

            // SETTINGS FILE LOCATION
            try
            {
                if (chkBoxSettingsOnFile.Checked)
                {
                    if (optionsFileLocationChanged)
                    {
                        handler.DeleteOptionsFile();        // delete current settings file
                        if (handler.OptionsFileExists())    // if another settings file has been found in the selected directory, ask if the user wants to load it
                        {
                            DialogResult choice = MessageBox.Show("Another options file has been found in the folder you selected. Do you want to load it?",
                                                                  "Settings file found", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                            if (choice == DialogResult.Yes)
                            {
                                try
                                {
                                    handler.ReadOptionsFromFile();
                                }
                                catch
                                {
                                    handler.SetDefaultOptions();
                                }
                            }
                        }
                        else    // apply requested setting (not needed in the first case because OptionsFileExists() does it itself)
                        {
                            if (radioBtnExecFolder.Checked)
                                handler.OptionsFilePath = OptionsManager.LocalFilePath;
                            else if (radioBtnAppDataFolder.Checked)
                                handler.OptionsFilePath = OptionsManager.AppDataFilePath;
                        }
                    }
                }
                else
                {
                    handler.DeleteOptionsFile();
                    handler.OptionsFilePath = "";
                }
            }
            catch (Exception exc)       // handles exceptions in DeleteOptionsFile()
            {
                MessageBox.Show($"Unexpected error when deleting options file: {exc.Message}", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // DOWNLOAD OPTIONS
            if (!lblUriError.Visible)
            {
                handler.DownloadOptions["cemuBaseUrl"] = "http://" + txtBoxBaseUrl.Text;
                handler.DownloadOptions["cemuUrlSuffix"] = txtBoxUrlSuffix.Text;
            }
        }

        private void RefreshCustomFolderStats()
        {
            int foldersCounter = 0,
                enabledFoldersCounter = 0;

            foreach (string folder in handler.CustomFolders())
            {
                foldersCounter++;
                if (handler.FolderOptions[folder] == true)
                    enabledFoldersCounter++;
            }

            lblCustomFoldersCnt.Text = foldersCounter.ToString();
            lblEnabledCustomFoldersCnt.Text = enabledFoldersCounter.ToString();
        }

        private void EnableOrDisableCustomFolders(bool enable)
        {
            // Save in a temporary list all the custom folders
            List<string> customFolders = new List<string>();
            foreach (string folder in handler.CustomFolders())
                customFolders.Add(folder);

            // Apply changes to folderOptions
            foreach (string folder in customFolders)
                handler.FolderOptions[folder] = enable;
        }

        private void DeleteSettingsFile(object sender, EventArgs e)
        {
            string msgBoxMessage = "";
            try
            {
                bool optionsFileFound = handler.DeleteOptionsFile();

                // Build message telling if options file has been found and deleted or not
                msgBoxMessage += optionsFileFound ? $"Deleted options file in {handler.OptionsFilePath}.\r\n\r\n" : "No options file found.\r\n\r\n";
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
                handler.SetDefaultOptions();
                SetCheckboxesAccordingToOptions();
            }
        }

        /*
         *  Triggered when the user changes one of the options file location radio buttons.
         *  Checks if the selected path is different than the one set in OptionsManager and if so,
         *  set the corresponding flag.
         */
        private void CheckIfOptionsFileLocationHasChanged(object sender, EventArgs e)
        {
            // Evaluate whether settings file location has been changed looking at its current value
            if (handler.OptionsFilePath == OptionsManager.LocalFilePath)
            {
                if (!radioBtnExecFolder.Checked)
                    optionsFileLocationChanged = true;
                else
                    optionsFileLocationChanged = false;
            }
            else if (handler.OptionsFilePath == OptionsManager.AppDataFilePath)
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
                if (handler.OptionsFilePath == "")
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

        private void UpdateCompatOptsCheckboxesState(object sender = null, EventArgs e = null)
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

        private void SaveOptionsAndClose(object sender, EventArgs e)
        {
            SetOptionsAccordingToCheckboxes();
            if (newCustomFoldersEnabledState != null)
                EnableOrDisableCustomFolders(newCustomFoldersEnabledState.Value);

            try
            {
                if (chkBoxSettingsOnFile.Checked)
                    handler.WriteOptionsToFile();
                else
                    MessageBox.Show("Please take note that if you don't store options in a file, they'll be lost as soon as you exit the application.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Unexpected error when saving options on file: {exc.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Close();
        }

        private void DiscardAndClose(object sender, EventArgs e)
        {
            Close();
        }

        /*
         *  These methods will just update the "Enabled" label and notify the form to 
         *  enable/disable custom folders once the user clicks on "Save options"
         */
        private void CustomFoldersWillBeEnabled(object sender, EventArgs e)
        {
            lblEnabledCustomFoldersCnt.Text = lblCustomFoldersCnt.Text;
            newCustomFoldersEnabledState = true;
        }

        private void CustomFoldersWillBeDisabled(object sender, EventArgs e)
        {
            lblEnabledCustomFoldersCnt.Text = "0";
            newCustomFoldersEnabledState = false;
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

        private void OpenHelpForm(object sender, EventArgs e)
        {
            new HelpForm(this).Show();
        }
    }
}
