using System.Collections.Generic;

namespace CemuUpdateTool.Settings
{
    interface IOptionsGroup<TValue> : IEnumerable<KeyValuePair<string, TValue>>
    {
        TValue this[string optionKey] { get; set; }
    }
}
