using System;
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
                worker?.OnOperationSuccess(new DirectoryCreationOperationInfo(newFolder));
            }

            foreach (FileInfo file in sourceDirectory.GetFiles())
            {
                worker?.ThrowIfWorkIsCancelled();
                file.CopyTo(Path.Combine(destinationDirectoryPath, file.Name), worker);
            }

            foreach (DirectoryInfo subDirectory in sourceDirectory.GetDirectories())
                CopyDirectory(
                    Path.Combine(sourceDirectoryPath, subDirectory.Name),
                    Path.Combine(destinationDirectoryPath, subDirectory.Name),
                    worker
                );
        }

        /*
         *  Extension method to reuse advanced file copy logic in different parts of the program
         */
        public static void CopyTo(this FileInfo sourceFile, string destinationFilePath, Worker worker = null)
        {
            var sourceFileCopyInfo = new FileCopyOperationInfo(sourceFile);
            worker?.OnOperationStart(sourceFileCopyInfo);

            bool copySuccessful = false;
            while (!copySuccessful)
            {
                try
                {
                    FileInfo destinationFile = sourceFile.CopyTo(destinationFilePath, true);
                    copySuccessful = true;
                    worker?.OnOperationSuccess(new FileCopyOperationInfo(destinationFile));
                }
                catch (Exception exc)
                {
                    if (worker == null)
                        throw;

                    ErrorHandlingDecision decision = worker.DecideHowToHandleError(sourceFileCopyInfo, exc.Message);
                    if (decision == ErrorHandlingDecision.Abort)
                        throw;
                    else if (decision == ErrorHandlingDecision.Ignore)
                        break;
                }
            }
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

            // Delete files
            foreach (FileInfo file in directory.GetFiles())
            {
                worker?.ThrowIfWorkIsCancelled();
                TryDeleteFile(file, worker);
            }

            // Delete subdirs recursively
            foreach (DirectoryInfo subdir in directory.GetDirectories())
            {
                var emptiedFolder = RemoveDirectoryContents(Path.Combine(path, subdir.Name), worker);
                if (IsDirectoryEmpty(emptiedFolder.FullName))
                    emptiedFolder.Delete();
            }

            return directory;
        }

        private static void TryDeleteFile(FileInfo file, Worker worker)
        {
            bool deletionSuccessful = false;
            while (!deletionSuccessful)
            {
                try
                {
                    file.Delete();
                    deletionSuccessful = true;
                }
                catch (Exception exc)
                {
                    if (worker == null)
                        throw;

                    ErrorHandlingDecision decision = worker.DecideHowToHandleError(new FileDeletionOperationInfo(file), exc.Message);
                    if (decision == ErrorHandlingDecision.Abort)
                        throw;
                    else if (decision == ErrorHandlingDecision.Ignore)
                        break;
                }
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

                    bool entryExtractedSuccessfully = false;
                    while (!entryExtractedSuccessfully)
                    {
                        try
                        {
                            zipEntry.ExtractTo(archiveExtractionPath);
                            entryExtractedSuccessfully = true;
                        }
                        catch (Exception exc)
                        {
                            if (worker == null)
                                throw;

                            ErrorHandlingDecision decision = worker.DecideHowToHandleError(
                                new FileExtractionOperationInfo(zipEntry.Name, Path.GetFileName(zipPath)), exc.Message
                            );
                            if (decision == ErrorHandlingDecision.Abort)
                                throw;
                            else if (decision == ErrorHandlingDecision.Ignore)
                                break;
                        }
                    }
                }
            }
            return archiveExtractionPath;
        }

        private static void ExtractTo(this ZipArchiveEntry zipEntry, string extractionRootFolder)
        {
            string entryRelativePath = zipEntry.FullName.Replace('/', Path.DirectorySeparatorChar);
            string entryExtractionPath = Path.Combine(extractionRootFolder, entryRelativePath);

            if (zipEntry.IsDirectory())
                Directory.CreateDirectory(entryExtractionPath);
            else
                zipEntry.ExtractToFile(entryExtractionPath, overwrite: true);
        }

        private static bool IsDirectory(this ZipArchiveEntry entry)
        {
            return entry.FullName.EndsWith("/");
        }

        /*
         *  Helper method to determine whether a folder contains a Cemu installation
         */
        public static bool IsValidCemuInstallation(string path, out string reason)
        {
            reason = null;
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
                reason = "Directory does not exist";
            else if (!File.Exists(Path.Combine(path, "Cemu.exe")))
                reason = "Not a valid Cemu installation (Cemu.exe is missing)";

            return reason == null;
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
