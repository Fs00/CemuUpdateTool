using System.Linq;
using System.Collections.Generic;
using System.IO;
using Env = System.Environment;
using CemuUpdateTool.Utils;

namespace CemuUpdateTool.Settings
{
    /*
     *  Options
     *  Stores program options. They can be loaded from a file (see WriteOptionsToFile() for details on the format)
     */
    static partial class Options
    {
        public static IToggleableOptionsList FoldersToMigrate { private set; get; }
        public static IToggleableOptionsList FilesToMigrate { private set; get; }
        public static IOptionsGroup<bool> Migration { private set; get; }
        public static IOptionsGroup<string> Download { private set; get; }
        public static string CustomMlcFolderPath { set; get; } = "";      // mlc01 folder's custom path for Cemu 1.10+
        public static string CurrentOptionsFilePath { set; get; } = "";

        private const string OPTIONS_FILE_NAME = "settings.dat";
        public static readonly string LocalOptionsFilePath = $@".\{OPTIONS_FILE_NAME}";
        public static readonly string AppDataOptionsFilePath = Path.Combine(Env.GetFolderPath(Env.SpecialFolder.ApplicationData),
                                                                $@"Fs00\CemuUpdateTool\{OPTIONS_FILE_NAME}");

        private static readonly IOptionsGroup<bool> defaultFoldersToMigrate;
        private static readonly IOptionsGroup<bool> defaultFilesToMigrate;
        private static readonly IOptionsGroup<bool> migrationDefaults;
        private static readonly IOptionsGroup<string> downloadDefaults;

        // Used by OptionsParser and OptionsSerializer
        private const char SECTION_MARKER = '#';

        static Options()
        {
            defaultFoldersToMigrate = new OptionsGroupDictionaryAdapter<bool>(
                new Dictionary<string, bool> {
                    { OptionsFolder.ControllerProfiles, true },
                    { OptionsFolder.GameProfiles, false },
                    { OptionsFolder.GraphicPacks, true },
                    { OptionsFolder.OldSavegames, true },
                    { OptionsFolder.Savegames, true },
                    { OptionsFolder.DLCUpdates, true },
                    { OptionsFolder.TransferableCaches, true }
                }
            );

            defaultFilesToMigrate = new OptionsGroupDictionaryAdapter<bool>(
                new Dictionary<string, bool> {
                    { OptionsFile.SettingsBin, true },       // Cemu settings file
                    { OptionsFile.SettingsXml, true }        // file containing game list data
                }
            );

            migrationDefaults = new OptionsGroupDictionaryAdapter<bool>(
                new Dictionary<string, bool> {
                    { OptionsKeys.DeleteDestinationFolderContents, false },
                    { OptionsKeys.UseCustomMlcFolderIfSupported, false },
                    { OptionsKeys.AskForDesktopShortcut, true },
                    // Compatibility options for new Cemu installation
                    { OptionsKeys.SetCompatibilityOptions, true },
                    { OptionsKeys.CompatibilityRunAsAdmin, false },
                    { OptionsKeys.CompatibilityNoFullscreenOptimizations, true },
                    { OptionsKeys.CompatibilityOverrideHiDPIBehaviour, true }
                }
            );

            downloadDefaults = new OptionsGroupDictionaryAdapter<string>(
                new Dictionary<string, string> {
                    { OptionsKeys.CemuBaseUrl, "http://cemu.info/releases/cemu_" },
                    { OptionsKeys.CemuUrlSuffix, ".zip" },
                    { OptionsKeys.LastKnownCemuVersion, "1.0.0" },
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
         *  Looks for options file in executable and in %AppData% folder
         *  Priority is given to the file in the local folder, unless preferAppDataFile is set to true
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

        /*
         *  Deletes the options file, returning true if the file existed and has been removed, otherwise false
         */
        public static bool DeleteOptionsFile()
        {
            if (!string.IsNullOrEmpty(CurrentOptionsFilePath) && File.Exists(CurrentOptionsFilePath))
            {
                File.Delete(CurrentOptionsFilePath);
                if (CurrentOptionsFilePath == AppDataOptionsFilePath)
                    DeleteFs00AppDataFolderIfEmpty();
                return true;
            }
            else
                return false;
        }

        private static void DeleteFs00AppDataFolderIfEmpty()
        {
            Directory.Delete(Path.GetDirectoryName(AppDataOptionsFilePath), recursive: true);
            string fs00AppDataFolder = Path.Combine(Env.GetFolderPath(Env.SpecialFolder.ApplicationData), "Fs00");
            if (FileUtils.DirectoryIsEmpty(fs00AppDataFolder))
                Directory.Delete(fs00AppDataFolder);
        }

        /*
         *  Iterators on default files/folders dictionaries
         */
        public static IEnumerable<string> DefaultFoldersToMigrate()
        {
            foreach (var option in defaultFoldersToMigrate)
                yield return option.Key;
        }

        public static IEnumerable<string> DefaultFilesToMigrate()
        {
            foreach (var option in defaultFilesToMigrate)
                yield return option.Key;
        }

        /*
         *  Iterators that return only custom user folders/files (the ones that aren't in default dictionary)
         */
        public static IEnumerable<string> CustomFoldersToMigrate()
        {
            return FoldersToMigrate.GetAll().Except(DefaultFoldersToMigrate());
        }

        public static IEnumerable<string> CustomFilesToMigrate()
        {
            return FilesToMigrate.GetAll().Except(DefaultFilesToMigrate());
        }
    }
}
