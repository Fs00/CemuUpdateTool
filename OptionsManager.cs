using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    public class OptionsManager
    {
        public Dictionary<string, bool> options { set; get; }
        public bool deleteDestFolderContents { set; get; } = false;      // To be implemented
        public string optionsFilePath { set; get; }

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
                options = new Dictionary<string, bool>();

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
                        options.Add(parsedLine[0], Convert.ToBoolean(parsedLine[1]));
                    }
                    // If it's not arrived to the EOF, read one last line
                    if (!optionsFile.EndOfStream)
                        deleteDestFolderContents = Convert.ToBoolean(optionsFile.ReadLine());
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
            else       // if there's no option file
                return false;
        }

        public void WriteOptionsToFile()
        {
            string dataToWrite = "";
            foreach (KeyValuePair<string, bool> option in options)
                dataToWrite += option.Key + "," + option.Value + "\r\n";        // \r\n -> CR-LF
            dataToWrite += "##";
            //dataToWrite += "\n" + deleteDestFolderContents;       // not necessary at the moment
            try
            {
                // Write string on file overwriting any existing content
                File.WriteAllText(optionsFilePath, dataToWrite);
            }
            catch(Exception exc)
            {
                MessageBox.Show("An unexpected error occurred when saving options file: " + exc.Message +
                        "\r\nOptions won't be preserved after you close the program.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            options = new Dictionary<string, bool>();       // necessary to avoid dirty data if ReadOptionsFromFile() fails
            optionsFilePath = LOCAL_FILEPATH;
            options.Add(@"controllerProfiles", true);
            options.Add(@"gameProfiles", false);
            options.Add(@"graphicPacks", true);
            options.Add(@"mlc01\emulatorSave", true);
            options.Add(@"mlc01\usr\title", true);
            options.Add(@"shaderCache\transferable", true);
        }

        public List<string> GetFoldersToCopy()
        {
            // Returns a list of the folders which need to be copied
            List<string> foldersToCopy = new List<string>();
            foreach(KeyValuePair<string, bool> option in options)
            {
                if (option.Value == true)
                    foldersToCopy.Add(option.Key);
            }
            return foldersToCopy;
        }
    }
}
