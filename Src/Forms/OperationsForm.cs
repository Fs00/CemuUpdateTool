using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CemuUpdateTool.Settings;
using CemuUpdateTool.Workers;

namespace CemuUpdateTool.Forms
{
    /*
     *  Provides common layout and functionality for MigrationForm and UpdateForm
     *  Note: declaring it as abstract causes problems with VS Designer
     */
    abstract partial class OperationsForm : Form
    {
        protected CancellationTokenSource cTokenSource;
        
        private readonly TextBoxLogger logUpdater;       // used to update txtBoxLog asynchronously
        private readonly Stopwatch stopwatch;     // used to measure how much time the task took to complete

        private WorkOutcome workResult;
        private string workFailureMessage;

        protected OperationsForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            stopwatch = new Stopwatch();
            logUpdater = new TextBoxLogger(txtBoxLog);
        }

        protected virtual void RemoveDefaultTextBoxContextMenus()
        {
            txtBoxLog.ContextMenuStrip = new ContextMenuStrip();
        }

        private void Back(object sender, EventArgs evt)
        {
            ContainerForm.ShowHomeForm();
        }

        private void OpenHelpForm(object sender, EventArgs evt)
        {
            new HelpForm(this).Show();
        }

        protected string ChooseFolderWithPicker(string previouslySelectedFolder)
        {
            var folderPicker = new FolderBrowserDialog {
                RootFolder = Environment.SpecialFolder.MyComputer
            };
            if (!string.IsNullOrEmpty(previouslySelectedFolder) && Directory.Exists(previouslySelectedFolder))
                folderPicker.SelectedPath = previouslySelectedFolder;

            DialogResult result = folderPicker.ShowDialog();
            if (result == DialogResult.OK)
                return folderPicker.SelectedPath;
            else
                return null;
        }

        protected static bool DirectoryContainsACemuInstallation(string path, out string reason)
        {
            reason = null;
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
                reason = "Directory does not exist";
            else if (!File.Exists(Path.Combine(path, "Cemu.exe")))
                reason = "Not a valid Cemu installation (Cemu.exe is missing)";

            return reason == null;
        }

        private async void DoOperationsAsync(object sender, EventArgs evt)
        {
            if (!ArePreliminaryChecksSuccessful())
                return;
            
            logUpdater.Start();
            PrepareControlsForOperations();
            await TryPerformOperationsAsync();
            AppendResultLogMessage();
            logUpdater.StopAndPrintAllBufferedContent();
            ShowWorkResultDialog();
            ResetControls();
        }

        protected virtual bool ArePreliminaryChecksSuccessful() => true;
        
        protected virtual void PrepareControlsForOperations()
        {
            txtBoxLog.Clear();
            UpdateCurrentTaskLabel("Preparing");
            btnStart.Enabled = false;
            btnBack.Enabled = false;
            btnCancel.Enabled = true;
        }

        private async Task TryPerformOperationsAsync()
        {
            cTokenSource = new CancellationTokenSource();
            try
            {
                stopwatch.Start();
                workResult = await PerformOperationsAsync();
                lblCurrentTask.Text = "Operations completed!";
            }
            catch (Exception exc)
            {
                HandleOperationsError();
                
                lblCurrentTask.Text = "Operations stopped!";
                if (exc is OperationCanceledException)
                    workResult = WorkOutcome.CancelledByUser;
                else
                {
                    workFailureMessage = exc.Message;
                    workResult = WorkOutcome.Aborted;
                }
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        protected abstract Task<WorkOutcome> PerformOperationsAsync();
        protected virtual void HandleOperationsError() {}
        
        private void AppendResultLogMessage()
        {
            switch (workResult)
            {
                case WorkOutcome.Success:
                    logUpdater.AppendLogMessage(
                        $"\r\nOperations terminated without errors after {stopwatch.Elapsed.TotalSeconds} seconds.",
                        newLine: false
                    );
                    break;
                case WorkOutcome.CompletedWithErrors:
                    logUpdater.AppendLogMessage(
                        $"\r\nOperations terminated with errors after {stopwatch.Elapsed.TotalSeconds} seconds.",
                        newLine: false
                    );
                    break;
                case WorkOutcome.Aborted:
                    logUpdater.AppendLogMessage($"\r\nOperations aborted due to unrecoverable error: {workFailureMessage}", newLine: false);
                    break;
                case WorkOutcome.CancelledByUser:
                    logUpdater.AppendLogMessage("\r\nOperations cancelled due to user request.", false);
                    break;
            }
        }
        
        protected static void UpdateLastKnownCemuVersionOption(VersionNumber downloadedCemuVersion)
        {
            VersionNumber.TryParse(Options.Download[OptionKey.LastKnownCemuVersion],
                                   out VersionNumber previousLastKnownCemuVersion);
            if (previousLastKnownCemuVersion != downloadedCemuVersion)
                Options.Download[OptionKey.LastKnownCemuVersion] = downloadedCemuVersion.ToString();
        }

        protected void TryUpdateOptionsFile()
        {
            try
            {
                Options.WriteOptionsToCurrentlySelectedFile();
            }
            catch (Exception optionsUpdateExc)
            {
                logUpdater.AppendLogMessage(
                    $"WARNING: Unable to update settings file with the latest known Cemu version: {optionsUpdateExc.Message}");
            }
        }

        /*
         *  Resets the GUI in order for the form to be ready for another task
         */
        protected virtual void ResetControls()
        {
            ResetProgressBarAndText();
            ResetButtonsState();
            stopwatch.Reset();
        }

        private void ResetButtonsState()
        {
            btnCancel.Enabled = false;
            btnBack.Enabled = true;
        }

        private void ResetProgressBarAndText()
        {
            overallProgressBar.Value = 0;
            lblPercent.Text = "0%";
            lblCurrentTask.Text = "Waiting for operations to start...";
        }

        protected void CancelOperations(object sender = null, EventArgs evt = null)
        {
            lblCurrentTask.Text = "Cancelling...";
            cTokenSource.Cancel();
        }
        
        private void ShowWorkResultDialog()
        {
            switch (workResult)
            {
                case WorkOutcome.Success:
                    MessageBox.Show("Operations successfully terminated.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case WorkOutcome.Aborted:
                    MessageBox.Show("Operations aborted due to an unrecoverable error. See Details for more information.",
                            "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    break;
                case WorkOutcome.CancelledByUser:
                    MessageBox.Show("Operations cancelled by user.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case WorkOutcome.CompletedWithErrors:
                    MessageBox.Show("Operations terminated with errors.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        protected void AttachProgressEventHandlersToWorker(Worker worker)
        {
            worker.WorkStart += UpdateCurrentTaskLabel;
            worker.LogMessage += AppendLogMessageToTextBox;
            worker.ProgressChange += SetProgressBarCurrentAndMaximum;
            worker.ProgressIncrement += IncrementProgressBarValue;
        }
        
        private void UpdateCurrentTaskLabel(string newLabelText)
        {
            lblCurrentTask.Text = $"{newLabelText}...";
            logUpdater.AppendLogMessage($"-- {newLabelText} --");
        }
        
        private void SetProgressBarCurrentAndMaximum(int currentProgress, int maximumProgress)
        {
            overallProgressBar.Maximum = maximumProgress;
            overallProgressBar.Value = currentProgress;
            UpdatePercentageLabel();
        }

        private void IncrementProgressBarValue(int incrementValue)
        {
            overallProgressBar.Value += incrementValue;
            UpdatePercentageLabel();
        }

        private void UpdatePercentageLabel()
        {
            lblPercent.Text = CalculatePercentage(overallProgressBar.Value, overallProgressBar.Maximum) + "%";
        }
        
        private decimal CalculatePercentage(int numerator, int denominator)
        {
            return Math.Floor((decimal) numerator / denominator * 100);
        }

        private void AppendLogMessageToTextBox(string message, bool newLine)
        {
            logUpdater.AppendLogMessage(message, newLine);
        }

        #region Methods for handling drag & drop into folder textboxes
        protected virtual void ChangeCursorEffectOnTextBoxDragEnter(object sender, DragEventArgs evt)
        {
            if (evt.Data.GetDataPresent(DataFormats.Text) || evt.Data.GetDataPresent(DataFormats.FileDrop))
                evt.Effect = DragDropEffects.Copy;
            else
                evt.Effect = DragDropEffects.None;
        }

        protected virtual void PasteContentIntoTextBoxOnDragDrop(object sender, DragEventArgs evt)
        {
            if (evt.Data.GetDataPresent(DataFormats.FileDrop))
                ((TextBox) sender).Text = ((string[]) evt.Data.GetData(DataFormats.FileDrop))[0];
            else if (evt.Data.GetDataPresent(DataFormats.Text))
                ((TextBox) sender).Text = evt.Data.GetData(DataFormats.Text).ToString();
        }
        #endregion

        private void ShowOrHideDetailsTextBox(object sender, EventArgs evt)
        {
            ReplaceArrowNearDetails();
            txtBoxLog.Visible = !txtBoxLog.Visible;
        }

        private void ReplaceArrowNearDetails()
        {
            const char ARROW_DOWN = (char) 9661;
            const char ARROW_RIGHT = (char) 9655;

            if (txtBoxLog.Visible)
                lblDetails.Text = lblDetails.Text.Replace(ARROW_DOWN, ARROW_RIGHT);
            else
                lblDetails.Text = lblDetails.Text.Replace(ARROW_RIGHT, ARROW_DOWN);
        }

        /*
         *  Resizes the form when txtBoxLog's visible state changes
         *  Note: this event handler must be added only on inherited forms, otherwise the designer will crash
         */
        protected void ResizeFormOnLogTextBoxVisibleChanged(object sender, EventArgs evt)
        {
            // Avoid triggering the event before the form is shown
            if (ContainerForm.IsFormCurrentlyDisplayed(this))
            {
                if (txtBoxLog.Visible)
                    this.Height += txtBoxLog.Height;
                else
                    this.Height -= txtBoxLog.Height;
            }
        }

        private void PreventClosingIfOperationInProgress(object sender, FormClosingEventArgs evt)
        {
            if (logUpdater.IsRunning)
            {
                evt.Cancel = true;
                DialogResult choice = MessageBox.Show(
                    "You can't close this window while an operation is in progress. Do you want to cancel the job?",
                    "Exit not allowed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning
                );
                if (choice == DialogResult.Yes)
                    CancelOperations();
            }
        }
    }
}
