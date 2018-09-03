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
            WorkOutcome result;
            PrepareControlsForOperations();

            // Create a new Worker instance and pass it all needed data
            // Worker's BaseDestinationPath is to a temporary folder to reuse correctly download operations method (see PerformUpdateOperations)
            ctSource = new CancellationTokenSource();
            worker = new Worker(Path.Combine(Path.GetTempPath(), "cemu_update"), ctSource.Token, logUpdater.AppendLogMessage);

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

                if (worker.ErrorsEncountered > 0)
                {
                    logUpdater.AppendLogMessage($"\r\nOperations terminated with {worker.ErrorsEncountered} errors after {(float)stopwatch.ElapsedMilliseconds / 1000} seconds.", false);
                    result = WorkOutcome.CompletedWithErrors;
                }
                else
                {
                    logUpdater.AppendLogMessage($"\r\nOperations terminated without errors after {(float)stopwatch.ElapsedMilliseconds / 1000} seconds.", false);
                    result = WorkOutcome.Success;
                }
            }
            catch (Exception taskExc)
            {
                stopwatch.Stop();

                // Update result according to caught exception type
                if (taskExc is OperationCanceledException)
                {
                    logUpdater.AppendLogMessage($"\r\nOperations cancelled due to user request.", false);
                    result = WorkOutcome.CancelledByUser;
                }
                else
                {
                    logUpdater.AppendLogMessage($"\r\nOperation aborted due to unrecoverable error: {taskExc.Message}", false);
                    result = WorkOutcome.Aborted;
                }
            }

            ResetState();
            ShowWorkResultDialog(result);
        }

        protected override void ResetState()
        {
            base.ResetState();

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
