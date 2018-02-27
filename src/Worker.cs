﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Net;
using IWshRuntimeLibrary;

namespace CemuUpdateTool
{
    public delegate void WorkInfoCallback(string name, long dim);

    public enum WorkOutcome
    {
        Success,
        Aborted,
        CancelledByUser,
        CompletedWithErrors,
        Undetermined = -1       // just a placeholder value before the worker terminates
    }

    public class Worker
    {
        public string BaseSourcePath { private set; get; }              // older Cemu folder
        public string BaseDestinationPath { private set; get; }         // new Cemu folder

        public bool IsCancelled { private set; get; } = false;
        public bool IsAborted { private set; get; } = false;
        public bool ErrorsEncountered { private set; get; } = false;
              
        public List<FileInfo> CreatedFiles { private set; get; }               // list of files that have been created by the Worker, necessary for restoring the original situation when you cancel the operation
        public List<DirectoryInfo> CreatedDirectories { private set; get; }    // list of directories that have been created by the Worker, necessary for restoring the original situation when you cancel the operation

        List<string> foldersToCopy;     // list of folders to be copied
        List<long> foldersSizes;        // contains the sizes (in bytes) of the folders to copy
        byte currentFolderIndex = 0;    // index of the folder which is currently being copied
        MyWebClient client;

        public Worker() {}              // constructor to be used only for tasks that don't require writing on disk

        public Worker(string usrInputSrcPath, string usrInputDestPath, List<string> foldersToCopy)
        {
            BaseSourcePath = usrInputSrcPath;
            BaseDestinationPath = usrInputDestPath;
            this.foldersToCopy = foldersToCopy;

            CreatedFiles = new List<FileInfo>();
            CreatedDirectories = new List<DirectoryInfo>();
        }

        // TODO: PerformDownloadOperations()

        /*
         *  Method that performs all the migration operations requested by user.
         *  To be run in a separate thread using await keyword.
         */
        public WorkOutcome PerformMigrationOperations(Dictionary<string, bool> migrationOptions, WorkInfoCallback PerformingWork, 
                                                      ActualFileCallback CopyingFile, FileCopiedCallback FileCopied)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(BaseSourcePath) && !string.IsNullOrWhiteSpace(BaseDestinationPath),
                "Source and/or destination Cemu folder are set incorrectly!");

            // COPY CEMU SETTINGS FILE
            if (migrationOptions["copyCemuSettingsFile"] == true)
            {
                if (FileUtils.FileExists(Path.Combine(BaseSourcePath, "settings.bin")))
                {
                    bool copySuccessful = false;
                    PerformingWork("Copying settings.bin", 1);      // display in the MainForm the label "Copying settings.bin..." (1 is a placeholder)
                    FileInfo settingsFile = new FileInfo(Path.Combine(BaseSourcePath, "settings.bin"));
                    while (!copySuccessful)
                    {
                        try
                        {
                            settingsFile.CopyTo(Path.Combine(BaseDestinationPath, "settings.bin"), true);
                            copySuccessful = true;
                        }
                        catch (Exception exc)
                        {
                            // If an error is encountered, ask the user if he wants to retry, otherwise skip the task
                            DialogResult choice = MessageBox.Show($"Unexpected error when copying Cemu settings file: {exc.Message} Do you want to retry? (if you click No, the file will be skipped)",
                                    "Error during settings.bin copy", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                            if (choice == DialogResult.No)
                            {
                                ErrorsEncountered = true;
                                break;
                            }
                        }
                    }
                }
            }

            // FOLDER OPERATIONS
            foreach (string folder in foldersToCopy)
            {
                // Destination folder contents removal
                if (migrationOptions["deleteDestFolderContents"] == true)
                {
                    string destFolderPath = Path.Combine(BaseDestinationPath, folder);
                    if (FileUtils.DirectoryExists(destFolderPath))
                    {
                        PerformingWork($"Removing destination {folder} folder previous contents", 1);
                        FileUtils.RemoveDirContents(destFolderPath, this);
                    }
                }

                // Folder copy
                if (foldersSizes[currentFolderIndex] > 0)       // avoiding to copy empty/unexisting folders
                {
                    PerformingWork($"Copying {folder}", foldersSizes[currentFolderIndex]);      // tell the main form which folder I'm about to copy
                    FileUtils.CopyDir(Path.Combine(BaseSourcePath, folder), Path.Combine(BaseDestinationPath, folder), CopyingFile, FileCopied, this);
                }
                currentFolderIndex++;

                if (IsCancelled || IsAborted)
                {
                    try
                    {
                        if (CreatedFiles.Count > 0 || CreatedDirectories.Count > 0)
                        {
                            // Ask if the user wants to remove files that have already been copied and, if the user accepts, performs the task. Then exit from the function.
                            DialogResult choice = MessageBox.Show("Do you want to delete files that have already been copied?", "Operation cancelled", MessageBoxButtons.YesNo);
                            if (choice == DialogResult.Yes)
                            {
                                foreach (FileInfo copiedFile in CreatedFiles)
                                    copiedFile.Delete();
                                foreach (DirectoryInfo copiedDir in Enumerable.Reverse(CreatedDirectories))
                                    copiedDir.Delete();
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show($"An error occurred when deleting already copied files: {exc.Message}", "Unexpected error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (IsCancelled)
                        return WorkOutcome.CancelledByUser;
                    else
                        return WorkOutcome.Aborted;
                }
            }

            // If the program arrives here, it means that the copy task has been completed
            if (!ErrorsEncountered)
                return WorkOutcome.Success;
            else
                return WorkOutcome.CompletedWithErrors;
        }

        /*
         *  Calculates the size of every folder to copy using CalculateDirSize()
         *  Returns the overall size (useful for the MainForm)
         */
        public long CalculateFoldersSizes()
        {
            foldersSizes = new List<long>(foldersToCopy.Capacity);
            long overallSize = 0;

            // Calculate the size of every folder to copy
            foreach (string folder in foldersToCopy)
                foldersSizes.Add(FileUtils.CalculateDirSize(Path.Combine(BaseSourcePath, folder)));

            // Calculate the overall size and return it
            foreach (long folderSize in foldersSizes)
                overallSize += folderSize;

            return overallSize;
        }

        public void CreateDesktopShortcut(string cemuVersion, string mlcExternalPath)
        {
            // Initialize WshShell and create shortcut object
            WshShell shell = new WshShell();
            IWshShortcut shortcut = shell.CreateShortcut(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Cemu {cemuVersion}.lnk"));

            // Set shortcut attributes
            shortcut.TargetPath = Path.Combine(BaseDestinationPath, "Cemu.exe");
            shortcut.WorkingDirectory = BaseDestinationPath;
            if (mlcExternalPath != null)
                shortcut.Arguments = $"-mlc \"{mlcExternalPath}\"";
            shortcut.Description = "The Wii U emulator";

            // Save shortcut on disk
            shortcut.Save();
        }

        public void StopWork(WorkOutcome reason)
        {
            if (client != null && client.IsBusy)    // check if eventually there's a web operation pending and stop it
                client.CancelAsync();

            if (reason == WorkOutcome.Aborted)
                IsAborted = true;
            else if (reason == WorkOutcome.CancelledByUser)
                IsCancelled = true;
            else
                throw new ArgumentException("Not a valid reason to stop the worker.");
        }

        public void ErrorOccurred()
        {
            ErrorsEncountered = true;
        }
    }
}
