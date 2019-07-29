using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using CemuUpdateTool.Utils;

namespace CemuUpdateTool.Workers.Operations
{
    /*
     * Allows to reuse common notify-and-retry logic for an operation in different and unrelated places.
     * This logic is provided by a Worker, that after a failure will ask to the user if the operation
     * must be retried or not.
     */
    public abstract class RetryableOperation : Operation
    {
        public bool WillNotBeRetried { private set; get; }

        protected const string MESSAGE_FOR_ERROR_HANDLING_DECISION_START = "Unexpected error when ";
        public abstract string BuildMessageForErrorHandlingDecision(Exception error);

        public void RetryUntilSuccessOrCancellationByWorker(Worker worker)
        {
            while (!IsCompleted && !WillNotBeRetried)
                TryPerformOperationAndNotifyOutcomeToWorker(worker);
        }

        private void TryPerformOperationAndNotifyOutcomeToWorker(Worker worker)
        {
            try
            {
                Perform();
                worker.OnOperationSuccess(this);
            }
            catch (Exception exc)
            {
                LetWorkerDecideWhetherToRetryOperation(worker, exc);
            }
        }

        private void LetWorkerDecideWhetherToRetryOperation(Worker worker, Exception exc)
        {
            ErrorHandlingDecision decision = worker.DecideHowToHandleError(this, exc);
            if (decision == ErrorHandlingDecision.AbortOrCancel)
                throw exc;
            else if (decision == ErrorHandlingDecision.Ignore)
                WillNotBeRetried = true;
        }
    }
}
