using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CemuUpdateTool
{
    public static class FileUtils
    {
        /*
         *  Method that calculates the size of all the files contained in the passed directory and its subdirectories, then returns it
         */
        public static long CalculateDirSize(string folderPath)
        {
            long dirSize = 0;

            // Check if target folder exists, if not exit
            if (!DirectoryExists(folderPath))
                return 0;

            // Retrieve informations for files and subdirectories
            DirectoryInfo dirToCompute = new DirectoryInfo(folderPath);
            DirectoryInfo[] subdirsArray = dirToCompute.GetDirectories();
            FileInfo[] filesArray = dirToCompute.GetFiles();

            foreach (FileInfo file in filesArray)               // calculate files sizes
                dirSize += file.Length;

            foreach (DirectoryInfo subdir in subdirsArray)      // calculate subdirs sizes recursively
                dirSize += CalculateDirSize(Path.Combine(folderPath, subdir.Name));

            return dirSize;
        }

        /*
         *  Method that copies a Cemu subdir from old installation to new one.
         *  Sends callbacks to MigrationForm in order to update progress bars.
         */
        public static void CopyDir(string srcFolderPath, string destFolderPath, CancellationToken? cToken, IProgress<long> progressHandler, Action<string, bool> LogMessage,
                                   Action ReportError, List<FileInfo> createdFiles = null, List<DirectoryInfo> createdDirectories = null)
        {
            // Retrieve informations for files and subdirectories
            DirectoryInfo sourceDir = new DirectoryInfo(srcFolderPath);
            DirectoryInfo[] srcSubdirsArray = sourceDir.GetDirectories();
            FileInfo[] srcFilesArray = sourceDir.GetFiles();

            // Check if destination folder exists, if not create it
            if (!DirectoryExists(destFolderPath))
                createdDirectories?.Add(Directory.CreateDirectory(destFolderPath));     // TODO: verificare se CreateDirectory viene eseguito quando createdDirectories è null

            // Copy files
            foreach (FileInfo file in srcFilesArray)
            {
                cToken?.ThrowIfCancellationRequested();
                bool copySuccessful = false;
                FileInfo destinationFile;

                LogMessage($@"Copying {file.Directory.Name}\{file.Name}... ", false);      // TODO: to be improved
                string destFilePath = Path.Combine(destFolderPath, file.Name);
                while (!copySuccessful)
                {
                    try
                    {
                        destinationFile = file.CopyTo(destFilePath, true);
                        copySuccessful = true;
                        createdFiles?.Add(destinationFile);    // add to the list of copied files the destination file
                    }
                    catch(Exception exc)
                    {
                        DialogResult choice = MessageBox.Show($"Unexpected error when copying file {file.Name}: {exc.Message} What do you want to do?",
                            "Error during file copy", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);

                        if (choice == DialogResult.Abort)
                            throw;
                        else if (choice == DialogResult.Ignore)
                        {
                            ReportError();
                            break;
                        }
                    }
                }
                progressHandler.Report(file.Length);    // notify to the form that the current file has been copied
                LogMessage("Done!", true);
            }

            // Copy subdirs recursively
            foreach (DirectoryInfo subdir in srcSubdirsArray)
                CopyDir(Path.Combine(srcFolderPath, subdir.Name), Path.Combine(destFolderPath, subdir.Name), cToken, progressHandler, LogMessage, ReportError, createdFiles, createdDirectories);
        }

        /*
         *  Method that deletes the contents of the passed folder without deleting the folder itself
         *  If this method fails, it doesn't throw any exception but simply quits reporting the error to the Worker
         */
        public static void RemoveDirContents(string folderPath, CancellationToken? cToken, Action ReportError)
        {
            // Retrieve informations for files and subdirectories
            DirectoryInfo directory = new DirectoryInfo(folderPath);
            DirectoryInfo[] subdirsArray = directory.GetDirectories();
            FileInfo[] filesArray = directory.GetFiles();

            // Check if destination folder exists, if not throw exception
            if (!DirectoryExists(folderPath))
                throw new DirectoryNotFoundException("Directory doesn't exist!");

            // Delete files
            foreach (FileInfo file in filesArray)
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
                            " Do you want to retry or skip folder contents removal?",
                            "Error during file deletion", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

                        if (choice == DialogResult.Cancel)
                        {
                            ReportError();
                            return;
                        }
                    }
                }
            }

            // Delete subdirs recursively
            foreach (DirectoryInfo subdir in subdirsArray)  // TODO: in questo modo le sottocartelle non vengono cancellate?
                RemoveDirContents(Path.Combine(folderPath, subdir.Name), cToken, ReportError);
        }

        /*
         *  Extracts all the contents of a given Zip file in the same directory as the archive, keeping its internal folder structure
         *      
         *  TODO: callback per MigrationForm?
         */
        public static void ExtractZipFileContents(string zipPath, CancellationToken? cToken, Action ReportError,
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

                    if (choice == DialogResult.Retry)
                        continue;
                    else if (choice == DialogResult.Cancel)
                        throw;
                }
            }

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
                            createdDirectories?.Add(Directory.CreateDirectory(extractedFilePath));
                        // Otherwise, if it's a file, extract it
                        else
                        {
                            entry.ExtractToFile(extractedFilePath, true);
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
                            ReportError();
                            break;
                        }
                    }
                }
            }
        }

        /*
         * Custom case-sensitive implementations of File.Exists() and Directory.Exists() -- based on original solution by Eric Bole-Feysot
         */
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetLongPathName(string path, StringBuilder longPath, int longPathLength);
        public static bool FileExists(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("Empty argument!");

            StringBuilder longPath = new StringBuilder(255);
            if (GetLongPathName(Path.GetDirectoryName(filePath), longPath, longPath.Capacity) == 0) // if file's directory doesn't exist, file doesn't exist!
                return false;

            return Array.Exists(Directory.GetFiles(longPath.ToString()), s => s == filePath);
        }

        public static bool DirectoryExists(string dirPath)
        {
            if (string.IsNullOrEmpty(dirPath))
                throw new ArgumentException("Empty argument!");

            StringBuilder longPath = new StringBuilder(255);
            if (GetLongPathName(dirPath, longPath, longPath.Capacity) != 0 && dirPath == longPath.ToString())
                return true;
            else
                return false;
        }
    }
}
