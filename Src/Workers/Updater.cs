using CemuUpdateTool.Utils;
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace CemuUpdateTool.Workers
{
    sealed class Updater : Downloader
    {
        private readonly string cemuInstallationPath;

        public Updater(string cemuInstallationPath, CancellationToken cancToken)
            : base(Path.Combine(Path.GetTempPath(), "cemu_update"), cancToken)
        {
            this.cemuInstallationPath = cemuInstallationPath;
        }

        /*
         *  Performs update operations, which include Cemu executable replacing, resources folder update (avoids not updated translations) and, upon user request,
         *  precompiled removal and game profiles update.
         *  Returns the version number of the downloaded Cemu version in order to update the latest known Cemu version in options.
         */
        public VersionNumber PerformUpdateOperations(bool removePrecompiledCaches, bool updateGameProfiles,
                                                     DownloadProgressChangedEventHandler progressHandler)
        {
            VersionNumber downloadedCemuVer = PerformDownloadOperations(progressHandler);

            // Replace Cemu.exe from the downloaded Cemu version
            var downloadedCemuExe = new FileInfo(Path.Combine(extractedCemuFolder, "Cemu.exe"));
            downloadedCemuExe.CopyTo(Path.Combine(cemuInstallationPath, "Cemu.exe"), this);

            // Copy 'resources' folder to the updated Cemu installation to avoid old translations being used
            OnWorkStart("Updating translation files");
            FileUtils.CopyDirectory(Path.Combine(extractedCemuFolder, "resources"), Path.Combine(cemuInstallationPath, "resources"), this);

            if (removePrecompiledCaches)
            {
                OnWorkStart("Removing precompiled caches");
                FileUtils.RemoveDirectoryContents(Path.Combine(cemuInstallationPath, "shaderCache", "precompiled"), this);
            }

            if (updateGameProfiles)
            {
                OnWorkStart("Updating game profiles");
                FileUtils.CopyDirectory(Path.Combine(extractedCemuFolder, "gameProfiles"), Path.Combine(cemuInstallationPath, "gameProfiles"), this);
            }

            // Clean up temporary downloaded Cemu folder
            try
            {
                Directory.Delete(extractedCemuFolder, true);
            }
            catch (Exception exc)
            {
                OnLogMessage(LogMessageType.Error, $"Unexpected error during deletion of temporary downloaded Cemu folder: {exc.Message}");
            }

            return downloadedCemuVer;
        }
    }
}
