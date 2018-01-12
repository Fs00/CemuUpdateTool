using System;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    public partial class OptionsForm : Form
    {
        OptionsManager handler;
        bool optionsFileLocationChanged;

        // TODO: add validation for external mlc01 textbox
        // TODO: add "Restore defaults" button

        public OptionsForm(OptionsManager classInstance)
        {
            InitializeComponent();
            handler = classInstance;
            SetCheckboxesAccordingToOptions();
        }

        public void SetCheckboxesAccordingToOptions()   // Name is self-explanatory
        {
            // FOLDER OPTIONS
            if (handler.folderOptions.ContainsKey("controllerProfiles"))
                chkBoxControllerProfiles.Checked = handler.folderOptions["controllerProfiles"];
            if (handler.folderOptions.ContainsKey("gameProfiles"))
                chkBoxGameProfiles.Checked = handler.folderOptions["gameProfiles"];
            if (handler.folderOptions.ContainsKey("graphicPacks"))
                chkBoxGfxPacks.Checked = handler.folderOptions["graphicPacks"];

            // The program sets the saves' checkbox as indeterminate if there's only one of the two save folders (and it's set to true)
            if (handler.folderOptions.ContainsKey(@"mlc01\emulatorSave") && handler.folderOptions[@"mlc01\emulatorSave"] == true)
                chkBoxSavegames.CheckState = CheckState.Indeterminate;
            if (handler.folderOptions.ContainsKey(@"mlc01\usr\save") && handler.folderOptions[@"mlc01\usr\save"] == true)
            {
                if (chkBoxSavegames.CheckState == CheckState.Indeterminate)
                    chkBoxSavegames.CheckState = CheckState.Checked;
                else
                    chkBoxSavegames.CheckState = CheckState.Indeterminate;
            }
            if (handler.folderOptions.ContainsKey(@"mlc01\usr\title"))
                chkBoxDLCUpds.Checked = handler.folderOptions[@"mlc01\usr\title"];
            if (handler.folderOptions.ContainsKey(@"shaderCache\transferable"))
                chkBoxShaderCaches.Checked = handler.folderOptions[@"shaderCache\transferable"];

            // ADDITIONAL OPTIONS
            chkBoxCemuSettings.Checked = handler.migrationOptions["copyCemuSettingsFile"];
            chkBoxDeletePrevContent.Checked = handler.migrationOptions["deleteDestFolderContents"];
            chkBoxCustomMlc01Path.Checked = handler.migrationOptions["dontCopyMlcFolderFor1.10+"];
            if (!string.IsNullOrWhiteSpace(handler.mlcFolderExternalPath))
                txtBoxCustomMlc01Path.Text = handler.mlcFolderExternalPath;
            chkBoxCustomMlc01Path_CheckedChanged(null, null);   // make sure that textbox state is correct in relation to the checkbox

            // SETTINGS FILE LOCATION
            if (handler.optionsFilePath == "")
            {
                chkBoxSettingsOnFile.Checked = false;
                chkBoxSettingsOnFile_CheckedChanged(null, null);    // disables file location radio buttons
            }
            else
            {
                chkBoxSettingsOnFile.Checked = true;
                if (handler.optionsFilePath == handler.LOCAL_FILEPATH)
                    radioBtnExecFolder.Checked = true;
                else if (handler.optionsFilePath == handler.APPDATA_FILEPATH)
                    radioBtnAppDataFolder.Checked = true;
            }
        }

        private void SetOptionsAccordingToCheckboxes()  // Same as above
        {
            // FOLDER OPTIONS
            if (handler.folderOptions.ContainsKey("controllerProfiles"))
                handler.folderOptions["controllerProfiles"] = chkBoxControllerProfiles.Checked;
            else
                handler.folderOptions.Add("controllerProfiles", chkBoxControllerProfiles.Checked);

            if (handler.folderOptions.ContainsKey("gameProfiles"))
                handler.folderOptions["gameProfiles"] = chkBoxGameProfiles.Checked;
            else
                handler.folderOptions.Add("gameProfiles", chkBoxGameProfiles.Checked);

            if (handler.folderOptions.ContainsKey("graphicPacks"))
                handler.folderOptions["graphicPacks"] = chkBoxGfxPacks.Checked;
            else
                handler.folderOptions.Add("graphicPacks", chkBoxGfxPacks.Checked);

            if (chkBoxSavegames.CheckState != CheckState.Indeterminate)
            {
                if (handler.folderOptions.ContainsKey(@"mlc01\emulatorSave"))
                    handler.folderOptions[@"mlc01\emulatorSave"] = chkBoxSavegames.Checked;
                else
                    handler.folderOptions.Add(@"mlc01\emulatorSave", chkBoxSavegames.Checked);
                if (handler.folderOptions.ContainsKey(@"mlc01\usr\save"))
                    handler.folderOptions[@"mlc01\usr\save"] = chkBoxSavegames.Checked;
                else
                    handler.folderOptions.Add(@"mlc01\usr\save", chkBoxSavegames.Checked);
            }

            if (handler.folderOptions.ContainsKey(@"mlc01\usr\title"))
                handler.folderOptions[@"mlc01\usr\title"] = chkBoxDLCUpds.Checked;
            else
                handler.folderOptions.Add(@"mlc01\usr\title", chkBoxDLCUpds.Checked);

            if (handler.folderOptions.ContainsKey(@"shaderCache\transferable"))
                handler.folderOptions[@"shaderCache\transferable"] = chkBoxShaderCaches.Checked;
            else
                handler.folderOptions.Add(@"shaderCache\transferable", chkBoxShaderCaches.Checked);

            // ADDITIONAL OPTIONS
            handler.migrationOptions["copyCemuSettingsFile"] = chkBoxCemuSettings.Checked;
            handler.migrationOptions["deleteDestFolderContents"] = chkBoxDeletePrevContent.Checked;
            handler.migrationOptions["dontCopyMlcFolderFor1.10+"] = chkBoxCustomMlc01Path.Checked;
            
            if (!string.IsNullOrWhiteSpace(txtBoxCustomMlc01Path.Text))
                handler.mlcFolderExternalPath = txtBoxCustomMlc01Path.Text;
            else
                handler.mlcFolderExternalPath = "";

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
        }

        private void btnDiscard_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnDeleteSettingsFile_Click(object sender, EventArgs e)
        {
            handler.DeleteOptionsFile();            // To be improved, telling the user if options file existed before clicking the button(?)
            btnDeleteSettingsFile.Enabled = false;

            MessageBox.Show("Please take note that unless you uncheck \"Save options in a file\", options file will be recreated when you click on \"Save options\".",
                            "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void radioBtnExecFolder_CheckedChanged(object sender, EventArgs e)
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

        private void btnSaveOpts_Click(object sender, EventArgs e)
        {
            SetOptionsAccordingToCheckboxes();

            if (chkBoxSettingsOnFile.Checked)
                handler.WriteOptionsToFile();
            else
                MessageBox.Show("Please take note that if you don't store options in a file, they'll be lost as soon as you exit the application.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            Close();
        }

        private void chkBoxSettingsOnFile_CheckedChanged(object sender, EventArgs e)
        {
            if(!chkBoxSettingsOnFile.Checked)
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

                // Obviously, if current path is not set, tell the program to change it before writing the file on disk
                if (handler.optionsFilePath == "")
                    optionsFileLocationChanged = true;
            }
        }

        private void chkBoxCustomMlc01Path_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBoxCustomMlc01Path.Checked)
                txtBoxCustomMlc01Path.Enabled = true;
            else
                txtBoxCustomMlc01Path.Enabled = false;
        }
    }
}
