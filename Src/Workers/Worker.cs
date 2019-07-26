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
            string message = "Unexpected error when ";
            switch (operationInfo)
            {
                case FileCopyOperationInfo fileCopyInfo:
                    return message + $"copying file {fileCopyInfo.CopiedFile.Name}: {errorMessage} What do you want to do?";
                case FileDeletionOperationInfo fileDeletionInfo:
                    return message + $"deleting file {fileDeletionInfo.FileToDelete.Name}: {errorMessage} " +
                                     "Do you want to retry, ignore file or skip folder contents removal?";
                case FileExtractionOperationInfo fileExtractionInfo:
                    return message + $"extracting file {fileExtractionInfo.FileToExtract} from " +
                                     $"{fileExtractionInfo.ZipArchiveName}: {errorMessage} What do you want to do?";
                default:
                    return message + $"performing an unknown operation: {errorMessage}";
            }
        }

        private string GetErrorDialogTitleForOperation(OperationInfo operationInfo)
        {
            string dialogTitle = "Error during ";
            switch (operationInfo)
            {
                case FileCopyOperationInfo _:
                    return dialogTitle + "file copy";
                case FileDeletionOperationInfo _:
                    return dialogTitle + "file deletion";
                case FileExtractionOperationInfo _:
                    return dialogTitle + "file extraction";
                default:
                    return dialogTitle + "unknown operation";
            }
        }

        public virtual void OnOperationStart(OperationInfo operationInfo)
        {
            switch (operationInfo)
            {
                case FileCopyOperationInfo fileCopyInfo:
                    OnLogMessage(LogMessageType.Information, $"Copying {fileCopyInfo.CopiedFile.FullName}... ", false);
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
            switch (operationInfo)
            {
                case FileCopyOperationInfo fileCopyInfo:
                    OnLogMessage(LogMessageType.Error, $"{fileCopyInfo.CopiedFile.Name} not copied: {errorMessage}");
                    break;
                case FileDeletionOperationInfo fileDeletionInfo:
                    OnLogMessage(LogMessageType.Error, $"Unable to delete {fileDeletionInfo.FileToDelete.Name}: {errorMessage}");
                    break;
                case FileExtractionOperationInfo fileExtractionInfo:
                    OnLogMessage(LogMessageType.Error, $"Unable to extract file {fileExtractionInfo.FileToExtract}: {errorMessage}");
                    break;
            }
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
