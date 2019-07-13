using System.Collections.Generic;

namespace CemuUpdateTool.Settings
{
    /*
     * Represents an editable list of options that can be enabled or disabled (on/off).
     * Remove shouldn't throw any exception if an option doesn't exist.
     */
    interface IToggleableOptionsList : IOptionsGroup<bool>
    {
        void Add(string optionKey);
        void Remove(string optionKey);
        bool IsEnabled(string optionKey);

        IEnumerable<string> GetAllEnabled();
        IEnumerable<string> GetAll();
    }
}
