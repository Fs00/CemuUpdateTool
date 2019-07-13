using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CemuUpdateTool.Settings
{
    class OptionsGroupDictionaryAdapter<TValue> : IOptionsGroup<TValue>
    {
        protected readonly IDictionary<string, TValue> dictionary;

        public OptionsGroupDictionaryAdapter(IDictionary<string, TValue> dictionary)
        {
            this.dictionary = dictionary;
        }

        public TValue this[string optionKey]
        {
            get => dictionary[optionKey];
            set => dictionary[optionKey] = value;
        }

        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }
    }

    class ToggleableOptionsListDictionaryAdapter : OptionsGroupDictionaryAdapter<bool>, IToggleableOptionsList
    {
        public ToggleableOptionsListDictionaryAdapter(IDictionary<string, bool> dictionary) : base(dictionary) {}

        public void Add(string optionKey)
        {
            dictionary.Add(optionKey, true);
        }

        public void Remove(string optionKey)
        {
            dictionary.Remove(optionKey);
        }

        public bool IsEnabled(string optionKey)
        {
            return dictionary[optionKey] == true;
        }

        public IEnumerable<string> GetAll()
        {
            return dictionary.Keys;
        }

        public IEnumerable<string> GetAllEnabled()
        {
            return dictionary.Keys.Where(key => IsEnabled(key));
        }
    }
}
