using CemuUpdateTool.Settings;
using CemuUpdateTool.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace CemuUpdateTool.Workers
{
    class Downloader : Worker
    {
        protected readonly string extractedCemuFolder;

        private readonly WebClient webClient;
        private readonly RemoteVersionChecker versionChecker;

        public Downloader(string destinationCemuInstallationPath, CancellationToken cancToken, Action<string, bool> LoggerDelegate)
            : base(cancToken, LoggerDelegate)
        {
            extractedCemuFolder = destinationCemuInstallationPath;
            webClient = new WebClient();
            versionChecker = new RemoteVersionChecker(
                Options.Download[OptionKey.CemuBaseUrl],
                Options.Download[OptionKey.CemuUrlSuffix],
                maxVersionLength: 3
            );
            cancToken.Register(webClient.CancelAsync);
        }

        /*
         *  Downloads and extracts the latest Cemu version.
         *  Returns the version number of the downloaded Cemu version (needed to update the latest known Cemu version in options)
         */
        public VersionNumber PerformDownloadOperations(Action<string> PerformingWork, DownloadProgressChangedEventHandler progressHandler,
                                                       VersionNumber cemuVersionToBeDownloaded = null)
        {
            PerformingWork("Downloading Cemu archive");
            webClient.DownloadProgressChanged += progressHandler;

            // If no Cemu version to be downloaded is specified, discover which is the latest one
            if (cemuVersionToBeDownloaded == null)
            {
                // Avoid errors if version string in download options is malformed
                VersionNumber.TryParse(Options.Download[OptionKey.LastKnownCemuVersion], out VersionNumber lastKnownCemuVersion);
                cemuVersionToBeDownloaded = DiscoverLatestCemuVersion(lastKnownCemuVersion);
                if (cemuVersionToBeDownloaded == null)       // if this condition is true, it's much likely caused by wrong Cemu website set
                    throw new ApplicationException("Unable to find out latest Cemu version. Maybe you altered download options with wrong information?");

                HandleLogMessage($"Latest Cemu version found is {cemuVersionToBeDownloaded.ToString()}.", EventLogEntryType.Information);
            }
            // Otherwise, check if the supplied version exists. If not, quit the task
            else
            {
                bool versionChecked = false;
                while (!versionChecked)
                {
                    try
                    {
                        if (!versionChecker.RemoteVersionExists(cemuVersionToBeDownloaded, cancToken))
                            throw new ArgumentException("The Cemu version you supplied does not exist.");

                        versionChecked = true;
                    }
                    catch (WebException exc)
                    {
                        // Build the message according to the type of error
                        string message = $"An error occurred when trying to connect to Cemu version repository: {GetWebErrorMessage(exc.Status)} " +
                                         "Would you like to retry or give up the entire operation?";

                        DialogResult choice = MessageBox.Show(message, "Internet request error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                        if (choice == DialogResult.Cancel)
                            throw;
                    }
                }
            }

            // DOWNLOAD THE FILE
            HandleLogMessage(
                $"Downloading file {Options.Download[OptionKey.CemuBaseUrl] + cemuVersionToBeDownloaded.ToString() + Options.Download[OptionKey.CemuUrlSuffix]}... ",
                EventLogEntryType.Information,
                false
            );
            string downloadedCemuZip = DownloadCemuArchive(cemuVersionToBeDownloaded);
            HandleLogMessage("Done!", EventLogEntryType.Information);

            // EXTRACT CONTENTS
            HandleLogMessage("Extracting downloaded Cemu archive... ", EventLogEntryType.Information, false);
            string extractionPath = FileUtils.ExtractZipArchiveInSameDirectory(downloadedCemuZip, HandleLogMessage, cancToken);
            HandleLogMessage("Done!", EventLogEntryType.Information);

            // Since Cemu zips contain a root folder (./cemu_[VERSION]), copy the content from there to the destination path
            string tempDownloadedCemuFolder = Path.Combine(extractionPath, $"cemu_{cemuVersionToBeDownloaded.ToString()}");
            FileUtils.CopyDirectory(tempDownloadedCemuFolder, extractedCemuFolder, delegate {}, cancToken);    // silent copy

            try
            {
                Directory.Delete(tempDownloadedCemuFolder, recursive: true);
                File.Delete(downloadedCemuZip);
            }
            catch (Exception exc)
            {
                HandleLogMessage($"Unexpected error during deletion of temporary download/extraction files: {exc.Message}", EventLogEntryType.Error);
            }

            return cemuVersionToBeDownloaded;
        }

        /*
         *  Performs the download of the selected Cemu version.
         *  The file is downloaded in %Temp% directory (%UserProfile%\AppData\Local\Temp)
         */
        private string DownloadCemuArchive(VersionNumber cemuVersion)
        {
            string destinationFile = Path.Combine(Path.GetTempPath(), $"cemu_{cemuVersion.ToString()}.zip");
            bool fileDownloaded = false;
            while (!fileDownloaded)
            {
                try
                {
                    webClient.DownloadFileTaskAsync(
                        Options.Download[OptionKey.CemuBaseUrl] + cemuVersion.ToString() + Options.Download[OptionKey.CemuUrlSuffix],
                        destinationFile
                    ).Wait();
                    fileDownloaded = true;
                }
                catch (AggregateException exc)    // DownloadFileTaskAsync wraps all its exceptions in an AggregateException
                {
                    // Delete temporary download file if present, since it won't be used
                    try
                    {
                        if (File.Exists(destinationFile))
                            File.Delete(destinationFile);
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
            return destinationFile;
        }

        private VersionNumber DiscoverLatestCemuVersion(VersionNumber lastKnownCemuVersion = null)
        {
            VersionNumber latestCemuVersion = null;
            bool versionObtained = false;
            while (!versionObtained)
            {
                try
                {
                    latestCemuVersion = versionChecker.GetLatestRemoteVersionInBranch(VersionNumber.Empty, lastKnownCemuVersion, cancToken);
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
                    string message = $"An error occurred when trying to find out which is the latest Cemu version: {GetWebErrorMessage(exc.Status)} " +
                                      "Would you like to retry or give up the entire operation?";

                    DialogResult choice = MessageBox.Show(message, "Internet request error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    if (choice == DialogResult.Cancel)
                        throw;
                }
            }
            return latestCemuVersion;
        }

        /*
         *  Return an error string according to the WebExceptionStatus passed.
         */
        protected string GetWebErrorMessage(WebExceptionStatus excStatus)
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
