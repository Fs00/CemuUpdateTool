using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    public partial class UpdateForm : OperationsForm
    {
        public UpdateForm(OptionsManager opts) : base(opts)
        {
            InitializeComponent();
            txtBoxCemuFolder.ContextMenuStrip = new ContextMenuStrip();     // remove default menu strip
        }

        private void CheckFolderTextboxContent(object sender, EventArgs e)
        {
            // Check if it's a valid Cemu installation
            if (!FileUtils.IsValidCemuInstallation(txtBoxCemuFolder.Text, out string reason))
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
            string chosenFolder = ChooseFolder(txtBoxCemuFolder.Text);
            if (chosenFolder != null)
                txtBoxCemuFolder.Text = chosenFolder;
        }

        protected override async void DoOperationsAsync(object sender, EventArgs e)
        {
            // Start the textbox logger
            logUpdater = new TextBoxLogger(txtBoxLog);

            // Start preparing
            txtBoxLog.Clear();
            ChangeProgressLabelText("Preparing");
            btnStart.Enabled = false;
            btnBack.Enabled = false;
            WorkOutcome result = WorkOutcome.Success;

            // Create a new Worker instance and pass it all needed data
            ctSource = new CancellationTokenSource();
            worker = new Worker(txtBoxCemuFolder.Text, ctSource.Token, logUpdater.AppendLogMessage);

            // Starting from now, we can safely cancel operations without having problems
            btnCancel.Enabled = true;

            stopwatch.Start();
            try
            {
                // TODO
            }
            catch (Exception taskExc)
            {

            }
        }

        protected override void ResetEverything()
        {
            base.ResetEverything();

            // Reset Cemu version label
            lblCemuVersion.Visible = false;
            lblVersionNr.Text = "";

            // Reset textbox (I need to detach & reattach event handlers otherwise errorProviders will be triggered) and buttons
            txtBoxCemuFolder.TextChanged -= CheckFolderTextboxContent;
            txtBoxCemuFolder.Text = "";
            txtBoxCemuFolder.TextChanged += CheckFolderTextboxContent;
        }
    }
}
