using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
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
         *  Calculates the size of all the files contained in the given directory and its subdirectories
         */
        public static long CalculateDirSize(string folderPath, LogMessageHandler LogMessage)
        {
            long dirSize = 0;

            // Check if target folder exists, if not exit
            if (!Directory.Exists(folderPath))
            {
                LogMessage($"Unable to find folder {folderPath}. It will be skipped.", EventLogEntryType.Warning);
                return 0;
            }

            var directory = new DirectoryInfo(folderPath);

            // Calculate files sizes
            foreach (FileInfo file in directory.GetFiles())
                dirSize += file.Length;
            // Calculate subdirectories sizes recursively
            foreach (DirectoryInfo subdir in directory.GetDirectories())
                dirSize += CalculateDirSize(Path.Combine(folderPath, subdir.Name), LogMessage);

            return dirSize;
        }

        /*
         *  Copies a directory recursively to a destination folder (can be unexisting)
         *  Reports progress and keeps track of the created files and directories.
         */
        public static void CopyDirectory(string srcFolderPath, string destFolderPath, LogMessageHandler LogMessage, CancellationToken? cToken = null,
                                         IProgress<long> progressHandler = null, List<FileInfo> createdFiles = null, List<DirectoryInfo> createdDirectories = null)
        {
            DirectoryInfo sourceDir = new DirectoryInfo(srcFolderPath);

            // Check if destination folder exists, if not create it
            if (!Directory.Exists(destFolderPath))
            {
                var newFolder = Directory.CreateDirectory(destFolderPath);
                createdDirectories?.Add(newFolder);
            }

            // Copy files
            foreach (FileInfo file in sourceDir.GetFiles())
            {
                cToken?.ThrowIfCancellationRequested();
                file.CopyTo(Path.Combine(destFolderPath, file.Name), LogMessage, progressHandler, createdFiles);
            }

            // Copy subdirs recursively
            foreach (DirectoryInfo subdir in sourceDir.GetDirectories())
                CopyDirectory(Path.Combine(srcFolderPath, subdir.Name), Path.Combine(destFolderPath, subdir.Name), LogMessage, cToken, progressHandler, createdFiles, createdDirectories);
        }

        /*
         *  Extension method to reuse advanced file copy logic in different parts of the program
         */
        public static void CopyTo(this FileInfo srcFile, string destFilePath, LogMessageHandler LogMessage, IProgress<long> progressHandler = null, List<FileInfo> createdFiles = null)
        {
            LogMessage($"Copying {srcFile.FullName}... ", EventLogEntryType.Information, false);
            bool copySuccessful = false;
            while (!copySuccessful)
            {
                try
                {
                    FileInfo destinationFile = srcFile.CopyTo(destFilePath, true);
                    copySuccessful = true;
                    createdFiles?.Add(destinationFile);    // add to the list of copied files the destination file
                }
                catch (Exception exc)
                {
                    DialogResult choice = MessageBox.Show($"Unexpected error when copying file {srcFile.Name}: {exc.Message} What do you want to do?",
                        "Error during file copy", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);

                    if (choice == DialogResult.Abort)
                        throw;
                    else if (choice == DialogResult.Ignore)
                    {
                        LogMessage($"{srcFile.Name} not copied: {exc.Message}", EventLogEntryType.Error);
                        break;
                    }
                }
            }
            progressHandler?.Report(srcFile.Length);    // notify to the form that the current file has been copied
            if (copySuccessful)
                LogMessage("Done!", EventLogEntryType.Information);
        }

        /*
         *  Method that deletes the contents (including subfolders recursively) of the passed folder without deleting the folder itself
         *  Return the DirectoryInfo corresponding to the folder being emptied, so that the previous recursive call can delete it
         */
        public static DirectoryInfo RemoveDirectoryContents(string folderPath, LogMessageHandler LogMessage, CancellationToken? cToken = null)
        {
            DirectoryInfo directory = new DirectoryInfo(folderPath);

            // Check if destination folder exists, if not throw exception
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"Directory {folderPath} doesn't exist!");

            bool fullyEmptied = true;
            // Delete files
            foreach (FileInfo file in directory.GetFiles())
            {
                cToken?.ThrowIfCancellationRequested();     // check for cancellation

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
                        DialogResult choice = MessageBox.Show($"Unexpected error when deleting file {file.Name}: {exc.Message}" +
                            " Do you want to retry, ignore file or skip folder contents removal?",
                            "Error during file deletion", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);

                        if (choice == DialogResult.Abort)
                            throw;
                        else if (choice == DialogResult.Ignore)
                        {
                            LogMessage($"Unable to delete {file.Name}: {exc.Message}", EventLogEntryType.Error);
                            fullyEmptied = false;
                            break;
                        }
                    }
                }
            }

            // Delete subdirs recursively
            foreach (DirectoryInfo subdir in directory.GetDirectories())
            {
                var emptiedFolder = RemoveDirectoryContents(Path.Combine(folderPath, subdir.Name), LogMessage, cToken);
                if (IsDirectoryEmpty(emptiedFolder.FullName))
                    emptiedFolder.Delete();
                else
                    fullyEmptied = false;
            }

            if (fullyEmptied)
                LogMessage($"Contents of directory {folderPath} removed successfully.", EventLogEntryType.Information);
            else
                LogMessage($"Contents of directory {folderPath} removed partially due to some errors.", EventLogEntryType.Information);

            return directory;
        }

        /*
         *  Extracts all the contents of a given Zip file in the same directory as the archive, keeping its internal folder structure.
         *  Returns the path to the folder where the archive is extracted.
         */
        public static string ExtractZipArchiveInSameDirectory(string zipPath, LogMessageHandler LogMessage, CancellationToken? cToken = null,
                                                              List<FileInfo> createdFiles = null, List<DirectoryInfo> createdDirectories = null)
        {
            string extractionPath = Path.GetDirectoryName(zipPath);
            ZipArchive archive = null;

            // Open the archive file with read permission
            bool archiveOpeningSuccessful = false;
            while (!archiveOpeningSuccessful)
            {
                try
                {
                    archive = ZipFile.OpenRead(zipPath);
                    archiveOpeningSuccessful = true;
                }
                catch (Exception exc)
                {
                    DialogResult choice = MessageBox.Show($"Unexpected error when trying to open Zip file {Path.GetFileName(zipPath)}: {exc.Message} " +
                                    "Do you want to retry or cancel the operation?", "Error during Zip archive opening", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

                    if (choice == DialogResult.Cancel)
                        throw;
                }
            }

            using (archive)
            {
                // Extract all archive files
                bool entryWrittenSuccessfully = false;
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    cToken?.ThrowIfCancellationRequested();
                    entryWrittenSuccessfully = false;
                    while (!entryWrittenSuccessfully)
                    {
                        try
                        {
                            string entryRelativePath = entry.FullName.Replace('/', Path.DirectorySeparatorChar);    // replace all occurrencies of '/' with '\'
                            string extractedFilePath = Path.Combine(extractionPath, entryRelativePath);

                            // If the entry is a folder, create it
                            if (entryRelativePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                            {
                                var newFolder = Directory.CreateDirectory(extractedFilePath);
                                createdDirectories?.Add(newFolder);
                            }
                            // Otherwise, if it's a file, extract it
                            else
                            {
                                entry.ExtractToFile(extractedFilePath, true);
                                //LogMessage($"Entry {entry.Name} extracted successfully in {Path.GetDirectoryName(extractedFilePath)}.", EventLogEntryType.Information);
                                createdFiles?.Add(new FileInfo(extractedFilePath));
                            }
                            entryWrittenSuccessfully = true;
                        }
                        catch (Exception exc)
                        {
                            DialogResult choice = MessageBox.Show($"Unexpected error when extracting file {entry.Name} from {Path.GetFileName(zipPath)}: {exc.Message}" +
                                                  "What do you want to do?", "Error during file extraction", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);

                            if (choice == DialogResult.Abort)
                                throw;
                            else if (choice == DialogResult.Ignore)
                            {
                                LogMessage($"Unable to extract file {entry.Name}: {exc.Message}", EventLogEntryType.Error);
                                break;
                            }
                        }
                    }
                }
            }
            return extractionPath;
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
