using CemuUpdateTool.Settings;
using CemuUpdateTool.Utils;
using CemuUpdateTool.Workers;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            string chosenFolder = ChooseFolder(txtBoxCemuFolder.Text);
            if (chosenFolder != null)
                txtBoxCemuFolder.Text = chosenFolder;
        }

        protected override async void DoOperationsAsync(object sender, EventArgs e)
        {
            PrepareControlsForOperations();

            ctSource = new CancellationTokenSource();
            var updater = new Updater(txtBoxCemuFolder.Text, ctSource.Token);

            WorkOutcome result;
            stopwatch.Start();
            try
            {
                VersionNumber downloadedCemuVersion = await Task.Run(() => updater.PerformUpdateOperations(chkBoxDeletePrecompiled.Checked, chkBoxUpdGameProfiles.Checked));
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

                stopwatch.Stop();

                if (updater.ErrorsEncountered > 0)
                {
                    logUpdater.AppendLogMessage($"\r\nOperations terminated with {updater.ErrorsEncountered} errors after {(float)stopwatch.ElapsedMilliseconds / 1000} seconds.", false);
                    result = WorkOutcome.CompletedWithErrors;
                }
                else
                {
                    logUpdater.AppendLogMessage($"\r\nOperations terminated without errors after {(float)stopwatch.ElapsedMilliseconds / 1000} seconds.", false);
                    result = WorkOutcome.Success;
                }
                lblCurrentTask.Text = "Operations completed!";
            }
            catch (Exception taskExc)
            {
                stopwatch.Stop();

                // Update result according to caught exception type
                if (taskExc is OperationCanceledException)
                {
                    logUpdater.AppendLogMessage("\r\nOperations cancelled due to user request.", false);
                    result = WorkOutcome.CancelledByUser;
                }
                else
                {
                    logUpdater.AppendLogMessage($"\r\nOperation aborted due to unrecoverable error: {taskExc.Message}", false);
                    result = WorkOutcome.Aborted;
                }
                lblCurrentTask.Text = "Operations stopped!";
            }

            // Tell the textbox logger to stop after printing all queued messages
            logUpdater.StopAndWaitShutdown();

            ShowWorkResultDialog(result);
            ResetControls();
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
