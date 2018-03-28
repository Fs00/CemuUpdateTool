using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
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
    }

    public class Worker
    {
        public string BaseSourcePath { private set; get; }              // older Cemu folder
        public string BaseDestinationPath { private set; get; }         // new Cemu folder

        public bool ErrorsEncountered { private set; get; } = false;
              
        public List<FileInfo> CreatedFiles { private set; get; }               // list of files that have been created by the Worker, necessary for restoring the original situation when you cancel the operation
        public List<DirectoryInfo> CreatedDirectories { private set; get; }    // list of directories that have been created by the Worker, necessary for restoring the original situation when you cancel the operation

        List<string> foldersToCopy;     // list of folders to be copied
        List<long> foldersSizes;        // contains the sizes (in bytes) of the folders to copy
        byte currentFolderIndex = 0;    // index of the folder which is currently being copied
        CancellationToken cancToken;
        MyWebClient client;

        public Worker(string usrInputSrcPath, string usrInputDestPath, List<string> foldersToCopy, CancellationToken cancToken)
        {
            BaseSourcePath = usrInputSrcPath;
            BaseDestinationPath = usrInputDestPath;
            this.foldersToCopy = foldersToCopy;
            this.cancToken = cancToken;

            cancToken.Register(StopPendingWebOperation);
            CreatedFiles = new List<FileInfo>();
            CreatedDirectories = new List<DirectoryInfo>();
        }

        // TODO: PerformDownloadOperations()

        /*
         *  Method that performs all the migration operations requested by user.
         *  To be run in a separate thread using await keyword.
         */
        public void PerformMigrationOperations(Dictionary<string, bool> migrationOptions, WorkInfoCallback PerformingWork, 
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
                    PerformingWork("Copying settings.bin", 1);      // display in the MigrationForm the label "Copying settings.bin..." (1 is a placeholder)
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
                        FileUtils.RemoveDirContents(destFolderPath, cancToken, ErrorOccurred);
                    }
                }

                // Folder copy
                if (foldersSizes[currentFolderIndex] > 0)       // avoiding to copy empty/unexisting folders
                {
                    PerformingWork($"Copying {folder}", foldersSizes[currentFolderIndex]);      // tell the main form which folder I'm about to copy
                    FileUtils.CopyDir(Path.Combine(BaseSourcePath, folder), Path.Combine(BaseDestinationPath, folder), cancToken,
                                      CreatedFiles, CreatedDirectories, CopyingFile, FileCopied, ErrorOccurred);
                }
                currentFolderIndex++;
            }
        }

        /*
         *  Calculates the size of every folder to copy using CalculateDirSize()
         *  Returns the overall size (useful for the MigrationForm)
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

        /*
         *  Deletes folders and directories created by the Worker (only used when the user cancels the task)
         */
        public void PerformCleanup()
        {
            foreach (FileInfo copiedFile in CreatedFiles)
                copiedFile.Delete();
            foreach (DirectoryInfo copiedDir in Enumerable.Reverse(CreatedDirectories))
                copiedDir.Delete();
        }

        private void StopPendingWebOperation()
        {
            if (client != null && client.IsBusy)    // check if eventually there's a web operation pending and stop it
                client.CancelAsync();
        }

        public void ErrorOccurred()
        {
            ErrorsEncountered = true;
        }
    }
}
