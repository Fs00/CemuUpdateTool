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
        protected readonly string downloadedCemuInstallation;

        private readonly WebClient webClient = new WebClient();
        private readonly RemoteVersionChecker versionChecker;

        private string cemuArchiveDownloadPath;
        private string tempCemuArchiveExtractionPath;
        
        public Downloader(string destinationCemuInstallationPath, CancellationToken cancToken)
            : base(cancToken)
        {
            downloadedCemuInstallation = destinationCemuInstallationPath;
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
            try
            {
                return PerformActualDownloadOperations(cemuVersionToBeDownloaded);
            }
            finally
            {
                TryDeleteTemporaryDownloadFiles();
            }
        }

        private VersionNumber PerformActualDownloadOperations(VersionNumber cemuVersionToBeDownloaded = null)
        {
            OnWorkStart("Downloading Cemu archive");
            webClient.DownloadProgressChanged += (_, evt) => OnProgressChange(evt.ProgressPercentage, 100);

            if (cemuVersionToBeDownloaded == null)
                cemuVersionToBeDownloaded = DiscoverLatestCemuVersion();
            else
            {
                if (cemuVersionToBeDownloaded.Length != 3)
                    cemuVersionToBeDownloaded = cemuVersionToBeDownloaded.GetCopyOfLength(3);
                EnsureSuppliedCemuVersionExists(cemuVersionToBeDownloaded);
            }

            DownloadCemuArchive(cemuVersionToBeDownloaded);
            ExtractDownloadedArchive(cemuVersionToBeDownloaded);
            
            // Worker is not passed here since copy must not be logged (and it's fast enough not to be noticeable)
            FileUtils.CopyDirectory(tempCemuArchiveExtractionPath, downloadedCemuInstallation);
            
            return cemuVersionToBeDownloaded;
        }

        private void EnsureSuppliedCemuVersionExists(VersionNumber cemuVersionToBeDownloaded)
        {
            var versionCheckOperation = new RemoteVersionExistenceCheckOperation(cemuVersionToBeDownloaded, versionChecker);
            versionCheckOperation.RetryUntilSuccessOrCancellationByWorker(this);
            if (!versionCheckOperation.SuppliedVersionExists)
                throw new ArgumentException("The Cemu version you supplied does not exist or Cemu repository has changed.");
        }
        
        private VersionNumber DiscoverLatestCemuVersion()
        {
            // Avoid errors if version string in download options is malformed
            VersionNumber.TryParse(Options.Download[OptionKey.LastKnownCemuVersion], out VersionNumber lastKnownCemuVersion);
            
            var latestVersionSearchOperation = new LatestRemoteVersionSearchOperation(lastKnownCemuVersion, versionChecker);
            latestVersionSearchOperation.RetryUntilSuccessOrCancellationByWorker(this);
            return latestVersionSearchOperation.LatestVersionFound;
        }
        
        // The file is downloaded in %Temp% directory (%UserProfile%\AppData\Local\Temp)
        private void DownloadCemuArchive(VersionNumber cemuVersion)
        {
            cemuArchiveDownloadPath = Path.Combine(Path.GetTempPath(), $"cemu_{cemuVersion}.zip");
            var fileDownloadOperation = new FileDownloadOperation(
                Options.Download[OptionKey.CemuBaseUrl] + cemuVersion + Options.Download[OptionKey.CemuUrlSuffix],
                cemuArchiveDownloadPath,
                webClient
            );
            OnOperationStart(fileDownloadOperation);
            fileDownloadOperation.RetryUntilSuccessOrCancellationByWorker(this);
        }

        private void ExtractDownloadedArchive(VersionNumber downloadedCemuVersion)
        {
            OnLogMessage(LogMessageType.Information, "Extracting downloaded Cemu archive... ", false);
            FileUtils.ExtractZipArchiveInSameDirectory(cemuArchiveDownloadPath, this);
            OnLogMessage(LogMessageType.Information, "Done!");

            // Cemu zips always contain a root folder (./cemu_[VERSION])
            tempCemuArchiveExtractionPath =
                Path.Combine(Path.GetDirectoryName(cemuArchiveDownloadPath), $"cemu_{downloadedCemuVersion}");
        }
        
        private void TryDeleteTemporaryDownloadFiles()
        {
            try
            {
                if (File.Exists(cemuArchiveDownloadPath))
                    File.Delete(cemuArchiveDownloadPath);
                if (Directory.Exists(tempCemuArchiveExtractionPath))
                    Directory.Delete(tempCemuArchiveExtractionPath, recursive: true);
            }
            catch (Exception exc)
            {
                OnLogMessage(LogMessageType.Error,
                    $"Unexpected error during deletion of temporary download/extraction files: {exc.Message}");
            }
        }

        public override void OnOperationStart(Operation operationInfo)
        {
            base.OnOperationStart(operationInfo);
            switch (operationInfo)
            {
                case FileDownloadOperation fileDownload:
                    OnLogMessage(LogMessageType.Information,
                        $"Downloading file {fileDownload.FileUrl}... ",
                        newLine: false
                    );
                    break;
            }
        }

        public override void OnOperationSuccess(Operation operationInfo)
        {
            base.OnOperationSuccess(operationInfo);
            switch (operationInfo)
            {
                case LatestRemoteVersionSearchOperation remoteVersionSearch:
                    OnLogMessage(LogMessageType.Information,
                                 $"Latest Cemu version found is {remoteVersionSearch.LatestVersionFound}.");
                    break;
                case FileDownloadOperation _:
                    OnLogMessage(LogMessageType.Information, "Done!");
                    break;
            }
        }

        // Errors in operations that make web requests can not be ignored, therefore in those cases we need to replace the
        // base method dialog (which has Abort, Retry and Ignore controls) with a dialog that has only Retry and Cancel buttons  
        protected override ErrorHandlingDecision AskUserHowToHandleError(RetryableOperation operationInfo, Exception error)
        {
            if (operationInfo is RemoteVersionExistenceCheckOperation ||
                operationInfo is LatestRemoteVersionSearchOperation ||
                operationInfo is FileDownloadOperation)
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
