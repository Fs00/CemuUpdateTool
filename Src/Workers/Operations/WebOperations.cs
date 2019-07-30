using System;
using System.Net;
using CemuUpdateTool.Utils;

namespace CemuUpdateTool.Workers.Operations
{
    class RemoteVersionExistenceCheckOperation : RetryableOperation
    {
        public VersionNumber VersionToCheck { get; }
        public override string OperationName => "Cemu version existence check";

        private readonly RemoteVersionChecker versionChecker;
        
        private bool suppliedVersionExists;
        public bool SuppliedVersionExists
        {
            get
            {
                if (!IsCompleted)
                    throw new InvalidOperationException("Version existence has not been checked yet.");
                return suppliedVersionExists;
            }
        }

        public RemoteVersionExistenceCheckOperation(VersionNumber versionToCheck, RemoteVersionChecker versionChecker)
        {
            VersionToCheck = versionToCheck;
            this.versionChecker = versionChecker;
        }
        
        protected override void PerformOperationLogic()
        {
            suppliedVersionExists = versionChecker.RemoteVersionExists(VersionToCheck);
        }

        public override string BuildFailureMessage(string failureReason)
        {
            return $"Unable to check the existence of version {VersionToCheck}: {failureReason}";
        }

        public override string BuildMessageForErrorHandlingDecision(Exception error)
        {
            string message = "An error occurred while checking the existence of the supplied Cemu version: ";
            if (error is WebException webException)
                message += WebUtils.GetErrorMessageFromWebExceptionStatus(webException.Status);
            else
                message += error.Message;
            message += "\r\nWould you like to retry or give up the entire operation?";
            return message;
        }
    }

    class LatestRemoteVersionSearchOperation : RetryableOperation
    {
        public override string OperationName => "Cemu latest version search";

        private readonly VersionNumber startingVersion;
        private readonly RemoteVersionChecker versionChecker;
        
        private VersionNumber latestVersionFound;
        public VersionNumber LatestVersionFound
        {
            get
            {
                if (!IsCompleted)
                    throw new InvalidOperationException("Latest version has not been searched yet.");
                return latestVersionFound;
            }
        }

        public LatestRemoteVersionSearchOperation(VersionNumber startingVersion, RemoteVersionChecker versionChecker)
        {
            this.startingVersion = startingVersion;
            this.versionChecker = versionChecker;
        }

        protected override void PerformOperationLogic()
        {
            latestVersionFound = versionChecker.GetLatestRemoteVersionInBranch(VersionNumber.Empty, startingVersion);
        }

        public override string BuildFailureMessage(string failureReason)
        {
            return $"Unable to find out latest Cemu version: {failureReason}";
        }

        public override string BuildMessageForErrorHandlingDecision(Exception error)
        {
            string message = "An error occurred while finding the latest Cemu version: ";
            if (error is WebException webException)
                message += WebUtils.GetErrorMessageFromWebExceptionStatus(webException.Status);
            else
                message += error.Message;
            message += "\r\nWould you like to retry or give up the entire operation?";
            return message;
        }
    }

    class FileDownloadOperation : RetryableOperation
    {
        public override string OperationName => "file download";
        public string FileUrl { get; }
        public string DestinationFilePath { get; }
        
        private readonly WebClient webClient;

        public FileDownloadOperation(string fileUrl, string destinationFilePath, WebClient webClient)
        {
            FileUrl = fileUrl;
            DestinationFilePath = destinationFilePath;
            this.webClient = webClient;
        }

        protected override void PerformOperationLogic()
        {
            webClient.DownloadFileSynchronouslyWithProgressReporting(FileUrl, DestinationFilePath);
        }

        public override string BuildFailureMessage(string failureReason)
        {
            return $"Unable to download file {FileUrl}: {failureReason}";
        }

        public override string BuildMessageForErrorHandlingDecision(Exception error)
        {
            string message = $"An error occurred when trying to download file {FileUrl}: ";
            if (error is WebException webException)
                message += WebUtils.GetErrorMessageFromWebExceptionStatus(webException.Status);
            else
                message += error.Message;
            message += "\r\nWould you like to retry or give up the entire operation?";
            return message;
        }
    }
}