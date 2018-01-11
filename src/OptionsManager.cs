using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    public class OptionsManager
    {
        public Dictionary<string, bool> folderOptions { set; get; }      // contains a list of Cemu subfolders and whether they have to be copied
        public Dictionary<string, bool> migrationOptions { set; get; }   // contains a set of additional options for the migration
        public Dictionary<string, string> downloadOptions { set; get; }  // contains a set of options for the download of Cemu versions
        public string mlcFolderExternalPath { get; set; } = "";          // mlc01 folder's external path for Cemu 1.10+
        public string optionsFilePath { set; get; } = "";                // the path of the settings file

        // Default options for every dictionary
        Dictionary<string, bool> defaultFolderOptions = new Dictionary<string, bool> {
            { "controllerProfiles", true },
            { "gameProfiles", false },
            { "graphicPacks", true },
            { @"mlc01\emulatorSave", true },       // savegame directory before 1.11
            { @"mlc01\usr\save", true },           // savegame directory since 1.11
            { @"mlc01\usr\title", true },
            { @"shaderCache\transferable", true }
        };
        Dictionary<string, bool> defaultMigrationOptions = new Dictionary<string, bool> {
            { "copyCemuSettingsFile", true },
            { "deleteDestFolderContents", false },
            { "dontCopyMlcFolderFor1.10+", false },
            { "askForDesktopShortcut", true }
        };
        Dictionary<string, string> defaultDownloadOptions = new Dictionary<string, string> {
            { "cemuBaseUrl", "http://cemu.info/releases/cemu_" },
            { "cemuUrlSuffix", ".zip" },
            { "lastKnownCemuVersion", "1.0.0" },
        };

        // Useful constants to make code more clean & readable
        public readonly string LOCAL_FILEPATH = @".\settings.dat";
        public readonly string APPDATA_FILEPATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Fs00\CemuUpdateTool\settings.dat");

        /*
         *  Executed at application startup.
         *  If the program reads options from file successfully, it checks for additionalOptions missing entries. Otherwise, default options are set.
         */
        public OptionsManager()
        {
            if (ReadOptionsFromFile() == false)
                SetDefaultOptions();
            else
                CheckForMissingEntries();
        }

        /*
         *  Method that populates options dictionaries reading settings file
         *  If there's no options file or an error happens when parsing it, SetDefaultOptions() is called instead.
         *  Priority is given to the file in the local folder.
         */
        public bool ReadOptionsFromFile()
        {
            bool localFileExists;
            bool readingOutcome = true;
            string line;
            string[] parsedLine;

            if ((localFileExists = FileOperations.FileExists(LOCAL_FILEPATH)) || FileOperations.FileExists(APPDATA_FILEPATH))
            {
                // Create the dictionaries here so we are sure that OptionsForm won't throw NullReferenceException if there aren't any options in the file
                folderOptions = new Dictionary<string, bool>();
                migrationOptions = new Dictionary<string, bool>();
                downloadOptions = new Dictionary<string, string>();

                // Set the file path property according to the current file position
                if (localFileExists)
                    optionsFilePath = LOCAL_FILEPATH;
                else
                    optionsFilePath = APPDATA_FILEPATH;

                StreamReader optionsFile = new StreamReader(optionsFilePath);
                try
                {
                    // Retrieve folder options from file
                    while ((line = optionsFile.ReadLine()) != "##")
                    {
                        parsedLine = line.Split(',');
                        folderOptions.Add(parsedLine[0], Convert.ToBoolean(parsedLine[1]));
                    }

                    if (!optionsFile.EndOfStream)
                    {
                        // Retrieve additional options from file
                        while ((line = optionsFile.ReadLine()) != "###")
                        {
                            parsedLine = line.Split(',');
                            migrationOptions.Add(parsedLine[0], Convert.ToBoolean(parsedLine[1]));
                        }

                        // Retrieve custom mlc01 folder path if present
                        if (!optionsFile.EndOfStream)
                            mlcFolderExternalPath = optionsFile.ReadLine();
                    }
                }
                catch(Exception exc)
                {
                    MessageBox.Show("An unexpected error occurred when parsing options file: " + exc.Message + " Last character read: " + optionsFile.BaseStream.Position +
                        ".\r\nDefault settings will be loaded instead.", "Error in settings.dat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    readingOutcome = false;
                }
                finally
                {
                    optionsFile.Close();
                }
                return readingOutcome;
            }
            else       // if there's no option file, optionsFilePath remains at its default value ("")
                return false;
        }

        /*
         *  Method that checks if migrationOptions and downloadOptions have all the needed entries.
         *  If an entry is not found, it is created with default option.
         *  Called only if ReadOptionsFromFile() terminates successfully.
         */
        public void CheckForMissingEntries()
        {
            foreach (string key in defaultMigrationOptions.Keys)
            {
                if (!migrationOptions.ContainsKey(key))
                    migrationOptions.Add(key, defaultMigrationOptions[key]);
            }

            foreach (string key in defaultDownloadOptions.Keys)
            {
                if (!downloadOptions.ContainsKey(key))
                    downloadOptions.Add(key, defaultDownloadOptions[key]);
            }
        }

        /*
         *  Method that sets options dictionaries to their default values
         *  Called if settings.dat is not found or ReadOptionsFromFile() throws an error
         */
        public void SetDefaultOptions()
        {
            folderOptions = new Dictionary<string, bool>(defaultFolderOptions);
            migrationOptions = new Dictionary<string, bool>(defaultMigrationOptions);
            downloadOptions = new Dictionary<string, string>(defaultDownloadOptions);
            mlcFolderExternalPath = "";
        }

        /*
         *  Writes all options to file.
         *  Options are saved one per line, in the following format: key,value
         *  Folder options terminate with "##", additional options with "###"
         */
        public void WriteOptionsToFile()
        {
            string dataToWrite = "";

            // Write folder options
            foreach (KeyValuePair<string, bool> option in folderOptions)
                dataToWrite += option.Key + "," + option.Value + "\r\n";        // \r\n -> CR-LF
            dataToWrite += "##\r\n";
            // Write additional options
            foreach (KeyValuePair<string, bool> option in migrationOptions)
                dataToWrite += option.Key + "," + option.Value + "\r\n";
            dataToWrite += "###";
            // Write mlc01 custom folder path
            if (mlcFolderExternalPath != "")
                dataToWrite += "\r\n" + mlcFolderExternalPath;

            try
            {
                // Create destination directory if it doesn't exist
                string optionsFileDir = Path.GetDirectoryName(optionsFilePath);
                if (!FileOperations.DirectoryExists(optionsFileDir))
                    Directory.CreateDirectory(optionsFileDir);

                // Write string on file overwriting any existing content
                File.WriteAllText(optionsFilePath, dataToWrite);
            }
            catch(Exception exc)
            {
                MessageBox.Show("An unexpected error occurred when saving options file: " + exc.Message +
                        "\r\nOptions won't be preserved after closing the program.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /*
         *  Deletes the options file, returning true if the file existed and has been removed, otherwise false
         */
        public bool DeleteOptionsFile()
        {
            if (!string.IsNullOrEmpty(optionsFilePath) && FileOperations.FileExists(optionsFilePath))
            {
                File.Delete(optionsFilePath);
                if (optionsFilePath == APPDATA_FILEPATH)    // clean redundant empty folders
                    Directory.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Fs00"), true);
                return true;
            }
            else
                return false;

        }

        /*
         *  Method that returns a list containing the paths of the folders which have to be copied
         */
        public List<string> GetFoldersToCopy(bool cemuVersionIsAtLeast110)
        {
            List<string> foldersToCopy = new List<string>();

            // Ignore mlc01 subfolders if source Cemu version is at least 1.10 and custom mlc folder option is selected
            if (cemuVersionIsAtLeast110 && migrationOptions["dontCopyMlcFolderFor1.10+"] == true)
            {
                foreach (string folder in SelectedFolders())
                {
                    if (!folder.StartsWith(@"mlc01\"))
                        foldersToCopy.Add(folder);
                }
            }
            else    // otherwise append folders without any extra check 
            {
                foreach (string folder in SelectedFolders())
                    foldersToCopy.Add(folder);
            }
            
            return foldersToCopy;
        }

        /*
         *  Iterator method that returns a folder path every iteration only if the corresponding option is selected
         */
        public IEnumerable<string> SelectedFolders()
        {
            foreach(KeyValuePair<string, bool> option in folderOptions)
            {
                if (option.Value == true)
                    yield return option.Key;
            }
        }
    }
}
