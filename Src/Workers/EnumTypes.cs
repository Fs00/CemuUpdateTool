using System;
using System.Windows.Forms;

namespace CemuUpdateTool.Workers
{
    public enum WorkOutcome
    {
        Success,
        CompletedWithErrors,
        Aborted,
        CancelledByUser
    }

    public enum LogMessageType
    {
        Information,
        Warning,
        Error
    }

    public enum ErrorHandlingDecision
    {
        AbortOrCancel,
        Retry,
        Ignore
    }

    static class WorkerEnumExtensions
    {
        public static ErrorHandlingDecision ToErrorHandlingDecision(this DialogResult result)
        {
            switch (result)
            {
                case DialogResult.Cancel:
                case DialogResult.Abort:
                    return ErrorHandlingDecision.AbortOrCancel;
                case DialogResult.Ignore:
                    return ErrorHandlingDecision.Ignore;
                case DialogResult.Retry:
                    return ErrorHandlingDecision.Retry;
                default:
                    throw new ArgumentException($"Can't convert DialogResult {result.ToString()} to an ErrorHandlingDecision value.");
            }
        }
    }
}
