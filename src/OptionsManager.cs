﻿using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    public class OptionsManager
    {
        public Dictionary<string, bool> FolderOptions { private set; get; }      // contains a list of Cemu subfolders and whether they have to be copied
        public Dictionary<string, bool> FileOptions { private set; get; }        // contains a list of files included in Cemu folder and whether they have to be copied
        public Dictionary<string, bool> MigrationOptions { private set; get; }   // contains a set of additional options for the migration
        public Dictionary<string, string> DownloadOptions { private set; get; }  // contains a set of options for the download of Cemu versions
        public string MlcFolderExternalPath { get; set; } = "";                  // mlc01 folder's external path for Cemu 1.10+
        public string OptionsFilePath { set; get; }                              // the path of the settings file (is empty string when no file is used)

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
         *  Executed at application startup.
         *  Looks for options file and if it's found, options are read from it.
         *  Default options are applied if file is not found or an error occurs during parsing.
         */
        public OptionsManager()
        {
            if (OptionsFileExists())
            {
                try
                {
                    ReadOptionsFromFile();
                }
                catch
                {
                    SetDefaultOptions();
                }
            }
            else
                SetDefaultOptions();
        }

        /*
         *  Overloaded constructor used to load options from a custom position.
         *  The given path must be correct and pointing to an existing file.
         */
        public OptionsManager(string optionsFilePath)
        {
            OptionsFilePath = optionsFilePath;
            try
            {
                ReadOptionsFromFile();
            }
            catch
            {
                SetDefaultOptions();
            }
        }

        /*
         *  Looks for options file in executable and %AppData% folder and updates the property accordingly.
         *  Priority is given to the file in the local folder.
         */
        public bool OptionsFileExists()
        {
            bool localFileExists;
            if ((localFileExists = FileUtils.FileExists(LocalFilePath)) || FileUtils.FileExists(AppDataFilePath))
            {
                // Set the file path property according to the current file position
                if (localFileExists)
                    OptionsFilePath = LocalFilePath;
                else
                    OptionsFilePath = AppDataFilePath;

                return true;
            }
            else
            {
                OptionsFilePath = "";
                return false;
            }
        }

        /*
         *  Method that creates and populates options dictionaries reading settings file.
         *  Options file path must be set correctly before calling this method.
         *  After parsing, dictionaries are checked to avoid missing entries.
         */
        public void ReadOptionsFromFile()
        {
            // Dictionaries must be initialized here
            FolderOptions = new Dictionary<string, bool>();
            FileOptions = new Dictionary<string, bool>();
            MigrationOptions = new Dictionary<string, bool>();
            DownloadOptions = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(OptionsFilePath))      // should never happen
                throw new InvalidOperationException("Options file path is not specified.");
            
            MyStreamReader optionsFile = new MyStreamReader(OptionsFilePath);
            try
            {
                // Check if the file is empty
                if (optionsFile.BaseStream.Length == 0)
                    throw new InvalidDataException("Options file is empty.");

                // Start reading
                string sectionHeaderLine = null;
                while (!optionsFile.EndOfStream)
                {
                    if (optionsFile.Peek() == SECTION_HEADER_CHAR)   // if the next char is '#'
                    {
                        sectionHeaderLine = optionsFile.ReadLine();
                        // Check if the sectionId is a number
                        if (!byte.TryParse(sectionHeaderLine.TrimStart('#'), out byte sectionId))
                            throw new FormatException("Section ID is not a number");
                        else
                            ReadFileSection(optionsFile, sectionId);
                    }
                    else
                        optionsFile.ReadLine();
                }

                if (sectionHeaderLine == null)      // if no lines starting with '#' have been encountered
                    throw new InvalidDataException("Options file didn't contain any useful information.");

                // Check for missing options in file
                CheckForMissingEntries();
            }
            catch (Exception exc)
            {
                string message;
                if (exc is InvalidDataException)
                    message = exc.Message;
                else
                    message = $"An unexpected error occurred when parsing options file: {exc.Message.TrimEnd('.')} at line {optionsFile.LineCount}.";

                MessageBox.Show(message + "\r\nDefault settings will be loaded instead.", "Error in settings.dat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                throw;
            }
            finally
            {
                optionsFile.Close();
            }
        }

        /*
         *  Reads an options file section and fills the corresponding dictionary/variable
         *  It reads until it finds the end of file or another section header. The StreamReader must be positioned in the line under the section header.
         *  These are the sections ids:
         *      0: folderOptions
         *      1: migrationOptions
         *      2: downloadOptions
         *      3: mlcFolderExternalPath (only one line is read)
         *      4: fileOptions
         */
        private void ReadFileSection(StreamReader fileStream, byte sectionId)
        {
            // Check if the sectionId is valid
            if (sectionId < 0 || sectionId > 4)
                throw new ArgumentOutOfRangeException("Section ID is not valid");

            // Set up the dynamic dictionary "pointer" according to the sectionId
            dynamic dictionary = null;
            switch (sectionId)
            {
                case 0:
                    dictionary = FolderOptions;
                    break;
                case 1:
                    dictionary = MigrationOptions;
                    break;
                case 2:
                    dictionary = DownloadOptions;
                    break;
                case 4:
                    dictionary = FileOptions;
                    break;
            }

            int startingChar;       // first char code of the current reading line
            string[] parsedLine;    // the "splitted" line

            // Continue reading until you find a '#' or the EOF
            while ((startingChar = fileStream.Peek()) != SECTION_HEADER_CHAR && startingChar != EOF)
            {
                // If there's an empty line, just skip to the next one
                if (startingChar == CR || startingChar == LF)
                    fileStream.ReadLine();
                else
                {
                    if (sectionId == 3)
                    {
                        string tmpPath = fileStream.ReadLine();
                        if (tmpPath.IndexOfAny(Path.GetInvalidPathChars()) == -1)
                        {
                            MlcFolderExternalPath = tmpPath;
                            return;     // for this section there aren't any other things to read
                        }
                        else
                            throw new FormatException("Mlc01 external folder path is malformed");
                    }
                    else     // if I'm handling a dictionary
                    {
                        parsedLine = fileStream.ReadLine().Split(',');
                        if (parsedLine.Length != 2)
                            throw new FormatException("Not a \"key, value\" option");

                        switch (sectionId)
                        {
                            // <string, bool> dictionary (folderOptions, migrationOptions, fileOptions)
                            case 0: case 1: case 4:
                                dictionary.Add(parsedLine[0], Convert.ToBoolean(parsedLine[1]));
                                break;
                            // <string, string> dictionary (downloadOptions)
                            case 2:
                                dictionary.Add(parsedLine[0], parsedLine[1]);
                                break;
                        }
                    }
                }
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
         *  Called if settings.dat is not found or ReadOptionsFromFile() throws an error
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
         *  Options are saved one per line, in the following format: key,value
         *  Folder options terminate with "##", additional options with "###"
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
