using System;
using System.Threading;
using System.Windows.Forms;
using CemuUpdateTool.Workers.Operations;

namespace CemuUpdateTool.Workers
{
    /*
     * Represents a class that performs some work, which is made up of single operations.
     * Provides API for progress reporting, operations outcome notification, work cancellation and error recovery.
     */
    public abstract class Worker
    {
        public int ErrorsEncountered { private set; get; }

        public event Action<string> WorkStart;
        public event Action<string, bool> LogMessage;
        public event Action<int, int> ProgressChange;
        public event Action<int> ProgressIncrement;

        private CancellationToken cancToken;

        protected Worker(CancellationToken cancToken)
        {
            this.cancToken = cancToken;
        }

        public void ThrowIfWorkIsCancelled()
        {
            cancToken.ThrowIfCancellationRequested();
        }

        public ErrorHandlingDecision DecideHowToHandleError(RetryableOperation operationInfo, Exception error)
        {
            if (error is OperationCanceledException)
                return ErrorHandlingDecision.AbortOrCancel;
            
            ErrorHandlingDecision decision = AskUserHowToHandleError(operationInfo, error);
            if (decision == ErrorHandlingDecision.Ignore)
            {
                OnOperationErrorHandled(operationInfo, error.Message);
                ErrorsEncountered++;
            }

            return decision;
        }

        protected virtual ErrorHandlingDecision AskUserHowToHandleError(RetryableOperation operationInfo, Exception error)
        {
            string dialogMessage = operationInfo.BuildMessageForErrorHandlingDecision(error);
            string dialogTitle = "Error during " + operationInfo.OperationName;

            DialogResult choice = MessageBox.Show(dialogMessage, dialogTitle,
                                                  MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
            return choice.ToErrorHandlingDecision();
        }

        public virtual void OnOperationStart(Operation operationInfo)
        {
            switch (operationInfo)
            {
                case FileCopyOperation fileCopyInfo:
                    OnLogMessage(LogMessageType.Information, $"Copying {fileCopyInfo.SourceFile.FullName}... ", false);
                    break;
            }
        }

        public virtual void OnOperationSuccess(Operation operationInfo)
        {
            switch (operationInfo)
            {
                case FileCopyOperation _:
                    OnLogMessage(LogMessageType.Information, "Done!");
                    break;
            }
        }

        public virtual void OnOperationErrorHandled(Operation operationInfo, string errorMessage)
        {
            OnLogMessage(LogMessageType.Error, operationInfo.BuildFailureMessage(errorMessage));
        }

        protected void OnLogMessage(LogMessageType type, string message, bool newLine = true)
        {
            string logMessage = "";
            if (type == LogMessageType.Error)
                logMessage += "ERROR: ";
            else if (type == LogMessageType.Warning)
                logMessage += "WARNING: ";

            logMessage += message;
            LogMessage?.Invoke(logMessage, newLine);
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
