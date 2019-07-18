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

        public Migrator(string sourceCemuInstallationPath,
                        string destinationCemuInstallationPath,
                        IList<string> foldersToCopy,
                        IList<string> filesToCopy,
                        CancellationToken cancToken,
                        Action<string, bool> LoggerDelegate)
               : base(cancToken, LoggerDelegate)
        {
            this.sourceCemuInstallationPath = sourceCemuInstallationPath;
            this.destinationCemuInstallationPath = destinationCemuInstallationPath;

            // Populate folders/file tuple arrays and calculate their sizes
            this.foldersToCopy = new (string, long)[foldersToCopy.Count];
            for (int i = 0; i < foldersToCopy.Count; i++)
                this.foldersToCopy[i] = (foldersToCopy[i], 0L);

            this.filesToCopy = new (string, long)[filesToCopy.Count];
            for (int i = 0; i < filesToCopy.Count; i++)
                this.filesToCopy[i] = (filesToCopy[i], 0L);
            CalculateSizes();
        }

        // ValueTuple arrays containing names and sizes of the files and folders to be copied
        (string Name, long Size)[] foldersToCopy;
        (string Name, long Size)[] filesToCopy;

        /*
         *  Calculates the size of every folder and file to copy
         */
        private void CalculateSizes()
        {
            // Calculate the size of every folder to copy
            for (int i = 0; i < foldersToCopy.Length; i++)
                foldersToCopy[i].Size = FileUtils.CalculateDirSize(Path.Combine(sourceCemuInstallationPath, foldersToCopy[i].Name), HandleLogMessage);

            // Calculate the size of every file to copy
            for (int i = 0; i < filesToCopy.Length; i++)
            {
                string filePath = Path.Combine(sourceCemuInstallationPath, filesToCopy[i].Name);
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
            foreach (var folder in foldersToCopy)
            {
                if (Options.Migration[OptionKey.DeleteDestinationFolderContents])
                {
                    string destFolderPath = Path.Combine(destinationCemuInstallationPath, folder.Name);
                    if (FileUtils.DirectoryExists(destFolderPath))
                    {
                        PerformingWork($"Removing destination {folder.Name} folder previous contents");
                        try
                        {
                            FileUtils.RemoveDirectoryContents(destFolderPath, HandleLogMessage, cancToken);
                        }
                        // Catch errors here since we don't want to abort the entire work if content deletion fails
                        catch (Exception exc) when (!(exc is OperationCanceledException))
                        {
                            HandleLogMessage($"Unable to complete folder {folder.Name} contents removal: {exc.Message}", EventLogEntryType.Error);
                        }
                    }
                }

                if (folder.Size > 0)     // avoiding to copy empty/unexisting folders
                {
                    PerformingWork($"Copying {folder.Name}");
                    FileUtils.CopyDirectory(Path.Combine(sourceCemuInstallationPath, folder.Name), Path.Combine(destinationCemuInstallationPath, folder.Name), HandleLogMessage,
                                      cancToken, progressHandler, CreatedFiles, CreatedDirectories);
                }
            }
        }
        private void MigrateFiles(Action<string> PerformingWork, IProgress<long> progressHandler)
        {
            PerformingWork("Copying files");
            foreach (var fileToCopy in filesToCopy)
            {
                cancToken.ThrowIfCancellationRequested();
                if (fileToCopy.Size > 0)
                {
                    var file = new FileInfo(Path.Combine(sourceCemuInstallationPath, fileToCopy.Name));
                    file.CopyTo(Path.Combine(destinationCemuInstallationPath, fileToCopy.Name), HandleLogMessage, progressHandler, CreatedFiles);
                }
                else
                    HandleLogMessage($"File {fileToCopy.Name} empty or unexisting: skipped.", EventLogEntryType.Warning);
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
