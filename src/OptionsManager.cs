using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace CemuUpdateTool
{
    public class OptionsManager
    {
        public Dictionary<string, bool> FolderOptions     { private set; get; }     // contains a list of Cemu subfolders and whether they have to be copied
        public Dictionary<string, bool> FileOptions       { private set; get; }     // contains a list of files included in Cemu folder and whether they have to be copied
        public Dictionary<string, bool> MigrationOptions  { private set; get; }     // contains a set of additional options for the migration
        public Dictionary<string, string> DownloadOptions { private set; get; }     // contains a set of options for the download of Cemu versions
        public string MlcFolderExternalPath { set; get; } = "";                     // mlc01 folder's external path for Cemu 1.10+
        public string OptionsFilePath       { set; get; }                           // the path of the settings file (is empty string when no file is used)

        // Default options for every dictionary
        Dictionary<string, bool> defaultFolderOptions = new Dictionary<string, bool> {
            { "controllerProfiles", true },
            { "gameProfiles", false },
            { "graphicPacks", true },
            { @"mlc01\emulatorSave", true },       // savegame directory before 1.11
            { @"mlc01\usr\save", true },           // savegame directory since 1.11
            { @"mlc01\usr\title", true },          // DLC/updates directory
            { @"shaderCache\transferable", true }
        };
        Dictionary<string, bool> defaultFileOptions = new Dictionary<string, bool> {
            { "settings.bin", true },       // Cemu settings file
            { "settings.xml", true }        // file containing game list data
        };
        Dictionary<string, bool> defaultMigrationOptions = new Dictionary<string, bool> {
            { "deleteDestFolderContents", false },
            { "dontCopyMlcFolderFor1.10+", false },
            { "askForDesktopShortcut", true },
            // Compatibility options for new Cemu installation
            { "setCompatibilityOptions", true },
            { "compatOpts_runAsAdmin", false },
            { "compatOpts_noFullscreenOptimizations", true },
            { "compatOpts_overrideHiDPIBehaviour", true }
        };
        Dictionary<string, string> defaultDownloadOptions = new Dictionary<string, string> {
            { "cemuBaseUrl", "http://cemu.info/releases/cemu_" },
            { "cemuUrlSuffix", ".zip" },
            { "lastKnownCemuVersion", "1.0.0" },
        };

        // Useful constants to make code more clean & readable
        public static readonly string LocalFilePath = @".\settings.dat";
        public static readonly string AppDataFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Fs00\CemuUpdateTool\settings.dat");
        // Parsing constants
        const int SECTION_HEADER_CHAR = 35,    // '#'
                  CR = 13, LF = 10,
                  EOF = -1;

        /*
         *  Default constructor.
         *  Creates a new instance with default options
         */
        public OptionsManager() : this("") { }

        /*
         *  Overloaded constructor used to load options from a custom position.
         *  The given path must be correct and pointing to an existing file.
         */
        public OptionsManager(string optionsFilePath)
        {
            OptionsFilePath = optionsFilePath;
            if (string.IsNullOrEmpty(optionsFilePath))
                SetDefaultOptions();
            else
                ReadOptionsFromFile();
        }

        /*
         *  Looks for options file in executable and in %AppData% folder and returns the path to the chosen file
         *  Priority is given to the file in the local folder, unless preferAppDataFile is set to true
         */
        public static string LookForOptionsFile(bool preferAppDataFile = false)
        {
            bool localFileExists = FileUtils.FileExists(LocalFilePath);
            bool appDataFileExists = FileUtils.FileExists(AppDataFilePath);
            if (localFileExists || appDataFileExists)
            {
                if (preferAppDataFile)
                {
                    if (appDataFileExists)
                        return AppDataFilePath;
                    else
                        return LocalFilePath;
                }
                else
                {
                    if (localFileExists)
                        return LocalFilePath;
                    else
                        return AppDataFilePath;
                }
            }
            return "";
        }

        /*
         *  Reads options from the file specified in optionsFilePath
         *  Parsed options are saved in local variables before being copied into properties
         *  After parsing, dictionaries are checked to avoid missing entries.
         */
        public void ReadOptionsFromFile()
        {
            // Parsed options will be put in this local variables and copied into properties only if they're correct
            var folderOptions = new Dictionary<string, bool>();
            var fileOptions = new Dictionary<string, bool>();
            var migrationOptions = new Dictionary<string, bool>();
            var downloadOptions = new Dictionary<string, string>();
            string mlcFolderExternalPath = "";

            if (string.IsNullOrEmpty(OptionsFilePath))      // should never happen
                throw new InvalidOperationException("Options file path is not specified.");
            
            using (MyStreamReader optionsFile = new MyStreamReader(OptionsFilePath))
            {
                // Check if the file is empty
                if (optionsFile.BaseStream.Length == 0)
                    throw new InvalidDataException("Options file is empty.");

                string sectionHeaderLine = null;
                try
                {
                    // Start reading
                    while (!optionsFile.EndOfStream)
                    {
                        if (optionsFile.Peek() == SECTION_HEADER_CHAR)
                        {
                            sectionHeaderLine = optionsFile.ReadLine();
                            // Check if the sectionId is a number
                            if (!byte.TryParse(sectionHeaderLine.TrimStart('#'), out byte sectionId))
                                throw new OptionsParsingException("Section ID is not a number", optionsFile.CurrentLine);
                            else
                            {
                                /*
                                 *  PARSE FILE SECTION
                                 *  The StreamReader is now positioned in the line under the section header.
                                 *  The iteration stops when end of file or another section header is found.
                                 *  These are the sections ids:
                                 *      0: folderOptions
                                 *      1: migrationOptions
                                 *      2: downloadOptions
                                 *      3: mlcFolderExternalPath (only one line is read)
                                 *      4: fileOptions
                                 */

                                // Check if the sectionId is valid
                                if (sectionId < 0 || sectionId > 4)
                                    throw new OptionsParsingException("Section ID is not valid", optionsFile.CurrentLine);

                                int startingChar;       // first char code of the current reading line
                                string[] parsedLine;    // the "splitted" line

                                // Continue reading until you find a '#' or the EOF
                                while ((startingChar = optionsFile.Peek()) != SECTION_HEADER_CHAR && startingChar != EOF)
                                {
                                    // If there's an empty line, just skip to the next one
                                    if (startingChar == CR || startingChar == LF)
                                        optionsFile.ReadLine();
                                    else
                                    {
                                        if (sectionId == 3)
                                        {
                                            string tmpPath = optionsFile.ReadLine();
                                            if (tmpPath.IndexOfAny(Path.GetInvalidPathChars()) == -1)
                                            {
                                                mlcFolderExternalPath = tmpPath;
                                                break;     // for this section there aren't any other things to read
                                            }
                                            else
                                                throw new OptionsParsingException("Mlc01 external folder path is malformed", optionsFile.CurrentLine);
                                        }
                                        else     // I'm handling a dictionary
                                        {
                                            parsedLine = optionsFile.ReadLine().Split(',');
                                            if (parsedLine.Length != 2)
                                                throw new OptionsParsingException("Not a \"key, value\" option", optionsFile.CurrentLine);

                                            switch (sectionId)
                                            {
                                                // <string, bool> dictionaries (folderOptions, migrationOptions, fileOptions)
                                                case 0:
                                                    folderOptions.Add(parsedLine[0], Convert.ToBoolean(parsedLine[1]));
                                                    break;
                                                case 1:
                                                    migrationOptions.Add(parsedLine[0], Convert.ToBoolean(parsedLine[1]));
                                                    break;
                                                case 4:
                                                    fileOptions.Add(parsedLine[0], Convert.ToBoolean(parsedLine[1]));
                                                    break;
                                                // <string, string> dictionary (downloadOptions)
                                                case 2:
                                                    downloadOptions.Add(parsedLine[0], parsedLine[1]);
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }   // end of file section parsing
                        }
                        else
                            optionsFile.ReadLine();     // if the line read is not a section header, just ignore it and continue
                    }
                }
                catch (Exception exc)   // wrap any parsing exception in an OptionsParsingException containing the current line number
                {
                    if (!(exc is OptionsParsingException))
                        exc = new OptionsParsingException(exc.Message, optionsFile.CurrentLine);
                    throw exc;
                }

                if (sectionHeaderLine == null)      // if no lines starting with '#' have been encountered
                    throw new InvalidDataException("Options file didn't contain any useful information.");

                // Since parsing has been successful, we can now copy parsed options into properties
                FolderOptions = folderOptions;
                FileOptions = fileOptions;
                MigrationOptions = migrationOptions;
                DownloadOptions = downloadOptions;
                MlcFolderExternalPath = mlcFolderExternalPath;

                // Check for missing options in file
                CheckForMissingEntries();
            }
        }

        /*
         *  Method that checks if options dictionaries have all the needed entries.
         *  If an entry is not found, it is created with default option.
         *  Called only if ReadOptionsFromFile() terminates successfully.
         */
        private void CheckForMissingEntries()
        {
            foreach (string key in defaultFolderOptions.Keys)
            {
                if (!FolderOptions.ContainsKey(key))
                    FolderOptions.Add(key, false);    // if a folder was not found, it means that it shouldn't be copied
            }

            foreach (string key in defaultFileOptions.Keys)
            {
                if (!FileOptions.ContainsKey(key))
                    FileOptions.Add(key, false);    // if a file was not found, it means that it shouldn't be copied
            }

            foreach (string key in defaultMigrationOptions.Keys)
            {
                if (!MigrationOptions.ContainsKey(key))
                    MigrationOptions.Add(key, defaultMigrationOptions[key]);
            }

            foreach (string key in defaultDownloadOptions.Keys)
            {
                if (!DownloadOptions.ContainsKey(key))
                    DownloadOptions.Add(key, defaultDownloadOptions[key]);
            }
        }

        /*
         *  Method that sets options dictionaries to their default values
         */
        public void SetDefaultOptions()
        {
            FolderOptions = new Dictionary<string, bool>(defaultFolderOptions);
            FileOptions = new Dictionary<string, bool>(defaultFileOptions);
            MigrationOptions = new Dictionary<string, bool>(defaultMigrationOptions);
            DownloadOptions = new Dictionary<string, string>(defaultDownloadOptions);
            MlcFolderExternalPath = "";
        }

        /*
         *  Writes all options to file.
         *  Options are divided in sections, which contain one option per line, in the following format: key,value
         */
        public void WriteOptionsToFile()
        {
            StringBuilder dataToWrite = new StringBuilder();

            // Write folder options
            dataToWrite.AppendLine($"{(char) SECTION_HEADER_CHAR}0");
            foreach (KeyValuePair<string, bool> option in FolderOptions)
                dataToWrite.AppendLine($"{option.Key},{option.Value}");

            // Write additional options
            dataToWrite.AppendLine($"{(char) SECTION_HEADER_CHAR}1");
            foreach (KeyValuePair<string, bool> option in MigrationOptions)
                dataToWrite.AppendLine($"{option.Key},{option.Value}");

            // Write download options
            dataToWrite.AppendLine($"{(char) SECTION_HEADER_CHAR}2");
            foreach (KeyValuePair<string, string> option in DownloadOptions)
                dataToWrite.AppendLine($"{option.Key},{option.Value}");

            // Write mlc01 custom folder path
            if (MlcFolderExternalPath != "")
                dataToWrite.AppendLine($"{(char) SECTION_HEADER_CHAR}3\r\n" + MlcFolderExternalPath);   // \r\n -> CR-LF

            // Write file options
            dataToWrite.AppendLine($"{(char) SECTION_HEADER_CHAR}4");
            foreach (KeyValuePair<string, bool> option in FileOptions)
                dataToWrite.AppendLine($"{option.Key},{option.Value}");

            // Create destination directory if it doesn't exist
            string optionsFileDir = Path.GetDirectoryName(OptionsFilePath);
            if (!FileUtils.DirectoryExists(optionsFileDir))
                Directory.CreateDirectory(optionsFileDir);

            // Write string on file overwriting any existing content
            File.WriteAllText(OptionsFilePath, dataToWrite.ToString());
        }

        /*
         *  Deletes the options file, returning true if the file existed and has been removed, otherwise false
         */
        public bool DeleteOptionsFile()
        {
            if (!string.IsNullOrEmpty(OptionsFilePath) && FileUtils.FileExists(OptionsFilePath))
            {
                File.Delete(OptionsFilePath);
                if (OptionsFilePath == AppDataFilePath)    // clean redundant empty folders
                {
                    Directory.Delete(Path.GetDirectoryName(AppDataFilePath), true);
                    string fs00AppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Fs00");
                    if (FileUtils.DirectoryIsEmpty(fs00AppDataFolder))
                        Directory.Delete(fs00AppDataFolder);
                }
                return true;
            }
            else
                return false;
        }

        /*
         *  Returns a list containing the relative paths of the folders which have to be copied
         */
        public List<string> GetFoldersToCopy(bool cemuVersionIsAtLeast110)
        {
            List<string> foldersToCopy = new List<string>();

            // Ignore mlc01 subfolders if source Cemu version is at least 1.10 and custom mlc folder option is selected
            if (cemuVersionIsAtLeast110 && MigrationOptions["dontCopyMlcFolderFor1.10+"] == true)
            {
                foreach (string folder in SelectedEntries(FolderOptions))
                {
                    if (!folder.StartsWith(@"mlc01\"))
                        foldersToCopy.Add(folder);
                }
            }
            else    // otherwise append folders without any extra check 
            {
                foreach (string folder in SelectedEntries(FolderOptions))
                    foldersToCopy.Add(folder);
            }
            
            return foldersToCopy;
        }

        /*
         *  Returns a list containing the relative paths of the files which have to be copied
         */
        public List<string> GetFilesToCopy()
        {
            List<string> filesToCopy = new List<string>();

            foreach (string file in SelectedEntries(FileOptions))
                filesToCopy.Add(file);

            return filesToCopy;
        }

        /*
         *  Iterator method that returns only the selected keys in the given options dictionary
         */
        private IEnumerable<string> SelectedEntries(Dictionary<string, bool> optsDictionary)
        {
            foreach(KeyValuePair<string, bool> option in optsDictionary)
            {
                if (option.Value == true)
                    yield return option.Key;
            }
        }

        /*
         *  Iterator method that returns only custom user folders (the ones that aren't in default dictionary)
         */
        public IEnumerable<string> CustomFolders()
        {
            foreach (KeyValuePair<string, bool> option in FolderOptions)
            {
                if (!defaultFolderOptions.ContainsKey(option.Key))
                    yield return option.Key;
            }
        }

        /*
         *  Iterator method that returns only custom user files (the ones that aren't in default dictionary)
         */
        public IEnumerable<string> CustomFiles()
        {
            foreach (KeyValuePair<string, bool> option in FileOptions)
            {
                if (!defaultFileOptions.ContainsKey(option.Key))
                    yield return option.Key;
            }
        }
    }

    /*
     *  Custom exception for parsing errors that contains line number at which the exception has been thrown 
     */
    public class OptionsParsingException : Exception
    {
        public int CurrentLine { get; }

        public OptionsParsingException() : base() { }
        public OptionsParsingException(string message) : base(message) { }
        public OptionsParsingException(string message, int lineCount) : base(message)
        {
            CurrentLine = lineCount;
        }
        public OptionsParsingException(string message, Exception inner) : base(message, inner) { }
        public OptionsParsingException(string message, Exception inner, int lineCount) : base(message, inner)
        {
            CurrentLine = lineCount;
        }
        protected OptionsParsingException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }

    /*
     *  Custom StreamReader derived class that holds the current number of read lines
     *  CurrentLine works only with ReadLine(), since it's the only method I use to read the stream.
     */
    class MyStreamReader : StreamReader
    {
        public MyStreamReader(string path) : base(path) {}

        public int CurrentLine { private set; get; }

        public override string ReadLine()
        {
            string result = base.ReadLine();
            if (result != null)
                CurrentLine++;
            return result;
        }
    }
}
