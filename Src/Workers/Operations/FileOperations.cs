using System;
using System.IO;
using System.IO.Compression;
using CemuUpdateTool.Utils;

namespace CemuUpdateTool.Workers.Operations
{
    class FileCopyOperation : RetryableOperation
    {
        public FileInfo SourceFile { get; }
        public FileInfo DestinationFile { get; }

        public override string OperationName => "file copy";

        public FileCopyOperation(FileInfo sourceFile, FileInfo destinationFile)
        {
            SourceFile = sourceFile;
            DestinationFile = destinationFile;
        }

        public override string BuildMessageForErrorHandlingDecision(Exception error)
        {
            return MESSAGE_FOR_ERROR_HANDLING_DECISION_START +
                   $"copying file {SourceFile.Name}: {error.Message} What do you want to do?";
        }

        public override string BuildFailureMessage(string failureReason)
        {
            return $"{SourceFile.Name} not copied: {failureReason}";
        }

        protected override void PerformOperationLogic()
        {
            SourceFile.CopyTo(DestinationFile.FullName, overwrite: true);
        }
    }

    class FileDeletionOperation : RetryableOperation
    {
        public FileInfo FileToDelete { get; }

        public override string OperationName => "file deletion";

        public FileDeletionOperation(FileInfo fileToDelete)
        {
            FileToDelete = fileToDelete;
        }

        public override string BuildMessageForErrorHandlingDecision(Exception error)
        {
            return MESSAGE_FOR_ERROR_HANDLING_DECISION_START +
                   $"deleting file {FileToDelete.Name}: {error.Message} " +
                   "Do you want to retry, ignore file or skip folder contents removal?";
        }

        public override string BuildFailureMessage(string failureReason)
        {
            return $"Unable to delete {FileToDelete.Name}: {failureReason}";
        }

        protected override void PerformOperationLogic()
        {
            FileToDelete.Delete();
        }
    }

    class FileOrDirectoryExtractionOperation : RetryableOperation
    {
        public ZipArchiveEntry EntryToExtract { get; }
        public string ExtractionRootFolder { get; }
        public string ZipArchiveName { get; }

        public override string OperationName => "file extraction";

        public FileOrDirectoryExtractionOperation(ZipArchiveEntry entryToExtract, string extractionRootFolder, string zipArchiveName)
        {
            EntryToExtract = entryToExtract;
            ExtractionRootFolder = extractionRootFolder;
            ZipArchiveName = zipArchiveName;
        }

        public override string BuildMessageForErrorHandlingDecision(Exception error)
        {
            return MESSAGE_FOR_ERROR_HANDLING_DECISION_START +
                   $"extracting file/directory {EntryToExtract} from {ZipArchiveName}: {error.Message} What do you want to do?";
        }

        public override string BuildFailureMessage(string failureReason)
        {
            return $"Unable to extract file {EntryToExtract}: {failureReason}";
        }

        protected override void PerformOperationLogic()
        {
            EntryToExtract.ExtractTo(ExtractionRootFolder);
        }
    }
}