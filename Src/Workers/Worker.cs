using System;
using System.Diagnostics;
using System.Threading;

namespace CemuUpdateTool.Workers
{
    public delegate void LogMessageHandler(string message, EventLogEntryType type, bool newLine = true);

    public enum WorkOutcome
    {
        Success,
        CompletedWithErrors,
        Aborted,
        CancelledByUser
    }

    abstract class Worker
    {
        // Counts the number of errors
        public int ErrorsEncountered      { private set; get; }

        protected CancellationToken cancToken;
        private Action<string, bool> LoggerDelegate;    // callback that writes a message on an external log (in this case OperationsForm textbox)

        public Worker(CancellationToken cancToken, Action<string, bool> LoggerDelegate)
        {
            this.cancToken = cancToken;
            this.LoggerDelegate = LoggerDelegate;
        }

        /*
         *  Callback that handles a log message given its type (warning, info, error etc.).
         *  Through this callback, methods called by the Worker can notify errors.
         */
        protected void HandleLogMessage(string message, EventLogEntryType type, bool newLine = true)
        {
            string logMessage = "";
            if (type == EventLogEntryType.Error || type == EventLogEntryType.FailureAudit)
            {
                // if it's an error message, update errors encountered counter
                ErrorsEncountered++;
                logMessage += "ERROR: ";
            }
            else if (type == EventLogEntryType.Warning)
                logMessage += "WARNING: ";

            logMessage += message;
            LoggerDelegate(logMessage, newLine);   // pass the message to the form
        }
    }
}
