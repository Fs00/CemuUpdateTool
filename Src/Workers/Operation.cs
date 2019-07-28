using System.IO;

namespace CemuUpdateTool.Workers
{
    public abstract class Operation
    {
        public bool IsCompleted { private set; get; }
        public abstract string OperationName { get; }

        protected abstract void PerformOperationLogic();
        public abstract string BuildFailureMessage(string failureReason);

        public void Perform()
        {
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
