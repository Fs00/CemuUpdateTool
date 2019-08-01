using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using CemuUpdateTool.Utils;
using CemuUpdateTool.Workers;

namespace CemuUpdateTool.Forms
{
    /*
     *  Window that provides Update functionality
     */
    sealed partial class UpdateForm : OperationsForm
    {
        private Updater updater;
        
        public UpdateForm() : base()
        {
            InitializeComponent();
            RemoveDefaultTextBoxContextMenus();
        }

        protected override void RemoveDefaultTextBoxContextMenus()
        {
            base.RemoveDefaultTextBoxContextMenus();
            txtBoxCemuFolder.ContextMenuStrip = new ContextMenuStrip();
        }

        private void CheckCemuFolderTextBoxContent(object sender, EventArgs e)
        {
            if (!DirectoryContainsACemuInstallation(txtBoxCemuFolder.Text, out string reason))
            {
                errProviderFolders.SetError(txtBoxCemuFolder, reason);
                btnStart.Enabled = false;
                ResetCemuVersionLabels();
            }
            else
            {
                errProviderFolders.SetError(txtBoxCemuFolder, "");
                btnStart.Enabled = true;
                UpdateCemuVersionLabelsAccordingToSelectedFolder();
            }
        }

        private void UpdateCemuVersionLabelsAccordingToSelectedFolder()
        {
            VersionNumber selectedFolderCemuVersion = 
                FileUtils.RetrieveExecutableVersionNumber(Path.Combine(txtBoxCemuFolder.Text, "Cemu.exe"));
            lblCemuVersion.Visible = true;
            lblCemuVersionNumber.Text = selectedFolderCemuVersion.ToString();
        }

        private void SelectCemuFolder(object sender, EventArgs e)
        {
            string chosenFolder = ChooseFolderWithPicker(txtBoxCemuFolder.Text);
            if (chosenFolder != null)
                txtBoxCemuFolder.Text = chosenFolder;
        }

        protected override async Task<WorkOutcome> PerformOperationsAsync()
        {
            VersionNumber downloadedCemuVersion = await PerformUpdateOperationsAsync();
            UpdateLastKnownCemuVersionOption(downloadedCemuVersion);
            TryUpdateOptionsFile();

            if (updater.ErrorsEncountered > 0)
                return WorkOutcome.CompletedWithErrors;
            
            return WorkOutcome.Success;
        }

        private async Task<VersionNumber> PerformUpdateOperationsAsync()
        {
            updater = new Updater(txtBoxCemuFolder.Text, cTokenSource.Token);
            AttachProgressEventHandlersToWorker(updater);
            VersionNumber downloadedCemuVersion = await Task.Run(
                () => updater.PerformUpdateOperations(chkBoxDeletePrecompiled.Checked, chkBoxUpdGameProfiles.Checked)
            );
            return downloadedCemuVersion;
        }

        protected override void ResetControls()
        {
            base.ResetControls();
            ResetCemuVersionLabels();
            ResetCemuFolderTextBox();
        }
        
        private void ResetCemuVersionLabels()
        {
            lblCemuVersion.Visible = false;
            lblCemuVersionNumber.Text = "";
        }

        private void ResetCemuFolderTextBox()
        {
            txtBoxCemuFolder.TextChanged -= CheckCemuFolderTextBoxContent;
            txtBoxCemuFolder.Text = "";
            txtBoxCemuFolder.TextChanged += CheckCemuFolderTextBoxContent;
        }

        // These "fake overrides" are needed on Visual Studio to avoid form designer errors
        /*protected override void ResizeFormOnLogTextBoxVisibleChanged(object sender, EventArgs e) { base.ResizeFormOnLogTextBoxVisibleChanged(sender, e); }
        protected override void PasteContentIntoTextBoxOnDragDrop(object sender, DragEventArgs e) { base.PasteContentIntoTextBoxOnDragDrop(sender, e); }
        protected override void ChangeCursorEffectOnTextBoxDragEnter(object sender, DragEventArgs e) { base.ChangeCursorEffectOnTextBoxDragEnter(sender, e); }*/
    }
}
