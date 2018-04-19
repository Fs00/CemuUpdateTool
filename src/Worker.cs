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
    public delegate void LogMessageHandler(string message, EventLogEntryType type, bool newLine = true);
    public enum WorkOutcome
    {
        Success,
        CompletedWithErrors,
        Aborted,
        CancelledByUser
    }

    public class Worker
    {
        public string BaseSourcePath { get; }              // older Cemu folder
        public string BaseDestinationPath { get; }         // new Cemu folder

        public bool ErrorsEncountered { private set; get; } = false;
              
        public List<FileInfo> CreatedFiles { private set; get; }               // list of files that have been created by the Worker, necessary for restoring the original situation when you cancel the operation
        public List<DirectoryInfo> CreatedDirectories { private set; get; }    // list of directories that have been created by the Worker, necessary for restoring the original situation when you cancel the operation

        List<string> foldersToCopy;             // list of folders to be copied
        List<long> foldersSizes;                // contains the sizes (in bytes) of the folders to copy
        byte currentFolderIndex = 0;            // index of the folder which is currently being copied
        CancellationToken cancToken;
        MyWebClient client;
        Action<string, bool> LoggerDelegate;    // callback that writes a message on an external log (in this case MigrationForm textbox)

        public Worker(string usrInputSrcPath, string usrInputDestPath, List<string> foldersToCopy, CancellationToken cancToken, Action<string, bool> LoggerDelegate)
        {
            BaseSourcePath = usrInputSrcPath;
            BaseDestinationPath = usrInputDestPath;
            this.foldersToCopy = foldersToCopy;
            this.cancToken = cancToken;
            this.LoggerDelegate = LoggerDelegate;

            CalculateFoldersSizes();
            cancToken.Register(StopPendingWebOperation);    // register the action to be performed when cancellation is requested
            CreatedFiles = new List<FileInfo>();
            CreatedDirectories = new List<DirectoryInfo>();
        }

        // TODO: PerformDownloadOperations()

        /*
         *  Method that performs all the migration operations requested by user.
         *  To be run in a separate thread using await keyword.
         */
        public void PerformMigrationOperations(Dictionary<string, bool> migrationOptions, Action<string> PerformingWork, IProgress<long> progressHandler)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(BaseSourcePath) && !string.IsNullOrWhiteSpace(BaseDestinationPath),
                "Source and/or destination Cemu folder are set incorrectly!");

            // COPY CEMU SETTINGS FILE
            if (migrationOptions["copyCemuSettingsFile"] == true)
            {
                if (FileUtils.FileExists(Path.Combine(BaseSourcePath, "settings.bin")))
                {
                    bool copySuccessful = false;
                    PerformingWork("Copying settings.bin");      // display in the MigrationForm the label "Copying settings.bin..."
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
                        PerformingWork($"Removing destination {folder} folder previous contents");
                        try
                        {
                            FileUtils.RemoveDirContents(destFolderPath, HandleLogMessage, cancToken);
                        }
                        // Catch errors here since we don't want to abort the entire work if content deletion fails
                        catch (Exception exc) when (!(exc is OperationCanceledException))
                        {
                            HandleLogMessage($"Unable to complete folder {folder} contents removal.", EventLogEntryType.Error);
                        }
                    }
                }

                // Folder copy
                if (foldersSizes[currentFolderIndex] > 0)     // avoiding to copy empty/unexisting folders
                {
                    PerformingWork($"Copying {folder}");      // tell the form which folder I'm about to copy
                    FileUtils.CopyDir(Path.Combine(BaseSourcePath, folder), Path.Combine(BaseDestinationPath, folder), HandleLogMessage,
                                      cancToken, progressHandler, CreatedFiles, CreatedDirectories);
                }
                currentFolderIndex++;
            }
        }

        /*
         *  Calculates the size of every folder to copy using CalculateDirSize()
         */
        private void CalculateFoldersSizes()
        {
            foldersSizes = new List<long>(foldersToCopy.Capacity);

            // Calculate the size of every folder to copy
            foreach (string folder in foldersToCopy)
                foldersSizes.Add(FileUtils.CalculateDirSize(Path.Combine(BaseSourcePath, folder), HandleLogMessage));
        }

        /*
         *  Sum all the folder sizes and return the result (needed by the MigrationForm)
         */
        public long GetOverallSizeToCopy()
        {
            long overallSize = 0;

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

        /*
         *  Checks if eventually there's a web operation pending and stops it
         *  Executed automatically when cancellation is requested (see constructor)
         */
        private void StopPendingWebOperation()
        {
            if (client != null && client.IsBusy)
                client.CancelAsync();
        }

        /*
         *  Callback that handles a log message given its type (warning, info, error etc.).
         *  Through this callback, methods called by the Worker can notify errors.
         */
        public void HandleLogMessage(string message, EventLogEntryType type, bool newLine = true)
        {
            string logMessage = "";
            if (type == EventLogEntryType.Error || type == EventLogEntryType.FailureAudit)
            {
                // if it's an error message, update errors encountered flag
                ErrorsEncountered = true;
                logMessage += "ERROR: ";
            }
            else if (type == EventLogEntryType.Warning)
                logMessage += "WARNING: ";

            logMessage += message;
            LoggerDelegate(logMessage, newLine);   // give the message to the MigrationForm
        }
    }
}
