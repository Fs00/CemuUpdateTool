using System;
using System.Text;
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

            if ((localFileExists = FileUtils.FileExists(LOCAL_FILEPATH)) || FileUtils.FileExists(APPDATA_FILEPATH))
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

                MyStreamReader optionsFile = new MyStreamReader(optionsFilePath);
                try
                {
                    // Check if the file is empty
                    if (optionsFile.BaseStream.Length == 0)
                        throw new InvalidDataException("Options file is empty.");

                    // Start reading
                    byte sectionId;
                    string sectionHeaderLine = null;
                    while (!optionsFile.EndOfStream)
                    {
                        if (optionsFile.Peek() == 35)   // if the next char is '#'
                        {
                            sectionHeaderLine = optionsFile.ReadLine();
                            // Check if the sectionId is a number
                            if (!byte.TryParse(sectionHeaderLine.TrimStart('#'), out sectionId))
                                throw new FormatException("Section ID is not a number");
                            else
                                ReadFileSection(optionsFile, sectionId);
                        }
                        else
                            optionsFile.ReadLine();
                    }

                    if (sectionHeaderLine == null)      // if no lines starting with '#' have been encountered
                        throw new InvalidDataException("Options file didn't contain any useful information.");
                }
                catch (Exception exc)
                {
                    string message;
                    if (exc is InvalidDataException)
                        message = exc.Message;
                    else
                        message = $"An unexpected error occurred when parsing options file: {exc.Message.TrimEnd('.')} at line {optionsFile.LineCount}.";

                    MessageBox.Show(message + "\r\nDefault settings will be loaded instead.", "Error in settings.dat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
         *  Reads an options file section and fills the corresponding dictionary/variable
         *  It reads until it finds the end of file or another section header. The StreamReader must be positioned in the line under the section header.
         *  These are the sections ids:
         *      0: folderOptions
         *      1: migrationOptions
         *      2: downloadOptions
         *      3: mlcFolderExternalPath (only one line is read)
         */
        private void ReadFileSection(StreamReader fileStream, byte sectionId)
        {
            // Check if the sectionId is valid
            if (sectionId < 0 || sectionId > 3)
                throw new ArgumentOutOfRangeException("Section ID is not valid");

            // Set up the dynamic dictionary "pointer" according to the sectionId
            dynamic dictionary = null;
            if (sectionId == 0)
                dictionary = folderOptions;
            else if (sectionId == 1)
                dictionary = migrationOptions;
            else if (sectionId == 2)
                dictionary = downloadOptions;

            int startingChar;       // first char code of the current reading line
            string[] parsedLine;    // the "splitted" line

            // Continue reading until you find a '#' or the EOF
            while ((startingChar = fileStream.Peek()) != 35 && startingChar != -1)
            {
                // If there's an empty line (13-CR, 10-LF), just skip to the next one
                if (startingChar == 13 || startingChar == 10)
                    fileStream.ReadLine();
                else
                {
                    if (sectionId <= 2)         // if I'm handling a dictionary
                    {
                        parsedLine = fileStream.ReadLine().Split(',');
                        if (parsedLine.Length != 2)
                            throw new FormatException("Not a \"key, value\" option");

                        if (sectionId <= 1)     // if I'm handling a <string, bool> dictionary (folderOptions, migrationOptions)
                            dictionary.Add(parsedLine[0], Convert.ToBoolean(parsedLine[1]));
                        else                    // if I'm handling a <string, string> dictionary (downloadOptions)
                            dictionary.Add(parsedLine[0], parsedLine[1]);
                    }
                    else                        // section 3
                    {
                        string tmpPath = fileStream.ReadLine();
                        if (tmpPath.IndexOfAny(Path.GetInvalidPathChars()) == -1)
                        {
                            mlcFolderExternalPath = tmpPath;
                            return;     // for this section there aren't any other things to read
                        }
                        else
                            throw new FormatException("Mlc01 external folder path is malformed");
                    }
                }
            }
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
            StringBuilder dataToWrite = new StringBuilder();

            // Write folder options
            dataToWrite.AppendLine("#0");
            foreach (KeyValuePair<string, bool> option in folderOptions)
                dataToWrite.AppendLine($"{option.Key},{option.Value}");
            // Write additional options
            dataToWrite.AppendLine("#1");
            foreach (KeyValuePair<string, bool> option in migrationOptions)
                dataToWrite.AppendLine($"{option.Key},{option.Value}");
            // Write download options
            dataToWrite.AppendLine("#2");
            foreach (KeyValuePair<string, string> option in downloadOptions)
                dataToWrite.AppendLine($"{option.Key},{option.Value}");
            // Write mlc01 custom folder path
            if (mlcFolderExternalPath != "")
                dataToWrite.Append("#3\r\n" + mlcFolderExternalPath);   // \r\n -> CR-LF

            try
            {
                // Create destination directory if it doesn't exist
                string optionsFileDir = Path.GetDirectoryName(optionsFilePath);
                if (!FileUtils.DirectoryExists(optionsFileDir))
                    Directory.CreateDirectory(optionsFileDir);

                // Write string on file overwriting any existing content
                File.WriteAllText(optionsFilePath, dataToWrite.ToString());
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
            if (!string.IsNullOrEmpty(optionsFilePath) && FileUtils.FileExists(optionsFilePath))
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

    /*
     *  Custom StreamReader derived class that holds the current number of read lines
     *  LineCount works only with ReadLine(), since it's the only method I use for reading the stream.
     */
    class MyStreamReader : StreamReader
    {
        public MyStreamReader(string path) : base(path) {}

        public int LineCount { private set; get; }

        public override string ReadLine()
        {
            string result = base.ReadLine();
            if (result != null)
                LineCount++;
            return result;
        }
    }
}
