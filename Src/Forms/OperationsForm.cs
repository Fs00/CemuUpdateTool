using CemuUpdateTool.Workers;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace CemuUpdateTool.Forms
{
    /*
     *  OperationsForm
     *  Form from which MigrationForm and UpdateForm inherit.
     *  It's designed to behave as an abstract class, but declaring it as abstract causes problems with VS Designer
     */
    /*abstract*/ partial class OperationsForm : Form
    {
        protected CancellationTokenSource ctSource;           // handles task cancellation
        protected Stopwatch stopwatch;                        // used to measure how much time the task took to complete
        protected TextBoxLogger logUpdater;                   // used to update txtBoxLog asynchronously

        protected OperationsForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            stopwatch = new Stopwatch();

            // Remove default (and useless) menu strips
            txtBoxLog.ContextMenuStrip = new ContextMenuStrip();
        }

        protected void Back(object sender, EventArgs evt)
        {
            ContainerForm.ShowHomeForm();
        }

        protected void OpenHelpForm(object sender, EventArgs evt)
        {
            new HelpForm(this).Show();
        }

        protected string ChooseFolder(string previouslySelectedFolder)
        {
            // Open folder picker in Computer or in the currently selected folder (if it exists)
            var folderPicker = new FolderBrowserDialog();
            folderPicker.RootFolder = Environment.SpecialFolder.MyComputer;
            if (!string.IsNullOrEmpty(previouslySelectedFolder) && Directory.Exists(previouslySelectedFolder))
                folderPicker.SelectedPath = previouslySelectedFolder;

            DialogResult result = folderPicker.ShowDialog();
            if (result == DialogResult.OK)
                return folderPicker.SelectedPath;
            else
                return null;
        }

        protected /*abstract*/ virtual void DoOperationsAsync(object sender, EventArgs evt) { }

        protected virtual void PrepareControlsForOperations()
        {
            logUpdater = new TextBoxLogger(txtBoxLog);

            txtBoxLog.Clear();
            ChangeProgressLabelText("Preparing");
            btnStart.Enabled = false;
            btnBack.Enabled = false;
            btnCancel.Enabled = true;
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
            ctSource.Cancel();
        }

        /*
         *  Shows a MessageBox with the final result of the task
         */
        protected void ShowWorkResultDialog(WorkOutcome result)
        {
            switch (result)
            {
                case WorkOutcome.Success:
                    MessageBox.Show("Operation successfully terminated.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case WorkOutcome.Aborted:
                    MessageBox.Show("Operation aborted due to an unexpected error.", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    break;
                case WorkOutcome.CancelledByUser:
                    MessageBox.Show("Operation cancelled by user.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case WorkOutcome.CompletedWithErrors:
                    MessageBox.Show("Operation terminated with errors.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        protected void HandleDownloadProgress(object sender, System.Net.DownloadProgressChangedEventArgs evtArgs)
        {
            // Set maximum progress bar value according to file size (happens only the first time)
            overallProgressBar.Maximum = (int)evtArgs.TotalBytesToReceive;

            // Update percent label and progress bar
            lblPercent.Text = evtArgs.ProgressPercentage + "%";
            overallProgressBar.Value = (int)evtArgs.BytesReceived;

            // Refresh log textbox to make sure that log is always updated
            logUpdater.UpdateTextBox();
        }

        /*
         *  Callback method that updates current task label according to the next task
         *  It also updates log textbox in order to prevent messages not being written if progress bar isn't updated during a task
         */
        protected void ChangeProgressLabelText(string newLabelText)
        {
            lblCurrentTask.Text = $"{newLabelText}...";
            logUpdater.AppendLogMessage($"-- {newLabelText} --");
            logUpdater.UpdateTextBox();
        }

        #region Methods for handling drag & drop into folder textboxes
        protected virtual void ChangeCursorEffectOnTextboxDragEnter(object sender, DragEventArgs evt)
        {
            if (evt.Data.GetDataPresent(DataFormats.Text) || evt.Data.GetDataPresent(DataFormats.FileDrop))
                evt.Effect = DragDropEffects.Copy;
            else
                evt.Effect = DragDropEffects.None;
        }

        protected virtual void PasteContentIntoTextboxOnDragDrop(object sender, DragEventArgs evt)
        {
            if (evt.Data.GetDataPresent(DataFormats.FileDrop))
                (sender as TextBox).Text = (evt.Data.GetData(DataFormats.FileDrop) as string[])[0];
            else if (evt.Data.GetDataPresent(DataFormats.Text))
                (sender as TextBox).Text = evt.Data.GetData(DataFormats.Text).ToString();
        }
        #endregion

        /*
         *  Shows/hides log textbox when clicking on Details label
         */
        protected void ShowHideDetailsTextbox(object sender, EventArgs evt)
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
        protected virtual void ResizeFormOnLogTextboxVisibleChanged(object sender, EventArgs evt)
        {
            // Avoid triggering the event before the form is shown
            if (ContainerForm.IsCurrentDisplayingForm(this))
            {
                if (txtBoxLog.Visible)
                    this.Height += txtBoxLog.Height;
                else
                    this.Height -= txtBoxLog.Height;
            }
        }

        protected void PreventClosingIfOperationInProgress(object sender, FormClosingEventArgs evt)
        {
            if (logUpdater != null && logUpdater.IsReady)
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
