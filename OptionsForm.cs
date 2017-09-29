using System;
using System.Windows.Forms;
using System.IO;

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

        public void SetCheckboxesAccordingToOptions()
        {
            // Name is self-explanatory
            if (handler.options.ContainsKey("controllerProfiles"))
                chkBoxControllerProfiles.Checked = handler.options["controllerProfiles"];
            if (handler.options.ContainsKey("gameProfiles"))
                chkBoxGameProfiles.Checked = handler.options["gameProfiles"];
            if (handler.options.ContainsKey("graphicPacks"))
                chkBoxGfxPacks.Checked = handler.options["graphicPacks"];
            if (handler.options.ContainsKey(@"mlc01\emulatorSave"))
                chkBoxSavegames.Checked = handler.options[@"mlc01\emulatorSave"];
            if (handler.options.ContainsKey(@"mlc01\usr\title"))
                chkBoxDLCUpds.Checked = handler.options[@"mlc01\usr\title"];
            if (handler.options.ContainsKey(@"shaderCache\transferable"))
                chkBoxShaderCaches.Checked = handler.options[@"shaderCache\transferable"];
            chkBoxDeletePrevContent.Checked = handler.deleteDestFolderContents;

            if (handler.optionsFilePath == handler.LOCAL_FILEPATH)
                radioBtnExecFolder.Checked = true;
            else
                radioBtnAppDataFolder.Checked = true;
        }

        private void SetOptionsAccordingToCheckboxes()
        {
            // Same as above
            if (handler.options.ContainsKey("controllerProfiles"))
                handler.options["controllerProfiles"] = chkBoxControllerProfiles.Checked;
            else
                handler.options.Add("controllerProfiles", chkBoxControllerProfiles.Checked);

            if (handler.options.ContainsKey("gameProfiles"))
                handler.options["gameProfiles"] = chkBoxGameProfiles.Checked;
            else
                handler.options.Add("gameProfiles", chkBoxGameProfiles.Checked);

            if (handler.options.ContainsKey("graphicPacks"))
                handler.options["graphicPacks"] = chkBoxGfxPacks.Checked;
            else
                handler.options.Add("graphicPacks", chkBoxGfxPacks.Checked);

            if (handler.options.ContainsKey(@"mlc01\emulatorSave"))
                handler.options[@"mlc01\emulatorSave"] = chkBoxSavegames.Checked;
            else
                handler.options.Add(@"mlc01\emulatorSave", chkBoxSavegames.Checked);

            if (handler.options.ContainsKey(@"mlc01\usr\title"))
                handler.options[@"mlc01\usr\title"] = chkBoxDLCUpds.Checked;
            else
                handler.options.Add(@"mlc01\usr\title", chkBoxDLCUpds.Checked);

            if (handler.options.ContainsKey(@"shaderCache\transferable"))
                handler.options[@"shaderCache\transferable"] = chkBoxShaderCaches.Checked;
            else
                handler.options.Add(@"shaderCache\transferable", chkBoxShaderCaches.Checked);

            handler.deleteDestFolderContents = chkBoxDeletePrevContent.Checked;     // at the moment it will always be false

            if (optionsFileLocationChanged)
            {
                handler.DeleteOptionsFile();
                if (handler.optionsFilePath == handler.LOCAL_FILEPATH)
                    handler.optionsFilePath = handler.APPDATA_FILEPATH;
                else
                    handler.optionsFilePath = handler.LOCAL_FILEPATH;
            }
        }

        private void btnDiscard_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnDeleteSettingsFile_Click(object sender, EventArgs e)
        {
            handler.DeleteOptionsFile();            // To be improved, telling the user if options file existed before clicking the button
            btnDeleteSettingsFile.Enabled = false;

            MessageBox.Show("Please take note that if you click on \"Save options\", the options file will be recreated. " +
                "To avoid this, click on \"Discard changes\" or close the window.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void radioBtnExecFolder_CheckedChanged(object sender, EventArgs e)
        {
            if(handler.optionsFilePath == handler.LOCAL_FILEPATH)
            {
                if (!radioBtnExecFolder.Checked)
                    optionsFileLocationChanged = true;
                else
                    optionsFileLocationChanged = false;
            }
            else
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
            handler.WriteOptionsToFile();
            Close();
        }
    }
}
