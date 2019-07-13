using System.Text;

namespace CemuUpdateTool.Settings
{
    static partial class Options
    {
        /*
         * Options are written in the file under several sections, one for each options group.
         * Every section is introduced by the section marker, followed by the section's numeric ID.
         * Every group entry is serialized in the format "key,value".
         * Section 3 is different from the others since it contains only one line for the custom MLC path.
         */
        private class OptionsSerializer
        {
            private StringBuilder serializedOptions;

            public string Serialize()
            {
                serializedOptions = new StringBuilder();

                SerializeOptionsGroup(Options.FoldersToMigrate, OptionsFileSection.FoldersToMigrate);
                SerializeOptionsGroup(Options.FilesToMigrate, OptionsFileSection.FilesToMigrate);
                SerializeOptionsGroup(Options.Migration, OptionsFileSection.MigrationOptions);
                SerializeOptionsGroup(Options.Download, OptionsFileSection.DownloadOptions);

                WriteSectionHeader(OptionsFileSection.CustomMlcFolderPath);
                serializedOptions.Append(Options.CustomMlcFolderPath);

                return serializedOptions.ToString();
            }

            private void SerializeOptionsGroup<TValue>(IOptionsGroup<TValue> optionsGroup, OptionsFileSection section)
            {
                WriteSectionHeader(section);
                foreach (var option in optionsGroup)
                    serializedOptions.AppendLine($"{option.Key},{option.Value}");
            }

            private void WriteSectionHeader(OptionsFileSection section)
            {
                serializedOptions.AppendLine(SECTION_MARKER.ToString() + (int)section);
            }
        }
    }
}
