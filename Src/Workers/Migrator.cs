using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using CemuUpdateTool.Settings;
using CemuUpdateTool.Utils;
using IWshRuntimeLibrary;
using Microsoft.Win32;

namespace CemuUpdateTool.Workers
{
    sealed class Migrator : Worker
    {
        // Necessary for restoring the original situation when operation is cancelled
        public List<FileInfo> CreatedFiles { get; } = new List<FileInfo>();
        public List<DirectoryInfo> CreatedDirectories { get; } = new List<DirectoryInfo>();

        private readonly string sourceCemuInstallationPath;
        private readonly string destinationCemuInstallationPath;

        private readonly VersionNumber sourceCemuVersion;

        public Migrator(string sourceCemuInstallationPath,
                        string destinationCemuInstallationPath,
                        VersionNumber sourceCemuVersion,
                        CancellationToken cancToken,
                        Action<string, bool> LoggerDelegate)
               : base(cancToken, LoggerDelegate)
        {
            this.sourceCemuInstallationPath = sourceCemuInstallationPath;
            this.destinationCemuInstallationPath = destinationCemuInstallationPath;
            this.sourceCemuVersion = sourceCemuVersion;
        }

        public void PerformMigrationOperations(Action<string> PerformingWork, IProgress<long> progressHandler)
        {
            if (string.IsNullOrWhiteSpace(sourceCemuInstallationPath) || string.IsNullOrWhiteSpace(destinationCemuInstallationPath))
                throw new ArgumentException("Source and/or destination Cemu folder are set incorrectly!");

            MigrateFolders(PerformingWork, progressHandler);
            MigrateFiles(PerformingWork, progressHandler);

            if (Options.Migration[OptionKey.SetCompatibilityOptions])
                SetCompatibilityOptionsForDestinationCemuExecutable();
        }

        private void MigrateFolders(Action<string> PerformingWork, IProgress<long> progressHandler)
        {
            foreach (string folder in Options.FoldersToMigrate.GetAllEnabled())
            {
                if (ShouldNotCopyMlcSubfolders() && IsMlcSubfolder(folder))
                    continue;

                string sourceFolderPath = Path.Combine(sourceCemuInstallationPath, folder);
                if (!Directory.Exists(sourceFolderPath))
                    continue;

                string destinationFolderPath = Path.Combine(destinationCemuInstallationPath, folder);
                if (Options.Migration[OptionKey.DeleteDestinationFolderContents] && Directory.Exists(destinationFolderPath))
                {
                    PerformingWork($"Removing destination {folder} folder previous contents");
                    try
                    {
                        FileUtils.RemoveDirectoryContents(destinationFolderPath, HandleLogMessage, cancToken);
                    }
                    // Catch errors here since we don't want to abort the entire work if content deletion fails
                    catch (Exception exc) when (!(exc is OperationCanceledException))
                    {
                        HandleLogMessage($"Unable to complete folder {folder} contents removal: {exc.Message}", EventLogEntryType.Error);
                    }
                }

                PerformingWork($"Copying {folder}");
                FileUtils.CopyDirectory(sourceFolderPath, destinationFolderPath, HandleLogMessage,
                                    cancToken, progressHandler, CreatedFiles, CreatedDirectories);
            }
        }

        private bool ShouldNotCopyMlcSubfolders()
        {
            return Options.Migration[OptionKey.UseCustomMlcFolderIfSupported] &&
                   sourceCemuVersion >= new VersionNumber(1, 10);
        }

        private bool IsMlcSubfolder(string folder)
        {
            return folder.StartsWith(@"mlc01\");
        }

        private void MigrateFiles(Action<string> PerformingWork, IProgress<long> progressHandler)
        {
            PerformingWork("Copying files");
            foreach (string fileToCopy in Options.FilesToMigrate.GetAllEnabled())
            {
                cancToken.ThrowIfCancellationRequested();
                var file = new FileInfo(Path.Combine(sourceCemuInstallationPath, fileToCopy));
                if (file.Exists)
                    file.CopyTo(Path.Combine(destinationCemuInstallationPath, fileToCopy), HandleLogMessage, progressHandler, CreatedFiles);
                else
                    HandleLogMessage($"File {fileToCopy} doesn't exist in source Cemu installation.", EventLogEntryType.Warning);
            }
        }

        private void SetCompatibilityOptionsForDestinationCemuExecutable()
        {
            string keyValue = BuildCompatibilityRegistryKeyValue();
            if (string.IsNullOrEmpty(keyValue))
                return;

            string destinationCemuExecutablePath = Path.Combine(destinationCemuInstallationPath, "Cemu.exe");
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers"))
                {
                    if (key == null)
                        throw new KeyNotFoundException("Unable to create the key.");
                    key.SetValue(destinationCemuExecutablePath, keyValue);
                }

                HandleLogMessage(
                    $"Compatibility options for {destinationCemuExecutablePath} set in the Windows Registry correctly.",
                    EventLogEntryType.Information
                );
            }
            catch (Exception exc)
            {
                HandleLogMessage(
                    $"Unable to set compatibility options for {destinationCemuExecutablePath} in the Windows Registry: {exc.Message}",
                    EventLogEntryType.Error
                );
            }
        }

        private static string BuildCompatibilityRegistryKeyValue()
        {
            string keyValue = "";
            if (Options.Migration[OptionKey.CompatibilityRunAsAdmin])
                keyValue += "RUNASADMIN ";
            if (Options.Migration[OptionKey.CompatibilityNoFullscreenOptimizations])
                keyValue += "DISABLEDXMAXIMIZEDWINDOWEDMODE ";
            if (Options.Migration[OptionKey.CompatibilityOverrideHiDPIBehaviour])
                keyValue += "HIGHDPIAWARE";
            return keyValue;
        }

        public void CreateDesktopShortcut(string cemuVersion, string mlcExternalPath)
        {
            WshShell shell = new WshShell();
            IWshShortcut shortcut = shell.CreateShortcut(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Cemu {cemuVersion}.lnk")
            );

            // Set shortcut attributes
            shortcut.TargetPath = Path.Combine(destinationCemuInstallationPath, "Cemu.exe");
            shortcut.WorkingDirectory = destinationCemuInstallationPath;
            if (mlcExternalPath != null)
                shortcut.Arguments = $"-mlc \"{mlcExternalPath}\"";
            shortcut.Description = "The Wii U emulator";

            shortcut.Save();
        }

        public void DeleteCreatedFilesAndFolders()
        {
            foreach (FileInfo copiedFile in CreatedFiles)
                copiedFile.Delete();
            foreach (DirectoryInfo copiedDir in Enumerable.Reverse(CreatedDirectories))
                copiedDir.Delete();

            HandleLogMessage("Created files and directories deleted successfully.", EventLogEntryType.Information);
        }
    }
}
