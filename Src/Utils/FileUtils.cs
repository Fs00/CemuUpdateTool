using System;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using System.Linq;
using CemuUpdateTool.Workers;

namespace CemuUpdateTool.Utils
{
    /*
     *  FileUtils
     *  Static class which contains helper methods to operate on files, directories and zip archives
     *  Methods are designed to optionally log events, keep track of the files created and be cancelled
     */
    public static class FileUtils
    {
        /*
         *  Copies a directory recursively to a destination folder (can be unexisting)
         *  Reports optionally progress to a Worker.
         */
        public static void CopyDirectory(string sourceDirectoryPath, string destinationDirectoryPath, Worker worker = null)
        {
            DirectoryInfo sourceDirectory = new DirectoryInfo(sourceDirectoryPath);

            // Check if destination folder exists, if not create it
            if (!Directory.Exists(destinationDirectoryPath))
            {
                var newFolder = Directory.CreateDirectory(destinationDirectoryPath);
                worker?.OnOperationSuccess(new DirectoryCreationOperationInfo(newFolder));
            }

            // Copy files
            foreach (FileInfo file in sourceDirectory.GetFiles())
            {
                worker?.ThrowIfWorkIsCancelled();
                file.CopyTo(Path.Combine(destinationDirectoryPath, file.Name), worker);
            }

            // Copy subdirs recursively
            foreach (DirectoryInfo subdir in sourceDirectory.GetDirectories())
                CopyDirectory(Path.Combine(sourceDirectoryPath, subdir.Name), Path.Combine(destinationDirectoryPath, subdir.Name), worker);
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
                    /*DialogResult choice = MessageBox.Show($"Unexpected error when copying file {sourceFile.Name}: {exc.Message} What do you want to do?",
                        "Error during file copy", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);*/
                    if (decision == ErrorHandlingDecision.Abort)
                        throw;
                    else if (decision == ErrorHandlingDecision.Ignore)
                        //LogMessage($"{sourceFile.Name} not copied: {exc.Message}", LogMessageType.Error);
                        break;
                }
            }
        }

        /*
         *  Method that deletes the contents (including subfolders recursively) of the passed folder without deleting the folder itself
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
                        /*DialogResult choice = MessageBox.Show($"Unexpected error when deleting file {file.Name}: {exc.Message}" +
                            " Do you want to retry, ignore file or skip folder contents removal?",
                            "Error during file deletion", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);*/
                        if (decision == ErrorHandlingDecision.Abort)
                            throw;
                        else if (decision == ErrorHandlingDecision.Ignore)
                            //LogMessage($"Unable to delete {file.Name}: {exc.Message}", LogMessageType.Error);
                            break;
                    }
                }
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
                            string entryRelativePath = zipEntry.FullName.Replace('/', Path.DirectorySeparatorChar);
                            string entryExtractionPath = Path.Combine(archiveExtractionPath, entryRelativePath);

                            if (zipEntry.IsDirectory())
                                Directory.CreateDirectory(entryExtractionPath);
                            else
                                zipEntry.ExtractToFile(entryExtractionPath, overwrite: true);

                            entryExtractedSuccessfully = true;
                        }
                        catch (Exception exc)
                        {
                            if (worker == null)
                                throw;

                            ErrorHandlingDecision decision = worker.DecideHowToHandleError(
                                new FileExtractionOperationInfo(zipEntry.Name, Path.GetFileName(zipPath)), exc.Message
                            );
                            /*DialogResult choice = MessageBox.Show(
                                $"Unexpected error when extracting file {zipEntry.Name} from {Path.GetFileName(zipPath)}: {exc.Message}" +
                                "What do you want to do?", "Error during file extraction",
                                MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error
                            );*/
                            if (decision == ErrorHandlingDecision.Abort)
                                throw;
                            else if (decision == ErrorHandlingDecision.Ignore)
                                //LogMessage($"Unable to extract file {zipEntry.Name}: {exc.Message}", LogMessageType.Error);
                                break;
                        }
                    }
                }
            }
            return archiveExtractionPath;
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
