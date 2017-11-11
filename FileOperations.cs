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
    public delegate void CompletionCallback(WorkOutcome outcome);

    public class FileOperations
    {
        /*
         *  Calculates the sizes of every folder in the list using CalculateDirSize() and puts them in the worker.
         *  Returns the overall size
         */
        public static long CalculateFoldersSizes(List<string> foldersToCopy, FileWorker worker)
        {
            worker.foldersSizes = new List<long>();
            long overallSize = 0;

            // Calculate the size of every folder to copy
            foreach(string folder in foldersToCopy)
                worker.foldersSizes.Add(CalculateDirSize(folder, worker));

            // Calculate the overall size and return it
            foreach (long folderSize in worker.foldersSizes)
                overallSize += folderSize;

            return overallSize;
        }

        /*
         *  Method that calculates the size of all the files contained in the passed directory and its subdirectories, then returns it
         */
        public static long CalculateDirSize(string localDirPath, FileWorker worker)
        {
            long dirSize = 0;

            // Check if target folder exists, if not exit
            if (!DirectoryExists(Path.Combine(worker.baseSourcePath, localDirPath)))
                return 0;

            // Retrieve informations for files and subdirectories
            DirectoryInfo dirToCompute = new DirectoryInfo(Path.Combine(worker.baseSourcePath, localDirPath));
            DirectoryInfo[] subdirsArray = dirToCompute.GetDirectories();
            FileInfo[] filesArray = dirToCompute.GetFiles();

            foreach (FileInfo file in filesArray)               // calculate files sizes
                dirSize += file.Length;

            foreach (DirectoryInfo subdir in subdirsArray)      // calculate subdirs sizes recursively
                dirSize += CalculateDirSize(Path.Combine(worker.baseSourcePath, localDirPath, subdir.Name), worker);

            return dirSize;
        }

        /*
         *  Method that copies a Cemu subdir from old installation to new one.
         *  Sends callbacks to MainForm in order to update progress bars.
         */
        public static void CopyDir(string localDirPath, ActualFileCallback CopyingFile, FileCopiedCallback FileCopied, FileWorker worker)
        {
            // Retrieve informations for files and subdirectories
            DirectoryInfo sourceDir = new DirectoryInfo(Path.Combine(worker.baseSourcePath, localDirPath));
            DirectoryInfo[] srcSubdirsArray = sourceDir.GetDirectories();
            FileInfo[] srcFilesArray = sourceDir.GetFiles();

            // Check if destination folder exists, if not create it
            if (!DirectoryExists(Path.Combine(worker.baseDestinationPath, localDirPath)))
                worker.directoriesAlreadyCopied.Add(Directory.CreateDirectory(Path.Combine(worker.baseDestinationPath, localDirPath)));

            // Copy files
            foreach (FileInfo file in srcFilesArray)
            {
                if (!worker.workIsCancelled && !worker.workAborted)     // check that work hasn't been cancelled
                {
                    bool copySuccessful = false;
                    FileInfo destinationFile = null;

                    CopyingFile(file.Name);         // Tell the MainForm the name of the file I'm about to copy
                    string destPath = Path.Combine(worker.baseDestinationPath, localDirPath, file.Name);
                    while(!copySuccessful)
                    {
                        try
                        {
                            destinationFile = file.CopyTo(destPath, true);
                        }
                        catch(Exception exc)
                        {
                            DialogResult choice = MessageBox.Show("Unexpected error when copying file " + file.Name + ": " + exc.Message + " What do you want to do?",
                                "Error during file copy", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);

                            if (choice == DialogResult.Retry)
                                continue;
                            else if (choice == DialogResult.Abort)
                            {
                                worker.workAborted = true;
                                return;
                            }
                            else if (choice == DialogResult.Ignore)
                                worker.errorsEncountered = true;
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
                if (!worker.workIsCancelled && !worker.workAborted)       // I need to check that here as well, otherwise the program would show the MessageBox above for every subdirectory
                    CopyDir(Path.Combine(localDirPath, subdir.Name), CopyingFile, FileCopied, worker);
                else
                    return;
            }
        }

        public static void RemoveDirContents(string folderPath, FileWorker worker)
        {
            // Retrieve informations for files and subdirectories
            DirectoryInfo directory = new DirectoryInfo(folderPath);
            DirectoryInfo[] subdirsArray = directory.GetDirectories();
            FileInfo[] filesArray = directory.GetFiles();

            // Check if destination folder exists, if not exit
            if (!DirectoryExists(folderPath))
                return;

            // Delete files
            foreach (FileInfo file in filesArray)
            {
                if (!worker.workIsCancelled && !worker.workAborted)     // check that work hasn't been cancelled
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
                            DialogResult choice = MessageBox.Show("Unexpected error when deleting file " + file.Name + ": " + exc.Message +
                                " Do you want to retry or skip folder contents removal?",
                                "Error during file deletion", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

                            if (choice == DialogResult.Retry)
                                continue;
                            else if (choice == DialogResult.Cancel)
                            {
                                worker.errorsEncountered = true;
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
                if (!worker.workIsCancelled && !worker.workAborted)       // I need to check that here as well, otherwise the program would show the MessageBox above for every subdirectory
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
            if (!File.Exists(filePath))
                return false;

            StringBuilder longPath = new StringBuilder(255);
            GetLongPathName(filePath, longPath, longPath.Capacity);

            string realPath = Path.GetDirectoryName(longPath.ToString());
            return Array.Exists(Directory.GetFiles(realPath), s => s == filePath);
        }

        public static bool DirectoryExists(string dirPath)
        {
            StringBuilder longPath = new StringBuilder(255);
            if (GetLongPathName(dirPath, longPath, longPath.Capacity) != 0 && dirPath == longPath.ToString())
                return true;
            else
                return false;
        }
    }
}
