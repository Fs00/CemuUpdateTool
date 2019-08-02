using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Env = System.Environment;
using CemuUpdateTool.Utils;

namespace CemuUpdateTool.Settings
{
    /*
     *  Provides global access to the options of the application.
     *  By default, the class is initialized with default settings.
     *  If CurrentOptionsFilePath is set, options can be read from and written to the specified file.
     */
    static partial class Options
    {
        public static IToggleableOptionsList FoldersToMigrate { private set; get; }
        public static IToggleableOptionsList FilesToMigrate { private set; get; }
        public static IOptionsGroup<bool> Migration { private set; get; }
        public static IOptionsGroup<string> Download { private set; get; }
        public static string CustomMlcFolderPath { set; get; } = "";    // mlc01 folder's custom path for Cemu 1.10+
        public static string CurrentOptionsFilePath { set; get; } = "";

        private const string OPTIONS_FILE_NAME = "settings.dat";
        public static readonly string LocalOptionsFilePath = $@".\{OPTIONS_FILE_NAME}";
        public static readonly string AppDataOptionsFilePath = Path.Combine(Env.GetFolderPath(Env.SpecialFolder.ApplicationData),
                                                                "Fs00", "CemuUpdateTool", OPTIONS_FILE_NAME);

        private static readonly IToggleableOptionsList defaultFoldersToMigrate;
        private static readonly IToggleableOptionsList defaultFilesToMigrate;
        private static readonly IOptionsGroup<bool> migrationDefaults;
        private static readonly IOptionsGroup<string> downloadDefaults;

        // Used by OptionsParser and OptionsSerializer
        private const char SECTION_MARKER = '#';

        static Options()
        {
            defaultFoldersToMigrate = new ToggleableOptionsListDictionaryAdapter(
                new Dictionary<string, bool> {
                    { FolderOption.ControllerProfiles, true },
                    { FolderOption.GameProfiles, false },
                    { FolderOption.GraphicPacks, true },
                    { FolderOption.GameSavesBeforeCemu1_11, true },
                    { FolderOption.GameSavesAfterCemu1_11, true },
                    { FolderOption.DLCAndUpdates, true },
                    { FolderOption.TransferableCaches, true }
                }
            );

            defaultFilesToMigrate = new ToggleableOptionsListDictionaryAdapter(
                new Dictionary<string, bool> {
                    { FileOption.SettingsBin, true },
                    { FileOption.SettingsXml, true }
                }
            );

            migrationDefaults = new OptionsGroupDictionaryAdapter<bool>(
                new Dictionary<string, bool> {
                    { OptionKey.DeleteDestinationFolderContents, false },
                    { OptionKey.UseCustomMlcFolderIfSupported, false },
                    { OptionKey.AskForDesktopShortcut, true },
                    { OptionKey.SetCompatibilityOptions, true },
                    { OptionKey.CompatibilityRunAsAdmin, false },
                    { OptionKey.CompatibilityNoFullscreenOptimizations, true },
                    { OptionKey.CompatibilityOverrideHiDPIBehaviour, true }
                }
            );

            downloadDefaults = new OptionsGroupDictionaryAdapter<string>(
                new Dictionary<string, string> {
                    { OptionKey.CemuBaseUrl, "http://cemu.info/releases/cemu_" },
                    { OptionKey.CemuUrlSuffix, ".zip" },
                    { OptionKey.LastKnownCemuVersion, "1.0.0" },
                }
            );

            ApplyDefaultOptions();
        }

        public static void ApplyDefaultOptions()
        {
            FoldersToMigrate = new ToggleableOptionsListDictionaryAdapter(
                defaultFoldersToMigrate.ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value)
            );
            FilesToMigrate = new ToggleableOptionsListDictionaryAdapter(
                defaultFilesToMigrate.ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value)
            );
            Migration = new OptionsGroupDictionaryAdapter<bool>(
                migrationDefaults.ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value)
            );
            Download = new OptionsGroupDictionaryAdapter<string>(
                downloadDefaults.ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value)
            );
            CustomMlcFolderPath = "";
        }

        /*
         *  Looks for options file and sets CurrentOptionsFilePath with the path to the found file.
         *  Priority is given to the file in the local folder, unless preferAppDataFile is set to true.
         */
        public static bool OptionsFileFound(bool preferAppDataFile = false)
        {
            bool localFileExists = File.Exists(LocalOptionsFilePath);
            bool appDataFileExists = File.Exists(AppDataOptionsFilePath);
            if (localFileExists || appDataFileExists)
            {
                if (preferAppDataFile)
                {
                    if (appDataFileExists)
                        CurrentOptionsFilePath = AppDataOptionsFilePath;
                    else
                        CurrentOptionsFilePath = LocalOptionsFilePath;
                }
                else
                {
                    if (localFileExists)
                        CurrentOptionsFilePath = LocalOptionsFilePath;
                    else
                        CurrentOptionsFilePath = AppDataOptionsFilePath;
                }
                return true;
            }

            CurrentOptionsFilePath = "";
            return false;
        }

        public static void LoadFromCurrentlySelectedFile()
        {
            if (string.IsNullOrEmpty(CurrentOptionsFilePath))
                throw new InvalidOperationException("Options file path has not been set.");

            var parser = new OptionsParser();
            parser.ReadFromFile(CurrentOptionsFilePath);
            parser.ApplyParsedOptions();
        }

        public static void WriteOptionsToCurrentlySelectedFile()
        {
            if (!string.IsNullOrEmpty(CurrentOptionsFilePath))
            {
                string serializedOptions = new OptionsSerializer().Serialize();

                string optionsFileDir = Path.GetDirectoryName(CurrentOptionsFilePath);
                if (!Directory.Exists(optionsFileDir))
                    Directory.CreateDirectory(optionsFileDir);

                File.WriteAllText(CurrentOptionsFilePath, serializedOptions);
            }
        }
        
        public static void DeleteOptionsFile()
        {
            if (!string.IsNullOrEmpty(CurrentOptionsFilePath))
            {
                File.Delete(CurrentOptionsFilePath);
                if (CurrentOptionsFilePath == AppDataOptionsFilePath)
                    DeleteFs00AppDataFolderIfEmpty();
                CurrentOptionsFilePath = "";
            }
        }

        private static void DeleteFs00AppDataFolderIfEmpty()
        {
            Directory.Delete(Path.GetDirectoryName(AppDataOptionsFilePath), recursive: true);
            string fs00AppDataFolder = Path.Combine(Env.GetFolderPath(Env.SpecialFolder.ApplicationData), "Fs00");
            if (FileUtils.IsDirectoryEmpty(fs00AppDataFolder))
                Directory.Delete(fs00AppDataFolder);
        }

        /*
         *  Iterators to avoid breaking encapsulation
         */
        public static IEnumerable<string> AllDefaultFoldersToMigrate()
        {
            return defaultFoldersToMigrate.GetAll();
        }

        public static IEnumerable<string> AllDefaultFilesToMigrate()
        {
            return defaultFilesToMigrate.GetAll();
        }

        public static IEnumerable<string> AllCustomFoldersToMigrate()
        {
            return FoldersToMigrate.GetAll().Except(AllDefaultFoldersToMigrate());
        }

        public static IEnumerable<string> AllCustomFilesToMigrate()
        {
            return FilesToMigrate.GetAll().Except(AllDefaultFilesToMigrate());
        }
    }
}
