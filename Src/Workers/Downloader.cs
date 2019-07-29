using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using CemuUpdateTool.Settings;
using CemuUpdateTool.Utils;
using CemuUpdateTool.Workers.Operations;

namespace CemuUpdateTool.Workers
{
    class Downloader : Worker
    {
        protected readonly string extractedCemuFolder;

        private readonly WebClient webClient = new WebClient();
        private readonly RemoteVersionChecker versionChecker;

        public Downloader(string destinationCemuInstallationPath, CancellationToken cancToken)
            : base(cancToken)
        {
            extractedCemuFolder = destinationCemuInstallationPath;
            versionChecker = new RemoteVersionChecker(
                Options.Download[OptionKey.CemuBaseUrl],
                Options.Download[OptionKey.CemuUrlSuffix],
                maxVersionLength: 3,
                cancToken
            );
            cancToken.Register(webClient.CancelAsync);
        }

        /*
         *  Downloads and extracts the latest Cemu version.
         *  Returns the version number of the downloaded Cemu version (needed to update the latest known Cemu version in options)
         */
        public VersionNumber PerformDownloadOperations(VersionNumber cemuVersionToBeDownloaded = null)
        {
            OnWorkStart("Downloading Cemu archive");
            webClient.DownloadProgressChanged += (_, evt) => OnProgressChange(evt.ProgressPercentage, 100);

            // If no Cemu version to be downloaded is specified, discover which is the latest one
            if (cemuVersionToBeDownloaded == null)
            {
                // Avoid errors if version string in download options is malformed
                VersionNumber.TryParse(Options.Download[OptionKey.LastKnownCemuVersion], out VersionNumber lastKnownCemuVersion);
                cemuVersionToBeDownloaded = DiscoverLatestCemuVersion(lastKnownCemuVersion);
                
                // TODO: fare sì che il version check lanci quest'eccezione nel controllo preliminare
                /*if (cemuVersionToBeDownloaded == null)
                    throw new ApplicationException("Unable to find out latest Cemu version. Maybe you altered download options with wrong information?");*/

                OnLogMessage(LogMessageType.Information, $"Latest Cemu version found is {cemuVersionToBeDownloaded}.");
            }
            // Otherwise, check if the supplied version exists. If not, quit the task
            else
            {
                var versionCheckOperation = new RemoteVersionExistenceCheckOperation(cemuVersionToBeDownloaded, versionChecker);
                versionCheckOperation.RetryUntilSuccessOrCancellationByWorker(this);
                if (!versionCheckOperation.SuppliedVersionExists)
                    throw new ArgumentException("The Cemu version you supplied does not exist or Cemu repository has changed.");
            }

            // DOWNLOAD THE FILE
            OnLogMessage(
                LogMessageType.Information,
                $"Downloading file {Options.Download[OptionKey.CemuBaseUrl] + cemuVersionToBeDownloaded + Options.Download[OptionKey.CemuUrlSuffix]}... ",
                false
            );
            string downloadedCemuZip = DownloadCemuArchive(cemuVersionToBeDownloaded);
            OnLogMessage(LogMessageType.Information, "Done!");

            // EXTRACT CONTENTS
            OnLogMessage(LogMessageType.Information, "Extracting downloaded Cemu archive... ", false);
            string extractionPath = FileUtils.ExtractZipArchiveInSameDirectory(downloadedCemuZip, this);
            OnLogMessage(LogMessageType.Information, "Done!");

            // Since Cemu zips contain a root folder (./cemu_[VERSION]), copy the content from there to the destination path
            string tempDownloadedCemuFolder = Path.Combine(extractionPath, $"cemu_{cemuVersionToBeDownloaded}");
            // Worker is not passed since copy must not be logged (and it's fast enough not to be noticeable)
            FileUtils.CopyDirectory(tempDownloadedCemuFolder, extractedCemuFolder);

            TryDeleteTemporaryDownloadFiles(tempDownloadedCemuFolder, downloadedCemuZip);

            return cemuVersionToBeDownloaded;
        }

        /*
         *  Performs the download of the selected Cemu version.
         *  The file is downloaded in %Temp% directory (%UserProfile%\AppData\Local\Temp)
         */
        private string DownloadCemuArchive(VersionNumber cemuVersion)
        {
            string destinationFile = Path.Combine(Path.GetTempPath(), $"cemu_{cemuVersion}.zip");
            bool fileDownloaded = false;
            while (!fileDownloaded)
            {
                try
                {
                    webClient.DownloadFileSynchronouslyWithProgressReporting(
                        Options.Download[OptionKey.CemuBaseUrl] + cemuVersion + Options.Download[OptionKey.CemuUrlSuffix],
                        destinationFile
                    );
                    fileDownloaded = true;
                }
                catch (Exception exc)
                {
                    // TODO: valutare se spostare questo blocco nell'extension method
                    // Delete temporary download file if present, since it won't be used
                    try
                    {
                        if (File.Exists(destinationFile))
                            File.Delete(destinationFile);
                    }
                    catch (Exception deletionExc)
                    {
                        OnLogMessage(LogMessageType.Error, $"Unable to delete temporary download file: {deletionExc.Message}");
                    }

                    // Build the message according to the type of error
                    string message = "An error occurred when trying to download the latest Cemu version: ";
                    if (exc is WebException webExc)     // internet or read-only file error
                    {
                        if (webExc.Status == WebExceptionStatus.UnknownError && webExc.InnerException != null)  // file error
                            message += webExc.InnerException.Message;
                        else
                            message += WebUtils.GetErrorMessageFromWebExceptionStatus(webExc.Status);
                    }
                    else if (exc is InvalidOperationException)  // should never happen
                        message += exc.Message;
                    message += " Would you like to retry or give up the entire operation?";

                    DialogResult choice = MessageBox.Show(message, "Download error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    if (choice == DialogResult.Cancel)
                        throw;
                }
            }
            return destinationFile;
        }

        private VersionNumber DiscoverLatestCemuVersion(VersionNumber lastKnownCemuVersion = null)
        {
            var latestVersionSearchOperation = new LatestRemoteVersionSearchOperation(lastKnownCemuVersion, versionChecker);
            latestVersionSearchOperation.RetryUntilSuccessOrCancellationByWorker(this);
            return latestVersionSearchOperation.LatestVersionFound;
        }
        
        private void TryDeleteTemporaryDownloadFiles(string tempDownloadedCemuFolder, string downloadedCemuZip)
        {
            try
            {
                Directory.Delete(tempDownloadedCemuFolder, recursive: true);
                File.Delete(downloadedCemuZip);
            }
            catch (Exception exc)
            {
                OnLogMessage(LogMessageType.Error,
                    $"Unexpected error during deletion of temporary download/extraction files: {exc.Message}");
            }
        }

        // Errors in operations that make web requests can not be ignored, therefore in those cases we need to replace the
        // base method dialog (which has Abort, Retry and Ignore controls) with a dialog that has only Retry and Cancel buttons  
        protected override ErrorHandlingDecision AskUserHowToHandleError(RetryableOperation operationInfo, Exception error)
        {
            if (operationInfo is RemoteVersionExistenceCheckOperation || operationInfo is LatestRemoteVersionSearchOperation)
            {
                string dialogMessage = operationInfo.BuildMessageForErrorHandlingDecision(error);
                string dialogTitle = "Error during " + operationInfo.OperationName;

                DialogResult choice = MessageBox.Show(dialogMessage, dialogTitle,
                                                      MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                return choice.ToErrorHandlingDecision();
            }
            
            return base.AskUserHowToHandleError(operationInfo, error);
        }
    }
}
