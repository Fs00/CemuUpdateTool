using System;
using System.Threading;
using System.Windows.Forms;

namespace CemuUpdateTool.Workers
{
    public abstract class Worker
    {
        public int ErrorsEncountered      { private set; get; }

        public event Action<string> WorkStart;
        public event Action<string, bool> LogMessage;
        public event Action<int, int> ProgressChange;
        public event Action<int> ProgressIncrement;

        protected CancellationToken cancToken;

        public Worker(CancellationToken cancToken)
        {
            this.cancToken = cancToken;
        }

        public void ThrowIfWorkIsCancelled()
        {
            cancToken.ThrowIfCancellationRequested();
        }

        public ErrorHandlingDecision DecideHowToHandleError(OperationInfo operationInfo, string errorMessage)
        {
            string dialogMessage = GetErrorDialogMessageForOperation(operationInfo, errorMessage);
            string dialogTitle = GetErrorDialogTitleForOperation(operationInfo);

            DialogResult choice = MessageBox.Show(dialogMessage, dialogTitle,
                                  MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);

            var decision = choice.ToErrorHandlingDecision();
            if (decision == ErrorHandlingDecision.Ignore)
            {
                OnOperationErrorHandled(operationInfo, errorMessage);
                ErrorsEncountered++;
            }

            return decision;
        }

        private string GetErrorDialogMessageForOperation(OperationInfo operationInfo, string errorMessage)
        {
            // TODO
        }

        private string GetErrorDialogTitleForOperation(OperationInfo operationInfo)
        {
            string dialogTitle = "Error during ";
            switch (operationInfo)
            {
                case FileCopyOperationInfo _:
                    dialogTitle += "file copy";
                    break;
                case FileDeletionOperationInfo _:
                    dialogTitle += "file deletion";
                    break;
                case FileExtractionOperationInfo _:
                    dialogTitle += "file extraction";
                    break;
            }
            return dialogTitle;
        }

        public virtual void OnOperationStart(OperationInfo operationInfo)
        {
            switch (operationInfo)
            {
                case FileCopyOperationInfo fileCopyOperationInfo:
                    OnLogMessage(LogMessageType.Information, $"Copying {fileCopyOperationInfo.CopiedFile.FullName}... ", false);
                    break;
            }
        }

        public virtual void OnOperationSuccess(OperationInfo operationInfo)
        {
            switch (operationInfo)
            {
                case FileCopyOperationInfo _:
                    OnLogMessage(LogMessageType.Information, "Done!");
                    break;
            }
        }

        public virtual void OnOperationErrorHandled(OperationInfo operationInfo, string errorMessage)
        {
            // TODO print log messages after error has been ignored
        }

        protected void OnLogMessage(LogMessageType type, string message, bool newLine = true)
        {
            string logMessage = "";
            if (type == LogMessageType.Error)
                logMessage += "ERROR: ";
            else if (type == LogMessageType.Warning)
                logMessage += "WARNING: ";

            logMessage += message;
            LogMessage?.Invoke(message, newLine);
        }

        protected void OnProgressIncrement(int incrementValue)
        {
            ProgressIncrement?.Invoke(incrementValue);
        }

        protected void OnProgressChange(int currentProgress, int maximumProgress)
        {
            ProgressChange?.Invoke(currentProgress, maximumProgress);
        }

        protected void OnWorkStart(string workName)
        {
            WorkStart?.Invoke(workName);
        }
    }
}
