using System.Collections.Generic;

namespace CemuUpdateTool.Settings
{
    interface IToggleableOptionsList : IOptionsGroup<bool>
    {
        void Add(string optionKey);
        void Remove(string optionKey);
        bool IsEnabled(string optionKey);

        IEnumerable<string> GetAllEnabled();
        IEnumerable<string> GetAll();
    }
}
