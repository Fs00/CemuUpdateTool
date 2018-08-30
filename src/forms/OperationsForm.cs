using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    public /*abstract*/ partial class OperationsForm : Form
    {
        protected OptionsManager opts;
        protected Worker worker;

        protected CancellationTokenSource ctSource;           // handles task cancellation
        protected Stopwatch stopwatch;                        // used to measure how much time the task took to complete
        protected TextBoxLogger logUpdater;                   // used to update txtBoxLog asynchronously

        public OperationsForm() : this(null) { }    // needed by VS Designer

        public OperationsForm(OptionsManager opts)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            this.opts = opts;

            stopwatch = new Stopwatch();

            // Remove default (and useless) menu strips
            txtBoxLog.ContextMenuStrip = new ContextMenuStrip();
        }

        protected void Back(object sender, EventArgs e)
        {
            ContainerForm.ShowHomeForm();
        }

        protected void OpenHelpForm(object sender, EventArgs e)
        {
            new HelpForm(this).Show();
        }

        protected string ChooseFolder(string previouslySelectedFolder)
        {
            // Open folder picker in Computer or in the currently selected folder (if it exists)
            var folderPicker = new FolderBrowserDialog();
            folderPicker.RootFolder = Environment.SpecialFolder.MyComputer;
            if (!string.IsNullOrEmpty(previouslySelectedFolder) && FileUtils.DirectoryExists(previouslySelectedFolder))
                folderPicker.SelectedPath = previouslySelectedFolder;

            DialogResult result = folderPicker.ShowDialog();
            if (result == DialogResult.OK)
                return folderPicker.SelectedPath;
            else
                return null;
        }

        protected /*abstract*/ virtual void DoOperationsAsync(object sender, EventArgs e) { }

        /*
         *  Resets the GUI and all Worker-related variables in order for the form to be ready for another task
         */
        protected virtual void ResetEverything()
        {
            // Reset progress bars
            overallProgressBar.Value = 0;
            lblPercent.Text = "0%";
            lblCurrentTask.Text = "Waiting for operations to start...";

            btnCancel.Enabled = false;
            btnBack.Enabled = true;

            // Reset stopwatch
            stopwatch.Reset();

            // Tell the textbox logger to stop after printing all queued messages
            logUpdater.StopAndWaitShutdown();
        }

        protected void CancelOperations(object sender = null, EventArgs e = null)
        {
            lblCurrentTask.Text = "Cancelling...";
            ctSource.Cancel();
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

        /*
         *  Methods for handling drag & drop into folder textboxes
         */
        protected void TextboxDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text) || e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        protected void TextboxDragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                (sender as TextBox).Text = (e.Data.GetData(DataFormats.FileDrop) as string[])[0];
            else if (e.Data.GetDataPresent(DataFormats.Text))
                (sender as TextBox).Text = e.Data.GetData(DataFormats.Text).ToString();
        }

        /*
         *  Shows/hides log textbox when clicking on Details label
         */
        protected void ShowHideDetailsTextbox(object sender, EventArgs e)
        {
            if (txtBoxLog.Visible)  // arrow down -> arrow right
                lblDetails.Text = lblDetails.Text.Replace((char)9661, (char)9655);
            else                    // arrow right -> arrow down
                lblDetails.Text = lblDetails.Text.Replace((char)9655, (char)9661);

            txtBoxLog.Visible = !txtBoxLog.Visible;
        }

        /*
         *  Resizes the form when txtBoxLog's visible state changes
         */
        protected void ResizeFormOnLogTextboxVisibleChanged(object sender, EventArgs e)
        {
            if (ContainerForm.IsCurrentDisplayingForm(this))     // avoid triggering the event before the form is shown
            {
                if (txtBoxLog.Visible)
                    this.Height += txtBoxLog.Height;
                else
                    this.Height -= txtBoxLog.Height;
            }
        }

        /*
         *  If there's a job running, cancel window closing and ask the user if he wants to cancel operations
         */
        protected void PreventClosingIfOperationInProgress(object sender, FormClosingEventArgs e)
        {
            if (logUpdater != null && logUpdater.IsReady)
            {
                e.Cancel = true;
                DialogResult choice = MessageBox.Show("You can't close this window while an operation is in progress. Do you want to cancel the job?",
                                                      "Exit not allowed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (choice == DialogResult.Yes)
                    CancelOperations();
            }
        }
    }
}
