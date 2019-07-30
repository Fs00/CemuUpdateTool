using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using CemuUpdateTool.Settings;
using CemuUpdateTool.Workers;

namespace CemuUpdateTool.Forms
{
    /*
     *  UpdateForm
     *  Window that provides Update functionality.
     */
    partial class UpdateForm : OperationsForm
    {
        public UpdateForm() : base()
        {
            InitializeComponent();
            txtBoxCemuFolder.ContextMenuStrip = new ContextMenuStrip();     // remove default menu strip
        }

        private void CheckFolderTextboxContent(object sender, EventArgs e)
        {
            if (!DirectoryContainsACemuInstallation(txtBoxCemuFolder.Text, out string reason))
            {
                errProviderFolders.SetError(txtBoxCemuFolder, reason);
                btnStart.Enabled = false;

                lblCemuVersion.Visible = false;
                lblVersionNr.Text = "";
            }
            // Display Cemu version label and enable start button
            else
            {
                errProviderFolders.SetError(txtBoxCemuFolder, "");

                lblCemuVersion.Visible = true;
                lblVersionNr.Text = new VersionNumber(FileVersionInfo.GetVersionInfo(Path.Combine(txtBoxCemuFolder.Text, "Cemu.exe")), 3).ToString();

                btnStart.Enabled = true;
            }
        }

        private void SelectCemuFolder(object sender, EventArgs e)
        {
            string chosenFolder = ChooseFolderWithPicker(txtBoxCemuFolder.Text);
            if (chosenFolder != null)
                txtBoxCemuFolder.Text = chosenFolder;
        }
        
        protected override void HandleOperationsError()
        {
            // nothing to do here
        }

        protected override async Task<WorkOutcome> PerformOperations()
        {
            var updater = new Updater(txtBoxCemuFolder.Text, cTokenSource.Token);
            
            VersionNumber downloadedCemuVersion = await Task.Run(
                () => updater.PerformUpdateOperations(chkBoxDeletePrecompiled.Checked, chkBoxUpdGameProfiles.Checked)
            );
            
            // Update settings file with the new value of lastKnownCemuVersion (if it's changed)
            VersionNumber.TryParse(Options.Download[OptionKey.LastKnownCemuVersion], out VersionNumber previousLastKnownCemuVersion);
            if (previousLastKnownCemuVersion != downloadedCemuVersion)
            {
                Options.Download[OptionKey.LastKnownCemuVersion] = downloadedCemuVersion.ToString();
                try
                {
                    Options.WriteOptionsToCurrentlySelectedFile();
                }
                catch (Exception optionsUpdateExc)
                {
                    logUpdater.AppendLogMessage($"WARNING: Unable to update settings file with the latest known Cemu version: {optionsUpdateExc.Message}");
                }
            }

            if (updater.ErrorsEncountered > 0)
                return WorkOutcome.CompletedWithErrors;

            return WorkOutcome.Success;
        }

        protected override void ResetControls()
        {
            base.ResetControls();

            // Reset Cemu version label
            lblCemuVersion.Visible = false;
            lblVersionNr.Text = "";

            // Reset textbox (I need to detach & reattach event handlers otherwise errorProviders will be triggered) and buttons
            txtBoxCemuFolder.TextChanged -= CheckFolderTextboxContent;
            txtBoxCemuFolder.Text = "";
            txtBoxCemuFolder.TextChanged += CheckFolderTextboxContent;
        }

        // These "fake overrides" are needed to avoid VS designer errors
        protected override void ResizeFormOnLogTextboxVisibleChanged(object sender, EventArgs e) { base.ResizeFormOnLogTextboxVisibleChanged(sender, e); }
        protected override void PasteContentIntoTextboxOnDragDrop(object sender, DragEventArgs e) { base.PasteContentIntoTextboxOnDragDrop(sender, e); }
        protected override void ChangeCursorEffectOnTextboxDragEnter(object sender, DragEventArgs e) { base.ChangeCursorEffectOnTextboxDragEnter(sender, e); }
    }
}
