using System.Collections;
using System.Collections.Generic;

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

        public IEnumerable<string> GetAllEnabled()
        {
            foreach (KeyValuePair<string, bool> option in dictionary)
            {
                if (option.Value == true)
                    yield return option.Key;
            }
        }

        public IEnumerable<string> GetAll()
        {
            foreach (KeyValuePair<string, bool> option in dictionary)
                yield return option.Key;
        }

        public bool IsEnabled(string optionKey)
        {
            return dictionary[optionKey] == true;
        }
    }
}
