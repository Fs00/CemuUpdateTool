﻿using System;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    public partial class OptionsForm : Form
    {
        OptionsManager handler;
        bool optionsFileLocationChanged;

        public OptionsForm(OptionsManager classInstance)
        {
            InitializeComponent();
            handler = classInstance;
            SetCheckboxesAccordingToOptions();
        }

        public void SetCheckboxesAccordingToOptions()   // Name is self-explanatory
        {
            // FOLDER OPTIONS
            chkBoxControllerProfiles.Checked = handler.folderOptions["controllerProfiles"];
            chkBoxGameProfiles.Checked = handler.folderOptions["gameProfiles"];
            chkBoxGfxPacks.Checked = handler.folderOptions["graphicPacks"];
            // The program sets the saves' checkbox as indeterminate if there's only one of the two save folders (and it's set to true)
            if (handler.folderOptions[@"mlc01\emulatorSave"] == true)
                chkBoxSavegames.CheckState = CheckState.Indeterminate;
            if (handler.folderOptions[@"mlc01\usr\save"] == true)
            {
                if (chkBoxSavegames.CheckState == CheckState.Indeterminate)
                    chkBoxSavegames.CheckState = CheckState.Checked;
                else
                    chkBoxSavegames.CheckState = CheckState.Indeterminate;
            }
            chkBoxDLCUpds.Checked = handler.folderOptions[@"mlc01\usr\title"];
            chkBoxShaderCaches.Checked = handler.folderOptions[@"shaderCache\transferable"];

            // MIGRATION OPTIONS
            chkBoxCemuSettings.Checked = handler.migrationOptions["copyCemuSettingsFile"];
            chkBoxDeletePrevContent.Checked = handler.migrationOptions["deleteDestFolderContents"];
            chkBoxDesktopShortcut.Checked = handler.migrationOptions["askForDesktopShortcut"];
            // Custom mlc01 folder
            chkBoxCustomMlc01Path.Checked = handler.migrationOptions["dontCopyMlcFolderFor1.10+"];
            if (!string.IsNullOrWhiteSpace(handler.mlcFolderExternalPath))
                txtBoxCustomMlc01Path.Text = handler.mlcFolderExternalPath;
            UpdateCustomMlc01PathTextboxState();   // make sure that textbox state is correct in relation to the checkbox
            // Compatibility options
            chkBoxCompatOpts.Checked = handler.migrationOptions["setCompatibilityOptions"];
            chkBoxRunAsAdmin.Checked = handler.migrationOptions["compatOpts_runAsAdmin"];
            chkBoxNoFullscreenOptimiz.Checked = handler.migrationOptions["compatOpts_noFullscreenOptimizations"];
            chkBoxOverrideHiDPIBehaviour.Checked = handler.migrationOptions["compatOpts_overrideHiDPIBehaviour"];
            UpdateCompatOptsCheckboxesState();

            // SETTINGS FILE LOCATION
            if (handler.optionsFilePath == "")
            {
                chkBoxSettingsOnFile.Checked = false;
                UpdateOptionsFileLocationRadioButtons();    // disables file location radio buttons
            }
            else
            {
                chkBoxSettingsOnFile.Checked = true;
                if (handler.optionsFilePath == handler.LOCAL_FILEPATH)
                    radioBtnExecFolder.Checked = true;
                else if (handler.optionsFilePath == handler.APPDATA_FILEPATH)
                    radioBtnAppDataFolder.Checked = true;
            }

            // DOWNLOAD OPTIONS
            txtBoxBaseUrl.Text = handler.downloadOptions["cemuBaseUrl"];
            txtBoxUrlSuffix.Text = handler.downloadOptions["cemuUrlSuffix"];
        }

        private void SetOptionsAccordingToCheckboxes()
        {
            // FOLDER OPTIONS
            handler.folderOptions["controllerProfiles"] = chkBoxControllerProfiles.Checked;
            handler.folderOptions["gameProfiles"] = chkBoxGameProfiles.Checked;
            handler.folderOptions["graphicPacks"] = chkBoxGfxPacks.Checked;
            if (chkBoxSavegames.CheckState != CheckState.Indeterminate)
            {
                handler.folderOptions[@"mlc01\emulatorSave"] = chkBoxSavegames.Checked;
                handler.folderOptions[@"mlc01\usr\save"] = chkBoxSavegames.Checked;
            }
            handler.folderOptions[@"mlc01\usr\title"] = chkBoxDLCUpds.Checked;
            handler.folderOptions[@"shaderCache\transferable"] = chkBoxShaderCaches.Checked;

            // MIGRATION OPTIONS
            handler.migrationOptions["copyCemuSettingsFile"] = chkBoxCemuSettings.Checked;
            handler.migrationOptions["deleteDestFolderContents"] = chkBoxDeletePrevContent.Checked;
            handler.migrationOptions["askForDesktopShortcut"] = chkBoxDesktopShortcut.Checked;
            // Custom mlc01 path
            handler.migrationOptions["dontCopyMlcFolderFor1.10+"] = chkBoxCustomMlc01Path.Checked;
            if (!string.IsNullOrWhiteSpace(txtBoxCustomMlc01Path.Text) && errProviderMlcFolder.GetError(txtBoxCustomMlc01Path) == "")
                handler.mlcFolderExternalPath = txtBoxCustomMlc01Path.Text;
            else
                handler.mlcFolderExternalPath = "";
            // Compatibility options
            handler.migrationOptions["setCompatibilityOptions"] = chkBoxCompatOpts.Checked;
            handler.migrationOptions["compatOpts_runAsAdmin"] = chkBoxRunAsAdmin.Checked;
            handler.migrationOptions["compatOpts_noFullscreenOptimizations"] = chkBoxNoFullscreenOptimiz.Checked;
            handler.migrationOptions["compatOpts_overrideHiDPIBehaviour"] = chkBoxOverrideHiDPIBehaviour.Checked;

            // SETTINGS FILE LOCATION
            if (chkBoxSettingsOnFile.Checked)
            {
                if (optionsFileLocationChanged)
                {
                    handler.DeleteOptionsFile();
                    if (radioBtnExecFolder.Checked)
                        handler.optionsFilePath = handler.LOCAL_FILEPATH;
                    else if (radioBtnAppDataFolder.Checked)
                        handler.optionsFilePath = handler.APPDATA_FILEPATH;
                }
            }
            else
            {
                handler.DeleteOptionsFile();
                handler.optionsFilePath = "";
            }

            // DOWNLOAD OPTIONS
            // TODO: errorProvider per controllare validità Uri
            handler.downloadOptions["cemuBaseUrl"] = txtBoxBaseUrl.Text;
            handler.downloadOptions["cemuUrlSuffix"] = txtBoxUrlSuffix.Text;
        }

        private void DeleteSettingsFile(object sender, EventArgs e)
        {
            handler.DeleteOptionsFile();
            btnDeleteSettingsFile.Enabled = false;

            MessageBox.Show("Please take note that unless you uncheck \"Save options in a file\", options file will be recreated when you click on \"Save options\".",
                            "", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (handler.optionsFilePath == handler.LOCAL_FILEPATH)
            {
                if (!radioBtnExecFolder.Checked)
                    optionsFileLocationChanged = true;
                else
                    optionsFileLocationChanged = false;
            }
            else if (handler.optionsFilePath == handler.APPDATA_FILEPATH)
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
                if (handler.optionsFilePath == "")
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

            if (chkBoxSettingsOnFile.Checked)
                handler.WriteOptionsToFile();
            else
                MessageBox.Show("Please take note that if you don't store options in a file, they'll be lost as soon as you exit the application.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            Close();
        }

        private void DiscardAndClose(object sender, EventArgs e)
        {
            Close();
        }
    }
}
