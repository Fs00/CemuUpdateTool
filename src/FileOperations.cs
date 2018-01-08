using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CemuUpdateTool
{
    // SET OF CALLBACKS SIGNATURES FOR THE MAINFORM
    public delegate void FileCopiedCallback(long dim);
    public delegate void FolderInfoCallback(string name, long dim);
    public delegate void ActualFileCallback(string name);

    public static class FileOperations
    {
        /*
         *  Calculates the sizes of every folder in the list using CalculateDirSize() and puts them in the worker.
         *  Returns the overall size
         */
        public static long CalculateFoldersSizes(List<string> foldersToCopy, Worker worker)
        {
            worker.foldersSizes = new List<long>();
            long overallSize = 0;

            // Calculate the size of every folder to copy
            foreach(string folder in foldersToCopy)
                worker.foldersSizes.Add(CalculateDirSize(Path.Combine(worker.baseSourcePath, folder)));

            // Calculate the overall size and return it
            foreach (long folderSize in worker.foldersSizes)
                overallSize += folderSize;

            return overallSize;
        }

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
         *  Sends callbacks to MainForm in order to update progress bars.
         */
        public static void CopyDir(string srcFolderPath, string destFolderPath, ActualFileCallback CopyingFile, FileCopiedCallback FileCopied, Worker worker)
        {
            // Retrieve informations for files and subdirectories
            DirectoryInfo sourceDir = new DirectoryInfo(srcFolderPath);
            DirectoryInfo[] srcSubdirsArray = sourceDir.GetDirectories();
            FileInfo[] srcFilesArray = sourceDir.GetFiles();

            // Check if destination folder exists, if not create it
            if (!DirectoryExists(destFolderPath))
                worker.directoriesAlreadyCopied.Add(Directory.CreateDirectory(destFolderPath));

            // Copy files
            foreach (FileInfo file in srcFilesArray)
            {
                if (!worker.isCancelled && !worker.isAborted)     // check that work hasn't been cancelled
                {
                    bool copySuccessful = false;
                    FileInfo destinationFile = null;

                    CopyingFile(file.Name);                 // Tell the MainForm the name of the file I'm about to copy
                    string destFilePath = Path.Combine(destFolderPath, file.Name);
                    while(!copySuccessful)
                    {
                        try
                        {
                            destinationFile = file.CopyTo(destFilePath, true);
                        }
                        catch(Exception exc)
                        {
                            DialogResult choice = MessageBox.Show($"Unexpected error when copying file {file.Name}: {exc.Message} What do you want to do?",
                                "Error during file copy", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);

                            if (choice == DialogResult.Retry)
                                continue;
                            else if (choice == DialogResult.Abort)
                            {
                                worker.StopWork(WorkOutcome.Aborted);
                                return;
                            }
                            else if (choice == DialogResult.Ignore)
                                worker.ErrorOccurred();
                        }
                        copySuccessful = true;
                    }
                    if (destinationFile != null)
                        worker.filesAlreadyCopied.Add(destinationFile);     // Add to the list of copied files the destination file
                    FileCopied(file.Length);                                // Notify to the form that the current file has been copied
                }
                else
                    return;
            }

            // Copy subdirs recursively
            foreach (DirectoryInfo subdir in srcSubdirsArray)
            {
                if (!worker.isCancelled && !worker.isAborted)       // I need to check that here as well, otherwise the program would show the MessageBox above for every subdirectory
                    CopyDir(Path.Combine(srcFolderPath, subdir.Name), Path.Combine(destFolderPath, subdir.Name), CopyingFile, FileCopied, worker);
                else
                    return;
            }
        }

        /*
         *  Method that deletes the contents of the passed folder without deleting the folder itself
         */
        public static void RemoveDirContents(string folderPath, Worker worker)
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
                if (!worker.isCancelled && !worker.isAborted)     // check that work hasn't been cancelled
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
                            DialogResult choice = MessageBox.Show($"Unexpected error when deleting file {file.Name}: {exc.Message}" +
                                " Do you want to retry or skip folder contents removal?",
                                "Error during file deletion", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

                            if (choice == DialogResult.Retry)
                                continue;
                            else if (choice == DialogResult.Cancel)
                            {
                                worker.ErrorOccurred();
                                return;
                            }
                        }
                    }
                }
                else
                    return;
            }

            // Delete subdirs recursively
            foreach (DirectoryInfo subdir in subdirsArray)
            {
                if (!worker.isCancelled && !worker.isAborted)       // I need to check that here as well, otherwise the program would show the MessageBox above for every subdirectory
                    RemoveDirContents(Path.Combine(folderPath, subdir.Name), worker);
                else
                    return;
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
