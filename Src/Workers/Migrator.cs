using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CemuUpdateTool.Settings;
using CemuUpdateTool.Utils;
using CemuUpdateTool.Workers.Operations;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using File = System.IO.File;

namespace CemuUpdateTool.Workers
{
    sealed class Migrator : Worker
    {
        // Necessary for restoring the original situation when operation is cancelled
        public IReadOnlyList<FileInfo> CreatedFiles => createdFiles.AsReadOnly();
        public IReadOnlyList<DirectoryInfo> CreatedDirectories => createdDirectories.AsReadOnly();
        
        private readonly List<FileInfo> createdFiles = new List<FileInfo>();
        private readonly List<DirectoryInfo> createdDirectories = new List<DirectoryInfo>();

        private readonly string sourceCemuInstallationPath;
        private readonly string destinationCemuInstallationPath;
        private readonly VersionNumber sourceCemuVersion;

        public Migrator(string sourceCemuInstallationPath,
                        string destinationCemuInstallationPath,
                        VersionNumber sourceCemuVersion,
                        CancellationToken cancToken)
               : base(cancToken)
        {
            this.sourceCemuInstallationPath = sourceCemuInstallationPath;
            this.destinationCemuInstallationPath = destinationCemuInstallationPath;
            this.sourceCemuVersion = sourceCemuVersion;
        }

        public void PerformMigrationOperations()
        {
            if (string.IsNullOrWhiteSpace(sourceCemuInstallationPath) || string.IsNullOrWhiteSpace(destinationCemuInstallationPath))
                throw new ArgumentException("Source and/or destination Cemu folder are set incorrectly!");

            OnProgressChange(0, CountAllExistingFilesToMigrate());
            
            MigrateFolders();
            MigrateFiles();

            if (Options.Migration[OptionKey.SetCompatibilityOptions])
                SetCompatibilityOptionsForDestinationCemuExecutable();
        }

        private int CountAllExistingFilesToMigrate()
        {
            int count = CountFilesInExistingFoldersToMigrate();
            count += CountExistingFilesToMigrate();
            return count;
        }
        
        private int CountFilesInExistingFoldersToMigrate()
        {
            int count = 0;
            foreach (string folderRelativePath in FoldersToActuallyMigrate())
            {
                string pathToSourceFolder = Path.Combine(sourceCemuInstallationPath, folderRelativePath);
                if (Directory.Exists(pathToSourceFolder))
                    count += FileUtils.CountFilesIncludedInDirectoryRecursively(pathToSourceFolder);
            }
            return count;
        }

        private int CountExistingFilesToMigrate()
        {
            int count = 0;
            foreach (string fileRelativePath in Options.FilesToMigrate.GetAllEnabled())
            {
                string pathToSourceFile = Path.Combine(sourceCemuInstallationPath, fileRelativePath);
                if (File.Exists(pathToSourceFile))
                    count++;
            }
            return count;
        }

        private void MigrateFolders()
        {
            foreach (string folder in FoldersToActuallyMigrate())
            {
                string sourceFolderPath = Path.Combine(sourceCemuInstallationPath, folder);
                if (!Directory.Exists(sourceFolderPath) || FileUtils.IsDirectoryEmpty(sourceFolderPath))
                    continue;

                if (Options.Migration[OptionKey.DeleteDestinationFolderContents])
                    TryDeleteDestinationFolderContents(folder);

                OnWorkStart($"Copying {folder}");
                string destinationFolderPath = Path.Combine(destinationCemuInstallationPath, folder);
                FileUtils.CopyDirectory(sourceFolderPath, destinationFolderPath, this);
            }
        }

        private void TryDeleteDestinationFolderContents(string folder)
        {
            try
            {
                string folderPath = Path.Combine(destinationCemuInstallationPath, folder);
                if (Directory.Exists(folderPath))
                {
                    OnWorkStart($"Removing destination {folder} folder previous contents");
                    FileUtils.RemoveDirectoryContents(folderPath, this);
                }
            }
            // Catch errors here since we don't want to abort the entire work if content deletion fails
            catch (Exception exc) when (!(exc is OperationCanceledException))
            {
                OnLogMessage(LogMessageType.Error, $"Unable to complete folder {folder} contents removal: {exc.Message}");
            }
        }

        private IEnumerable<string> FoldersToActuallyMigrate()
        {
            foreach (string folder in Options.FoldersToMigrate.GetAllEnabled())
            {
                if (ShouldNotCopyMlcSubfolders())
                {
                    if (!IsMlcSubfolder(folder))
                        yield return folder;
                }

                yield return folder;
            }
        }

        // Cemu 1.10 has been the first version to support custom MLC folder location,
        // so if the user has chosen to use a custom MLC folder in options and Cemu version is compatible,
        // then MLC subfolders won't be copied
        private bool ShouldNotCopyMlcSubfolders()
        {
            return Options.Migration[OptionKey.UseCustomMlcFolderIfSupported] &&
                   sourceCemuVersion >= new VersionNumber(1, 10);
        }

        private bool IsMlcSubfolder(string folder)
        {
            return folder.StartsWith(@"mlc01\");
        }

        private void MigrateFiles()
        {
            OnWorkStart("Copying files");
            foreach (string fileRelativePath in Options.FilesToMigrate.GetAllEnabled())
            {
                ThrowIfWorkIsCancelled();
                
                var sourceFile = new FileInfo(Path.Combine(sourceCemuInstallationPath, fileRelativePath));
                if (sourceFile.Exists)
                    sourceFile.CopyToAndReportOutcomeToWorker(Path.Combine(destinationCemuInstallationPath, fileRelativePath), this);
                else
                    OnLogMessage(LogMessageType.Information,
                                 $"File {fileRelativePath} doesn't exist in source Cemu installation: skipped.");
            }
        }

        private void SetCompatibilityOptionsForDestinationCemuExecutable()
        {
            string keyValue = BuildCompatibilityRegistryKeyValue();
            if (string.IsNullOrEmpty(keyValue))
                return;
            TryWriteCompatibilityRegistryKey(keyValue);
        }

        private void TryWriteCompatibilityRegistryKey(string keyValue)
        {
            string destinationCemuExecutablePath = Path.Combine(destinationCemuInstallationPath, "Cemu.exe");
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers"))
                {
                    if (key == null)
                        throw new KeyNotFoundException("Unable to create the key.");
                    key.SetValue(destinationCemuExecutablePath, keyValue);
                }

                OnLogMessage(LogMessageType.Information,
                    $"Compatibility options for {destinationCemuExecutablePath} set in the Windows Registry correctly.");
            }
            catch (Exception exc)
            {
                OnLogMessage(LogMessageType.Error,
                    $"Unable to set compatibility options for {destinationCemuExecutablePath} in the Windows Registry: {exc.Message}");
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

        public void CreateDesktopShortcut(VersionNumber cemuVersion)
        {
            WshShell shell = new WshShell();
            IWshShortcut shortcut = shell.CreateShortcut(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Cemu {cemuVersion}.lnk")
            );
            shortcut.Description = "The Wii U emulator";
            shortcut.TargetPath = Path.Combine(destinationCemuInstallationPath, "Cemu.exe");
            shortcut.WorkingDirectory = destinationCemuInstallationPath;
            if (cemuVersion >= new VersionNumber(1, 10) && Options.Migration[OptionKey.UseCustomMlcFolderIfSupported])
                shortcut.Arguments = $@"-mlc ""{Options.CustomMlcFolderPath}""";
            
            shortcut.Save();
        }

        public void DeleteCreatedFilesAndFolders()
        {
            foreach (FileInfo copiedFile in CreatedFiles)
                copiedFile.Delete();
            foreach (DirectoryInfo copiedDir in CreatedDirectories.Reverse())
                copiedDir.Delete();

            OnLogMessage(LogMessageType.Information, "Created files and directories deleted successfully.");
        }

        public override void OnOperationSuccess(Operation operationInfo)
        {
            base.OnOperationSuccess(operationInfo);
            switch (operationInfo)
            {
                case FileCopyOperation fileCopyOperationInfo:
                    createdFiles.Add(fileCopyOperationInfo.SourceFile);
                    OnProgressIncrement(1);
                    break;
                case DirectoryCreationOperation directoryCreationOperationInfo:
                    createdDirectories.Add(directoryCreationOperationInfo.DirectoryToCreate);
                    break;
            }
        }

        public override void OnOperationErrorHandled(Operation operationInfo, string errorMessage)
        {
            base.OnOperationErrorHandled(operationInfo, errorMessage);
            switch (operationInfo)
            {
                case FileCopyOperation _:
                    OnProgressIncrement(1);
                    break;
            }
        }
    }
}
