using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace CemuUpdateTool.Settings
{
    enum OptionsFileSection
    {
        FoldersToMigrate = 0,
        MigrationOptions = 1,
        DownloadOptions = 2,
        CustomMlcFolderPath = 3,
        FilesToMigrate = 4
    }

    // See OptionsSerializer for details on the options file format.
    static partial class Options
    {
        private class OptionsParser
        {
            private Dictionary<string, bool> parsedFoldersToMigrate = new Dictionary<string, bool>();
            private Dictionary<string, bool> parsedFilesToMigrate = new Dictionary<string, bool>();
            private Dictionary<string, bool> parsedMigrationOptions = new Dictionary<string, bool>();
            private Dictionary<string, string> parsedDownloadOptions = new Dictionary<string, string>();
            private string parsedCustomMlcFolderPath = "";

            private LineCountingStreamReader optionsFileStream;

            private const int CR = 13, LF = 10;

            public void ApplyParsedOptions()
            {
                Options.FoldersToMigrate = new ToggleableOptionsListDictionaryAdapter(parsedFoldersToMigrate);
                Options.FilesToMigrate = new ToggleableOptionsListDictionaryAdapter(parsedFilesToMigrate);
                Options.Migration = new OptionsGroupDictionaryAdapter<bool>(parsedMigrationOptions);
                Options.Download = new OptionsGroupDictionaryAdapter<string>(parsedDownloadOptions);
                Options.CustomMlcFolderPath = parsedCustomMlcFolderPath;
            }

            public void ReadFromFile(string optionsFilePath)
            {
                using (optionsFileStream = new LineCountingStreamReader(optionsFilePath))
                {
                    if (optionsFileStream.BaseStream.Length == 0)
                        throw new InvalidDataException("Options file is empty.");

                    try
                    {
                        while (!optionsFileStream.EndOfStream)
                        {
                            if (IsNextCharacterSectionMarker())
                            {
                                string sectionId = optionsFileStream.ReadLine().TrimStart(SECTION_MARKER);
                                OptionsFileSection section = ParseFileSectionId(sectionId);
                                ParseFileSection(section);
                            }
                            else
                                optionsFileStream.ReadLine();
                        }
                    }
                    catch (Exception exc)
                    {
                        throw GetWrappedExceptionWithCurrentLineNumber(exc);
                    }

                    if (NoDataHasBeenParsed())
                        throw new InvalidDataException("Options file didn't contain any useful information.");

                    AddMissingOptionsWithDefaultValue();
                }
            }

            private OptionsFileSection ParseFileSectionId(string sectionIdAsString)
            {
                if (!int.TryParse(sectionIdAsString, out int sectionId))
                    throw new OptionsParsingException("Section ID is not a number", optionsFileStream.CurrentLine);

                if (!Enum.IsDefined(typeof(OptionsFileSection), sectionId))
                    throw new OptionsParsingException("Section ID is out of range", optionsFileStream.CurrentLine);

                return (OptionsFileSection)sectionId;
            }

            private void ParseFileSection(OptionsFileSection section)
            {
                switch (section)
                {
                    case OptionsFileSection.CustomMlcFolderPath:
                        ParseCustomMlcFolderPath();
                        break;
                    case OptionsFileSection.FoldersToMigrate:
                        ParseOptionsAndAddToDictionary(parsedFoldersToMigrate);
                        break;
                    case OptionsFileSection.FilesToMigrate:
                        ParseOptionsAndAddToDictionary(parsedFilesToMigrate);
                        break;
                    case OptionsFileSection.MigrationOptions:
                        ParseOptionsAndAddToDictionary(parsedMigrationOptions);
                        break;
                    case OptionsFileSection.DownloadOptions:
                        ParseOptionsAndAddToDictionary(parsedDownloadOptions);
                        break;
                }
            }

            private void ParseCustomMlcFolderPath()
            {
                if (IsNextCharacterSectionMarker() || optionsFileStream.EndOfStream)
                    return;

                string path = optionsFileStream.ReadLine();
                if (path.IndexOfAny(Path.GetInvalidPathChars()) == -1)
                    parsedCustomMlcFolderPath = path;
                else
                    throw new OptionsParsingException("Mlc01 external folder path is malformed", optionsFileStream.CurrentLine);
            }

            private void ParseOptionsAndAddToDictionary<TValue>(Dictionary<string, TValue> destinationDictionary)
            {
                var converterToTValue = TypeDescriptor.GetConverter(typeof(TValue));
                while (!IsNextCharacterSectionMarker() && !optionsFileStream.EndOfStream)
                {
                    if (IsNextCharacterEndOfLine())
                    {
                        optionsFileStream.ReadLine();
                        continue;
                    }

                    string[] parsedLine = optionsFileStream.ReadLine().Split(',');
                    if (parsedLine.Length != 2)
                        throw new OptionsParsingException("Option is not in the \"key, value\" format", optionsFileStream.CurrentLine);

                    destinationDictionary.Add(parsedLine[0], (TValue)converterToTValue.ConvertFromInvariantString(parsedLine[1]));
                }
            }

            private bool IsNextCharacterSectionMarker()
            {
                return optionsFileStream.Peek() == SECTION_MARKER;
            }

            private bool IsNextCharacterEndOfLine()
            {
                int nextCharacter = optionsFileStream.Peek();
                return nextCharacter == CR || nextCharacter == LF;
            }

            private Exception GetWrappedExceptionWithCurrentLineNumber(Exception exc)
            {
                if (exc is OptionsParsingException)
                    return exc;

                return new OptionsParsingException(exc.Message, optionsFileStream.CurrentLine);
            }

            private bool NoDataHasBeenParsed()
            {
                return parsedFoldersToMigrate.Count == 0 &&
                       parsedFilesToMigrate.Count == 0 &&
                       parsedMigrationOptions.Count == 0 &&
                       parsedDownloadOptions.Count == 0 &&
                       string.IsNullOrEmpty(parsedCustomMlcFolderPath);
            }

            // If default folder/files were missing, they are configured not to be copied
            private void AddMissingOptionsWithDefaultValue()
            {
                foreach (string folder in Options.AllDefaultFoldersToMigrate())
                {
                    if (!parsedFoldersToMigrate.ContainsKey(folder))
                        parsedFoldersToMigrate.Add(folder, false);
                }

                foreach (string file in Options.AllDefaultFilesToMigrate())
                {
                    if (!parsedFilesToMigrate.ContainsKey(file))
                        parsedFilesToMigrate.Add(file, false);
                }

                foreach (var option in Options.migrationDefaults)
                {
                    if (!parsedMigrationOptions.ContainsKey(option.Key))
                        parsedMigrationOptions.Add(option.Key, option.Value);
                }

                foreach (var option in Options.downloadDefaults)
                {
                    if (!parsedDownloadOptions.ContainsKey(option.Key))
                        parsedDownloadOptions.Add(option.Key, option.Value);
                }
            }
        }
    }
}
