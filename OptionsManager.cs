using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    public class OptionsManager
    {
        public Dictionary<string, bool> folderOptions { set; get; }
        public Dictionary<string, bool> additionalOptions { set; get; }
        public string mlcFolderExternalPath { get; set; }
        public string optionsFilePath { set; get; } = "";

        // Useful constants to make code more clean & readable
        public readonly string LOCAL_FILEPATH = @".\settings.dat";
        public readonly string APPDATA_FILEPATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Fs00\CemuUpdateTool\settings.dat");

        public OptionsManager()
        {
            if (ReadOptionsFromFile() == false)
                SetDefaultOptions();
        }

        public bool ReadOptionsFromFile()
        {
            bool localFileExists;
            bool readingOutcome = true;
            string line;
            string[] parsedLine;

            if((localFileExists = FileOperations.FileExists(LOCAL_FILEPATH)) || FileOperations.FileExists(APPDATA_FILEPATH))
            {
                // Create the dictionaries here so we are sure that OptionsForm won't throw NullReferenceException if there aren't any options in the file
                folderOptions = new Dictionary<string, bool>();
                additionalOptions = new Dictionary<string, bool>();

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
                            additionalOptions.Add(parsedLine[0], Convert.ToBoolean(parsedLine[1]));
                        }
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

        public void WriteOptionsToFile()
        {
            string dataToWrite = "";
            // Write folder options
            foreach (KeyValuePair<string, bool> option in folderOptions)
                dataToWrite += option.Key + "," + option.Value + "\r\n";        // \r\n -> CR-LF
            dataToWrite += "##\r\n";

            // Write additional options
            foreach (KeyValuePair<string, bool> option in additionalOptions)
                dataToWrite += option.Key + "," + option.Value + "\r\n";
            dataToWrite += "###";

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

        public bool DeleteOptionsFile()
        {
            // Returns true if options file existed and has been removed, otherwise false
            if (FileOperations.FileExists(optionsFilePath))
            {
                File.Delete(optionsFilePath);
                if (optionsFilePath == APPDATA_FILEPATH)    // clean redundant empty folders
                    Directory.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Fs00"), true);
                return true;
            }
            else
                return false;

        }

        public void SetDefaultOptions()
        {
            folderOptions = new Dictionary<string, bool> {      // necessary to avoid dirty data if ReadOptionsFromFile() fails
                { @"controllerProfiles", true },
                { @"gameProfiles", false },
                { @"graphicPacks", true },
                { @"mlc01\emulatorSave", true },       // savegame directory before 1.11
                { @"mlc01\usr\save", true },           // savegame directory since 1.11
                { @"mlc01\usr\title", true },
                { @"shaderCache\transferable", true }
            };

            additionalOptions = new Dictionary<string, bool> {
                { "copyCemuSettingsFile", true },
                { "deleteDestFolderContents", false },
                { "dontCopyMlcFolderFor1.10+", false },
                { "askForDesktopShortcut", false }          // DA VALUTARE
            };
        }

        public List<string> GetFoldersToCopy()
        {
            // Returns a list of the folders which need to be copied
            List<string> foldersToCopy = new List<string>();
            foreach(KeyValuePair<string, bool> option in folderOptions)
            {
                if (option.Value == true)
                    foldersToCopy.Add(option.Key);
            }
            return foldersToCopy;
        }
    }
}
