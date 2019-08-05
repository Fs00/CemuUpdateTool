using CemuUpdateTool.Utils;
using System;
using System.IO;
using System.Threading;

namespace CemuUpdateTool.Workers
{
    sealed class Updater : Downloader
    {
        private readonly string cemuInstallationToBeUpdatedPath;

        public Updater(string cemuInstallationToBeUpdatedPath, CancellationToken cancToken)
            : base(Path.Combine(Path.GetTempPath(), "cemu_update"), cancToken)
        {
            this.cemuInstallationToBeUpdatedPath = cemuInstallationToBeUpdatedPath;
        }

        public VersionNumber PerformUpdateOperations(bool removePrecompiledCaches, bool updateGameProfiles)
        {
            try
            {
                return PerformActualUpdateOperations(removePrecompiledCaches, updateGameProfiles);
            }
            finally
            {
                TryDeleteTemporaryCemuInstallation();
            }
        }

        /*
         *  Returns the version number of the downloaded Cemu version in order to update the latest known Cemu version in options.
         */
        public VersionNumber PerformActualUpdateOperations(bool removePrecompiledCaches, bool updateGameProfiles)
        {
            VersionNumber downloadedCemuVersion = PerformDownloadOperations();
            ReplaceOldCemuExecutable();
            ReplaceOldTranslationFiles();

            if (removePrecompiledCaches)
                RemoveOldPrecompiledCaches();

            if (updateGameProfiles)
                ReplaceOldGameProfiles();

            return downloadedCemuVersion;
        }

        private void ReplaceOldCemuExecutable()
        {
            OnWorkStart("Updating Cemu executable");
            var downloadedCemuExecutable = new FileInfo(Path.Combine(downloadedCemuInstallation, "Cemu.exe"));
            downloadedCemuExecutable.CopyToAndReportOutcomeToWorker(Path.Combine(cemuInstallationToBeUpdatedPath, "Cemu.exe"), this);
        }

        private void ReplaceOldTranslationFiles()
        {
            OnWorkStart("Updating translation files");
            FileUtils.CopyDirectory(
                Path.Combine(downloadedCemuInstallation, "resources"),
                Path.Combine(cemuInstallationToBeUpdatedPath, "resources"),
                this
            );
        }

        private void RemoveOldPrecompiledCaches()
        {
            OnWorkStart("Removing precompiled caches");
            FileUtils.RemoveDirectoryContents(Path.Combine(cemuInstallationToBeUpdatedPath, "shaderCache", "precompiled"), this);
        }

        private void ReplaceOldGameProfiles()
        {
            OnWorkStart("Updating game profiles");
            FileUtils.CopyDirectory(
                Path.Combine(downloadedCemuInstallation, "gameProfiles"),
                Path.Combine(cemuInstallationToBeUpdatedPath, "gameProfiles"),
                this
            );
        }

        private void TryDeleteTemporaryCemuInstallation()
        {
            try
            {
                if (Directory.Exists(downloadedCemuInstallation))
                    Directory.Delete(downloadedCemuInstallation, recursive: true);
            }
            catch (Exception exc)
            {
                OnLogMessage(LogMessageType.Error, $"Unexpected error during deletion of temporary downloaded Cemu folder: {exc.Message}");
            }
        }
    }
}
