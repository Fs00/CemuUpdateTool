using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Net;
using Microsoft.Win32;
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

        public int ErrorsEncountered { private set; get; }
              
        public List<FileInfo> CreatedFiles { private set; get; }               // list of files that have been created by the Worker, necessary for restoring the original situation when you cancel the operation
        public List<DirectoryInfo> CreatedDirectories { private set; get; }    // list of directories that have been created by the Worker, necessary for restoring the original situation when you cancel the operation

        // ValueTuple arrays containing names and sizes of the files and folders to be copied
        (string Name, long Size)[] foldersToCopy;
        (string Name, long Size)[] filesToCopy;

        CancellationToken cancToken;
        MyWebClient client;
        Action<string, bool> LoggerDelegate;    // callback that writes a message on an external log (in this case MigrationForm textbox)

        public Worker(string baseSrcPath, string baseDestPath, List<string> foldersToCopy, List<string> filesToCopy, CancellationToken cancToken, Action<string, bool> LoggerDelegate)
        {
            BaseSourcePath = baseSrcPath;
            BaseDestinationPath = baseDestPath;
            this.cancToken = cancToken;
            this.LoggerDelegate = LoggerDelegate;

            // Populate folders/file tuple arrays and calculate their sizes
            this.foldersToCopy = new (string, long)[foldersToCopy.Count];
            for (int i = 0; i < foldersToCopy.Count; i++)
                this.foldersToCopy[i] = (foldersToCopy[i], 0L);

            this.filesToCopy = new (string, long)[filesToCopy.Count];
            for (int i = 0; i < filesToCopy.Count; i++)
                this.filesToCopy[i] = (filesToCopy[i], 0L);
            CalculateSizes();

            client = new MyWebClient();
            cancToken.Register(StopPendingWebOperation);    // register the action to be performed when cancellation is requested
            CreatedFiles = new List<FileInfo>();
            CreatedDirectories = new List<DirectoryInfo>();
        }

        /*
         *  Downloads and extracts the latest Cemu version.
         *  Returns the version number of the downloaded Cemu version (needed by MigrationForm)
         */
        public VersionNumber PerformDownloadOperations(Dictionary<string, string> downloadOptions, Action<string> PerformingWork, DownloadProgressChangedEventHandler progressHandler)
        {
            VersionNumber latestCemuVersion = null;

            // Get data from dictionary
            PerformingWork("Retrieving latest Cemu version");
            client.BaseAddress = downloadOptions["cemuBaseUrl"];
            string cemuUrlSuffix = downloadOptions["cemuUrlSuffix"];
            VersionNumber.TryParse(downloadOptions["lastKnownCemuVersion"], out VersionNumber lastKnownCemuVersion);    // avoid errors if version string in downloadOptions is malformed

            // FIND OUT WHICH IS THE LATEST CEMU VERSION
            bool versionObtained = false;
            while (!versionObtained)
            {
                try
                {
                    latestCemuVersion = WebUtils.GetLatestRemoteVersionInBranch(VersionNumber.Empty, client, cemuUrlSuffix,
                                                                                maxVersionLength: 3, lastKnownCemuVersion, cancToken);
                    versionObtained = true;
                }
                // Handle web request cancellation
                catch (WebException exc) when (exc.Status == WebExceptionStatus.RequestCanceled)
                {
                    throw new OperationCanceledException();
                }
                // Handle any web request error
                catch (WebException exc)
                {
                    // Build the message according to the type of error
                    string message = $"An error occurred when trying to find out which is the latest Cemu version: {GetWebErrorMessage(exc.Status)} ";
                    message += "Would you like to retry or give up the entire operation?";

                    DialogResult choice = MessageBox.Show(message, "Internet request error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    if (choice == DialogResult.Cancel)
                        throw;
                }
            }
            // If this condition is true, it's much likely caused by wrong Cemu website set
            if (latestCemuVersion == null)
                throw new ApplicationException("Unable to find out latest Cemu version. Maybe you altered download options with wrong information?");

            HandleLogMessage($"Latest Cemu version found is {latestCemuVersion.ToString()}.", EventLogEntryType.Information);

            // Add the DownloadProgressChanged event handler
            client.DownloadProgressChanged += progressHandler;

            // DOWNLOAD THE FILE
            string destinationFile = Path.Combine(BaseDestinationPath, "cemu_dl.tmp.zip");
            HandleLogMessage($"Downloading file {client.BaseAddress + latestCemuVersion.ToString() + cemuUrlSuffix}... ", EventLogEntryType.Information, false);
            bool fileDownloaded = false;
            while (!fileDownloaded)
            {
                try
                {
                    client.DownloadFileTaskAsync(client.BaseAddress + latestCemuVersion.ToString() + cemuUrlSuffix, destinationFile).Wait();
                    fileDownloaded = true;
                }
                catch (AggregateException exc)    // DownloadFileTaskAsync wraps all its exceptions in an AggregateException
                {
                    // Delete temporary download file if present, since it won't be used
                    try
                    {
                        if (FileUtils.FileExists(destinationFile))
                            System.IO.File.Delete(destinationFile);
                    }
                    catch (Exception deletionExc)
                    {
                        HandleLogMessage($"Unable to delete temporary download file: {deletionExc.Message}", EventLogEntryType.Error);
                    }

                    // Handle web request cancellation
                    if ((exc.InnerException as WebException)?.Status == WebExceptionStatus.RequestCanceled)
                        throw new OperationCanceledException();

                    // Build the message according to the type of error
                    string message = $"An error occurred when trying to download the latest Cemu version: ";
                    if (exc.InnerException is WebException webExc)     // internet or read-only file error
                    {
                        if (webExc.Status == WebExceptionStatus.UnknownError && webExc.InnerException != null)  // file error
                            message += webExc.InnerException.Message;
                        else
                            message += GetWebErrorMessage(webExc.Status);
                    }
                    else if (exc.InnerException is InvalidOperationException)  // should never happen
                        message += exc.InnerException.Message;
                    message += " Would you like to retry or give up the entire operation?";

                    DialogResult choice = MessageBox.Show(message, "Download error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    if (choice == DialogResult.Cancel)
                        throw exc.InnerException;
                }
            }
            HandleLogMessage("Done!", EventLogEntryType.Information);

            // EXTRACT CONTENTS
            HandleLogMessage("Extracting downloaded Cemu archive... ", EventLogEntryType.Information, false);
            FileUtils.ExtractZipFileContents(destinationFile, HandleLogMessage, cancToken);
            HandleLogMessage("Done!", EventLogEntryType.Information);

            // Since Cemu zips contain a root folder (./cemu_VERSION), move all the content outside that folder
            string extractedRootFolder = Path.Combine(BaseDestinationPath, $"cemu_{latestCemuVersion.ToString()}");
            FileUtils.CopyDir(extractedRootFolder, BaseDestinationPath, delegate {}, cancToken, null, CreatedFiles, CreatedDirectories);    // silent copy

            // Remove zip file and original folder once extraction is finished
            try
            {
                Directory.Delete(extractedRootFolder, true);
                System.IO.File.Delete(destinationFile);
            }
            catch (Exception exc)
            {
                HandleLogMessage($"Unexpected error during deletion of temporary download/extraction files: {exc.Message}", EventLogEntryType.Error);
            }

            return latestCemuVersion;
        }

        /*
         *  Method that performs all the migration operations requested by user.
         *  To be run in a separate thread using await keyword.
         */
        public void PerformMigrationOperations(Dictionary<string, bool> migrationOptions, Action<string> PerformingWork, IProgress<long> progressHandler)
        {
            if (string.IsNullOrWhiteSpace(BaseSourcePath) || string.IsNullOrWhiteSpace(BaseDestinationPath))
                throw new ArgumentException("Source and/or destination Cemu folder are set incorrectly!");

            // FOLDER OPERATIONS
            foreach (var folder in foldersToCopy)
            {
                // Destination folder contents removal
                if (migrationOptions["deleteDestFolderContents"] == true)
                {
                    string destFolderPath = Path.Combine(BaseDestinationPath, folder.Name);
                    if (FileUtils.DirectoryExists(destFolderPath))
                    {
                        PerformingWork($"Removing destination {folder.Name} folder previous contents");
                        try
                        {
                            FileUtils.RemoveDirContents(destFolderPath, HandleLogMessage, cancToken);
                        }
                        // Catch errors here since we don't want to abort the entire work if content deletion fails
                        catch (Exception exc) when (!(exc is OperationCanceledException))
                        {
                            HandleLogMessage($"Unable to complete folder {folder.Name} contents removal: {exc.Message}", EventLogEntryType.Error);
                        }
                    }
                }

                // Folder copy
                if (folder.Size > 0)     // avoiding to copy empty/unexisting folders
                {
                    PerformingWork($"Copying {folder.Name}");      // tell the form which folder I'm about to copy
                    FileUtils.CopyDir(Path.Combine(BaseSourcePath, folder.Name), Path.Combine(BaseDestinationPath, folder.Name), HandleLogMessage,
                                      cancToken, progressHandler, CreatedFiles, CreatedDirectories);
                }
            }

            // FILE COPY
            PerformingWork("Copying files");
            foreach (var file in filesToCopy)
            {
                cancToken.ThrowIfCancellationRequested();
                if (file.Size > 0)
                {
                    var fileObj = new FileInfo(Path.Combine(BaseSourcePath, file.Name));
                    fileObj.CopyToCustom(Path.Combine(BaseDestinationPath, file.Name), HandleLogMessage, progressHandler, CreatedFiles);
                }
                else
                    HandleLogMessage($"File {file.Name} empty or unexisting: skipped.", EventLogEntryType.Warning);
            }

            // SET COMPATIBILITY OPTIONS for new Cemu executable
            if (migrationOptions["setCompatibilityOptions"] == true)
            {
                // Build the key value
                string keyValue = "";
                if (migrationOptions["compatOpts_runAsAdmin"])
                    keyValue += "RUNASADMIN ";
                if (migrationOptions["compatOpts_noFullscreenOptimizations"])
                    keyValue += "DISABLEDXMAXIMIZEDWINDOWEDMODE ";
                if (migrationOptions["compatOpts_overrideHiDPIBehaviour"])
                    keyValue += "HIGHDPIAWARE";

                // Write the value in the registry
                if (!string.IsNullOrEmpty(keyValue))
                {
                    string newCemuExePath = Path.Combine(BaseDestinationPath, "Cemu.exe");
                    try
                    {
                        using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers"))
                        {
                            if (key == null)
                                throw new KeyNotFoundException("Unable to create the key.");

                            key.SetValue(newCemuExePath, keyValue);
                        }
                        HandleLogMessage($"Compatibility options for {newCemuExePath} set in the Windows Registry correctly.", EventLogEntryType.Information);
                    }
                    catch (Exception exc)
                    {
                        HandleLogMessage($"Unable to set compatibility options for {newCemuExePath} in the Windows Registry: {exc.Message}", EventLogEntryType.Error);
                    }
                }
            }
        }

        /*
         *  Calculates the size of every folder and file to copy
         */
        private void CalculateSizes()
        {
            // Calculate the size of every folder to copy
            for (int i = 0; i < foldersToCopy.Length; i++)
                foldersToCopy[i].Size = FileUtils.CalculateDirSize(Path.Combine(BaseSourcePath, foldersToCopy[i].Name), HandleLogMessage);

            // Calculate the size of every file to copy
            for (int i = 0; i < filesToCopy.Length; i++)
            {
                string filePath = Path.Combine(BaseSourcePath, filesToCopy[i].Name);
                if (FileUtils.FileExists(filePath))
                    filesToCopy[i].Size = new FileInfo(filePath).Length;
            }
        }

        /*
         *  Sum all the folder sizes and return the result (needed by the MigrationForm)
         */
        public long GetOverallSizeToCopy()
        {
            long overallSize = 0;

            foreach ((_, long folderSize) in foldersToCopy)
                overallSize += folderSize;
            foreach ((_, long fileSize) in filesToCopy)
                overallSize += fileSize;

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
         *  Deletes folders and directories created by the Worker (only used when the user cancels the task or it fails)
         */
        public void DeleteCreatedFiles()
        {
            foreach (FileInfo copiedFile in CreatedFiles)
                copiedFile.Delete();
            foreach (DirectoryInfo copiedDir in Enumerable.Reverse(CreatedDirectories))
                copiedDir.Delete();

            HandleLogMessage("Created files and directories deleted successfully.", EventLogEntryType.Information);
        }

        /*
         *  Checks if eventually there's a web operation pending and stops it
         *  Executed automatically when cancellation is requested (see constructor)
         */
        private void StopPendingWebOperation()
        {
            client?.CancelAsync();
            client?.MyUnderlyingWebRequest?.Abort();
        }

        /*
         *  Callback that handles a log message given its type (warning, info, error etc.).
         *  Through this callback, methods called by the Worker can notify errors.
         */
        private void HandleLogMessage(string message, EventLogEntryType type, bool newLine = true)
        {
            string logMessage = "";
            if (type == EventLogEntryType.Error || type == EventLogEntryType.FailureAudit)
            {
                // if it's an error message, update errors encountered flag
                ErrorsEncountered++;
                logMessage += "ERROR: ";
            }
            else if (type == EventLogEntryType.Warning)
                logMessage += "WARNING: ";

            logMessage += message;
            LoggerDelegate(logMessage, newLine);   // give the message to the MigrationForm
        }

        /*
         *  Return an error string according to the WebExceptionStatus passed.
         *  Used by PerformDownloadOperations().
         */
        private string GetWebErrorMessage(WebExceptionStatus excStatus)
        {
            string message;
            switch (excStatus)
            {
                case WebExceptionStatus.NameResolutionFailure:
                    message = "Name resolution failure. This can be due to absent internet connection or wrong Cemu website option.";
                    break;
                case WebExceptionStatus.ConnectFailure:
                    message = "Connection failure. Is your internet connection working?";
                    break;
                case WebExceptionStatus.Timeout:
                    message = "Request timed out. Could be a temporary server error as well as missing internet connection.";
                    break;
                default:
                    message = excStatus.ToString() + ".";
                    break;
            }
            return message;
        }
    }
}
