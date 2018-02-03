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
        public string baseSourcePath { private set; get; }                  // older Cemu folder
        public string baseDestinationPath { private set; get; }             // new Cemu folder

        public bool isCancelled { private set; get; } = false;
        public bool isAborted { private set; get; } = false;
        public bool errorsEncountered { private set; get; } = false;

        public List<long> foldersSizes { set; get; }                         // contains the sizes (in bytes) of the folders to copy
        public List<FileInfo> filesAlreadyCopied { set; get; }               // list of files that have already been copied, necessary if you want to restore the original situation when you cancel the operation
        public List<DirectoryInfo> directoriesAlreadyCopied { set; get; }    // list of directories that have already been copied, necessary if you want to restore the original situation when you cancel the operation

        byte currentFolderIndex = 0;            // index of the currently copying folder
        MyWebClient client;

        public Worker(string usrInputSrcPath, string usrInputDestPath)
        {
            baseSourcePath = usrInputSrcPath;
            baseDestinationPath = usrInputDestPath;

            filesAlreadyCopied = new List<FileInfo>();
            directoriesAlreadyCopied = new List<DirectoryInfo>();
        }

        // TODO: PerformDownloadOperations()

        /*
         *  Method that performs all the migration operations requested by user.
         *  To be run in a separate thread using await keyword.
         */
        public WorkOutcome PerformMigrationOperations(List<string> foldersToCopy, Dictionary<string, bool> additionalOptions, WorkInfoCallback PerformingWork, 
                                      ActualFileCallback CopyingFile, FileCopiedCallback FileCopied)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(baseSourcePath) && !string.IsNullOrWhiteSpace(baseDestinationPath),
                "Source and/or destination Cemu folder are set incorrectly!");

            // COPY CEMU SETTINGS FILE
            if (additionalOptions["copyCemuSettingsFile"] == true)
            {
                if (FileUtils.FileExists(Path.Combine(baseSourcePath, "settings.bin")))
                {
                    bool copySuccessful = false;
                    PerformingWork("Copying settings.bin", 1);      // display in the MainForm the label "Copying settings.bin..." (1 is a placeholder)
                    FileInfo settingsFile = new FileInfo(Path.Combine(baseSourcePath, "settings.bin"));
                    while (!copySuccessful)
                    {
                        try
                        {
                            settingsFile.CopyTo(Path.Combine(baseDestinationPath, "settings.bin"), true);
                            copySuccessful = true;
                        }
                        catch (Exception exc)
                        {
                            // If an error is encountered, ask the user if he wants to retry, otherwise skip the task
                            DialogResult choice = MessageBox.Show($"Unexpected error when copying Cemu settings file: {exc.Message} Do you want to retry? (if you click No, the file will be skipped)",
                                    "Error during settings.bin copy", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                            if (choice == DialogResult.No)
                            {
                                errorsEncountered = true;
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
                if (additionalOptions["deleteDestFolderContents"] == true)
                {
                    string destFolderPath = Path.Combine(baseDestinationPath, folder);
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
                    FileUtils.CopyDir(Path.Combine(baseSourcePath, folder), Path.Combine(baseDestinationPath, folder), CopyingFile, FileCopied, this);
                }
                currentFolderIndex++;

                if (isCancelled || isAborted)
                {
                    try
                    {
                        if (filesAlreadyCopied.Count > 0 || directoriesAlreadyCopied.Count > 0)
                        {
                            // Ask if the user wants to remove files that have already been copied and, if the user accepts, performs the task. Then exit from the function.
                            DialogResult choice = MessageBox.Show("Do you want to delete files that have already been copied?", "Operation cancelled", MessageBoxButtons.YesNo);
                            if (choice == DialogResult.Yes)
                            {
                                foreach (FileInfo copiedFile in filesAlreadyCopied)
                                    copiedFile.Delete();
                                foreach (DirectoryInfo copiedDir in Enumerable.Reverse(directoriesAlreadyCopied))
                                    copiedDir.Delete();
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show($"An error occurred when deleting already copied files: {exc.Message}", "Unexpected error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (isCancelled)
                        return WorkOutcome.CancelledByUser;
                    else
                        return WorkOutcome.Aborted;
                }
            }

            // If the program arrives here, it means that the copy task has been completed
            if (!errorsEncountered)
                return WorkOutcome.Success;
            else
                return WorkOutcome.CompletedWithErrors;
        }

        public void CreateDesktopShortcut(string cemuVersion, string mlcExternalPath)
        {
            // Initialize WshShell and create shortcut object
            WshShell shell = new WshShell();
            IWshShortcut shortcut = shell.CreateShortcut(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Cemu {cemuVersion}.lnk"));

            // Set shortcut attributes
            shortcut.TargetPath = Path.Combine(baseDestinationPath, "Cemu.exe");
            shortcut.WorkingDirectory = baseDestinationPath;
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
                isAborted = true;
            else if (reason == WorkOutcome.CancelledByUser)
                isCancelled = true;
            else
                throw new ArgumentException("Not a valid reason to stop the worker.");
        }

        public void ErrorOccurred()
        {
            errorsEncountered = true;
        }
    }
}
