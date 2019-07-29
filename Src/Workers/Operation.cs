using System;
using System.IO;

namespace CemuUpdateTool.Workers
{
    /*
     * Represents a generic operation that doesn't return a result.
     * It is meant as a way to store basic information about an operation and pass them around,
     * so that other classes (typically a Worker) can display a detailed error message about the operation
     * without knowing its details.
     * Furthermore, subclasses have the chance to implement reusable logic around the Perform() method
     * (see RetryableOperation for an example).
     */
    public abstract class Operation
    {
        public bool IsCompleted { private set; get; }
        public abstract string OperationName { get; }

        protected abstract void PerformOperationLogic();
        public abstract string BuildFailureMessage(string failureReason);

        public void Perform()
        {
            if (IsCompleted)
                throw new InvalidOperationException("The operation is already completed.");

            PerformOperationLogic();
            IsCompleted = true;
        }
    }

    class DirectoryCreationOperation : Operation
    {
        public DirectoryInfo DirectoryToCreate { get; }

        public override string OperationName => "directory creation";

        public DirectoryCreationOperation(DirectoryInfo directoryToCreate)
        {
            DirectoryToCreate = directoryToCreate;
        }

        public override string BuildFailureMessage(string failureReason)
        {
            return $"Unable to create directory {DirectoryToCreate.Name}: {failureReason}";
        }

        protected override void PerformOperationLogic()
        {
            DirectoryToCreate.Create();
        }
    }
}
