using System;
using System.Text;

namespace CemuUpdateTool.Settings
{
    static partial class Options
    {
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
