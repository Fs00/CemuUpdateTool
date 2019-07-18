using CemuUpdateTool.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace CemuUpdateTool.Workers
{
    sealed class Updater : Downloader
    {
        private readonly string cemuInstallationPath;

        public Updater(string cemuInstallationPath, CancellationToken cancToken, Action<string, bool> LoggerDelegate)
            : base(Path.Combine(Path.GetTempPath(), "cemu_update"), cancToken, LoggerDelegate)
        {
            this.cemuInstallationPath = cemuInstallationPath;
        }

        /*
         *  Performs update operations, which include Cemu executable replacing, resources folder update (avoids not updated translations) and, upon user request,
         *  precompiled removal and game profiles update.
         *  Returns the version number of the downloaded Cemu version in order to update the latest known Cemu version in options.
         */
        public VersionNumber PerformUpdateOperations(bool removePrecompiledCaches, bool updateGameProfiles,
                                                     Action<string> PerformingWork, DownloadProgressChangedEventHandler progressHandler)
        {
            // Download the latest version of Cemu to downloadPath
            VersionNumber downloadedCemuVer = PerformDownloadOperations(PerformingWork, progressHandler);

            // Replace Cemu.exe from the downloaded Cemu version
            var downloadedCemuExe = new FileInfo(Path.Combine(extractedCemuFolder, "Cemu.exe"));
            downloadedCemuExe.CopyTo(Path.Combine(cemuInstallationPath, "Cemu.exe"), HandleLogMessage);

            // Copy 'resources' folder to the updated Cemu installation to avoid old translations being used
            PerformingWork("Updating translation files");
            FileUtils.CopyDirectory(Path.Combine(extractedCemuFolder, "resources"), Path.Combine(cemuInstallationPath, "resources"), HandleLogMessage, cancToken);

            // Remove precompiled caches
            if (removePrecompiledCaches)
            {
                PerformingWork("Removing precompiled caches");
                FileUtils.RemoveDirectoryContents(Path.Combine(cemuInstallationPath, "shaderCache", "precompiled"), HandleLogMessage, cancToken);
            }

            // Copy new game profiles
            if (updateGameProfiles)
            {
                PerformingWork("Updating game profiles");
                FileUtils.CopyDirectory(Path.Combine(extractedCemuFolder, "gameProfiles"), Path.Combine(cemuInstallationPath, "gameProfiles"), HandleLogMessage, cancToken);
            }

            // Clean up temporary downloaded Cemu folder
            try
            {
                Directory.Delete(extractedCemuFolder, true);
            }
            catch (Exception exc)
            {
                HandleLogMessage($"Unexpected error during deletion of temporary downloaded Cemu folder: {exc.Message}", EventLogEntryType.Error);
            }

            return downloadedCemuVer;
        }
    }
}
