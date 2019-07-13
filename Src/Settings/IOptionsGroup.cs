using System.Collections.Generic;

namespace CemuUpdateTool.Settings
{
    /*
     * Represents a fixed set of editable options, each one identified by a string key
     * and containing a value of the specified type.
     */
    interface IOptionsGroup<TValue> : IEnumerable<KeyValuePair<string, TValue>>
    {
        TValue this[string optionKey] { get; set; }
    }
}
