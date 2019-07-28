using System.IO;
using System.IO.Compression;
using System.Linq;
using CemuUpdateTool.Workers;

namespace CemuUpdateTool.Utils
{
    /*
     *  FileUtils
     *  Static class which contains helper methods to operate on files, directories and zip archives.
     *  Methods are designed to optionally log events, keep track of operation progress and be cancelled through a Worker.
     */
    public static class FileUtils
    {
        /*
         *  Copies a directory recursively to a destination folder (can be unexisting)
         */
        public static void CopyDirectory(string sourceDirectoryPath, string destinationDirectoryPath, Worker worker = null)
        {
            DirectoryInfo sourceDirectory = new DirectoryInfo(sourceDirectoryPath);
            if (!Directory.Exists(destinationDirectoryPath))
            {
                var newFolder = Directory.CreateDirectory(destinationDirectoryPath);
                worker?.OnOperationSuccess(new DirectoryCreationOperation(newFolder));
            }

            sourceDirectory.CopyAllFilesTo(destinationDirectoryPath, worker);
            sourceDirectory.CopyAllSubDirectoriesTo(destinationDirectoryPath, worker);
        }

        private static void CopyAllSubDirectoriesTo(this DirectoryInfo sourceDirectory, string destinationDirectoryPath, Worker worker = null)
        {
            foreach (DirectoryInfo subDirectory in sourceDirectory.GetDirectories())
                CopyDirectory(
                    Path.Combine(sourceDirectory.FullName, subDirectory.Name),
                    Path.Combine(destinationDirectoryPath, subDirectory.Name),
                    worker
                );
        }

        private static void CopyAllFilesTo(this DirectoryInfo sourceDirectory, string destinationDirectoryPath, Worker worker = null)
        {
            foreach (FileInfo file in sourceDirectory.GetFiles())
            {
                worker?.ThrowIfWorkIsCancelled();
                file.CopyTo(Path.Combine(destinationDirectoryPath, file.Name), worker);
            }
        }

        /*
         *  Extension method to reuse advanced file copy logic in different parts of the program
         */
        public static void CopyTo(this FileInfo sourceFile, string destinationFilePath, Worker worker = null)
        {
            FileInfo destinationFile = new FileInfo(destinationFilePath);
            var fileCopyOperation = new FileCopyOperation(sourceFile, destinationFile);
            worker?.OnOperationStart(fileCopyOperation);

            if (worker == null)
                fileCopyOperation.Perform();
            else
                fileCopyOperation.RetryUntilSuccessOrCancellationByWorker(worker);
        }

        /*
         *  Method that deletes the contents (including subfolders recursively) of the passed folder without deleting the folder itself.
         *  Return the DirectoryInfo corresponding to the folder being emptied, so that the previous recursive call can delete it
         */
        public static DirectoryInfo RemoveDirectoryContents(string path, Worker worker = null)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            if (!directory.Exists)
                throw new DirectoryNotFoundException($"Directory {path} doesn't exist!");

            directory.RemoveAllFiles(worker);
            directory.RemoveAllSubdirectories(worker);

            return directory;
        }

        private static void RemoveAllFiles(this DirectoryInfo directory, Worker worker = null)
        {
            foreach (FileInfo file in directory.GetFiles())
            {
                worker?.ThrowIfWorkIsCancelled();

                var fileDeletionOperation = new FileDeletionOperation(file);
                if (worker == null)
                    fileDeletionOperation.Perform();
                else
                    fileDeletionOperation.RetryUntilSuccessOrCancellationByWorker(worker);
            }
        }

        private static void RemoveAllSubdirectories(this DirectoryInfo directory, Worker worker)
        {
            foreach (DirectoryInfo subdir in directory.GetDirectories())
            {
                var emptiedFolder = RemoveDirectoryContents(Path.Combine(directory.FullName, subdir.Name), worker);
                if (IsDirectoryEmpty(emptiedFolder.FullName))
                    emptiedFolder.Delete();
            }
        }

        /*
         *  Extracts all the contents of a given Zip file in the same directory as the archive, keeping its internal folder structure.
         *  Returns the path to the folder where the archive is extracted.
         */
        public static string ExtractZipArchiveInSameDirectory(string zipPath, Worker worker = null)
        {
            string archiveExtractionPath = Path.GetDirectoryName(zipPath);
            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                foreach (ZipArchiveEntry zipEntry in archive.Entries)
                {
                    worker?.ThrowIfWorkIsCancelled();

                    var entryExtractionOperation = new FileOrDirectoryExtractionOperation(zipEntry, archiveExtractionPath, Path.GetFileName(zipPath));
                    if (worker == null)
                        entryExtractionOperation.Perform();
                    else
                        entryExtractionOperation.RetryUntilSuccessOrCancellationByWorker(worker);
                }
            }
            return archiveExtractionPath;
        }

        public static void ExtractTo(this ZipArchiveEntry zipEntry, string extractionRootFolder)
        {
            string entryRelativePath = zipEntry.FullName.Replace('/', Path.DirectorySeparatorChar);
            string entryExtractionPath = Path.Combine(extractionRootFolder, entryRelativePath);

            if (zipEntry.IsDirectory())
                Directory.CreateDirectory(entryExtractionPath);
            else
                zipEntry.ExtractToFile(entryExtractionPath, overwrite: true);
        }

        public static bool IsDirectory(this ZipArchiveEntry entry)
        {
            return entry.FullName.EndsWith("/");
        }

        /*
         *  Checks if a given directory has no elements inside.
         */
        public static bool IsDirectoryEmpty(string dirPath)
        {
            return !Directory.EnumerateFileSystemEntries(dirPath).Any();
        }
    }
}
