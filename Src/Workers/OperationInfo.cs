using System.IO;

namespace CemuUpdateTool.Workers
{
    public abstract class OperationInfo {}

    class FileCopyOperationInfo : OperationInfo
    {
        public FileInfo CopiedFile { get; }

        public FileCopyOperationInfo(FileInfo copiedFile)
        {
            CopiedFile = copiedFile;
        }
    }

    class FileDeletionOperationInfo : OperationInfo
    {
        public FileInfo FileToDelete { get; }

        public FileDeletionOperationInfo(FileInfo fileToDelete)
        {
            FileToDelete = fileToDelete;
        }
    }

    class FileExtractionOperationInfo : OperationInfo
    {
        public string FileToExtract { get; }
        public string ZipArchiveName { get; }

        public FileExtractionOperationInfo(string fileToExtract, string zipArchiveName)
        {
            FileToExtract = fileToExtract;
            ZipArchiveName = zipArchiveName;
        }
    }

    class DirectoryCreationOperationInfo : OperationInfo
    {
        public DirectoryInfo CreatedDirectory { get; }

        public DirectoryCreationOperationInfo(DirectoryInfo createdDirectory)
        {
            CreatedDirectory = createdDirectory;
        }
    }
}
