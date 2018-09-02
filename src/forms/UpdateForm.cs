﻿using System;
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
            // Worker's BaseDestinationPath is to a temporary folder to reuse correctly download operations method (see PerformUpdateOperations)
            ctSource = new CancellationTokenSource();
            worker = new Worker(Path.Combine(Path.GetTempPath(), "cemu_update"), ctSource.Token, logUpdater.AppendLogMessage);

            // Starting from now, we can safely cancel operations without having problems
            btnCancel.Enabled = true;

            stopwatch.Start();
            try
            {
                VersionNumber downloadedCemuVersion = await Task.Run(() => worker.PerformUpdateOperations(txtBoxCemuFolder.Text, chkBoxDeletePrecompiled.Checked, chkBoxUpdGameProfiles.Checked,
                                                                                                          opts.Download, ChangeProgressLabelText, HandleDownloadProgress));
                // Update settings file with the new value of lastKnownCemuVersion (if it's changed)
                VersionNumber.TryParse(opts.Download[OptionsKeys.LastKnownCemuVersion], out VersionNumber previousLastKnownCemuVersion);
                if (previousLastKnownCemuVersion != downloadedCemuVersion)
                {
                    opts.Download[OptionsKeys.LastKnownCemuVersion] = downloadedCemuVersion.ToString();
                    try
                    {
                        opts.WriteOptionsToFile();
                    }
                    catch (Exception optionsUpdateExc)
                    {
                        logUpdater.AppendLogMessage($"WARNING: Unable to update settings file with the latest known Cemu version: {optionsUpdateExc.Message}");
                    }
                }

                stopwatch.Stop();

                // If there have been errors during operations, update result
                if (worker.ErrorsEncountered > 0)
                    result = WorkOutcome.CompletedWithErrors;
            }
            catch (Exception taskExc)
            {
                stopwatch.Stop();

                // Update result according to caught exception type
                if (taskExc is OperationCanceledException)
                    result = WorkOutcome.CancelledByUser;
                else
                {
                    logUpdater.AppendLogMessage($"\r\nOperation aborted due to unrecoverable error: {taskExc.Message}", false);
                    result = WorkOutcome.Aborted;
                }
            }

            ShowWorkResultDialog(result);
            // Reset form controls to their original state
            ResetEverything();
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

        // These "fake overrides" are needed to avoid VS designer errors
        protected override void ResizeFormOnLogTextboxVisibleChanged(object sender, EventArgs e) { base.ResizeFormOnLogTextboxVisibleChanged(sender, e); }
        protected override void TextboxDragDrop(object sender, DragEventArgs e) { base.TextboxDragDrop(sender, e); }
        protected override void TextboxDragEnter(object sender, DragEventArgs e) { base.TextboxDragEnter(sender, e); }
    }
}
