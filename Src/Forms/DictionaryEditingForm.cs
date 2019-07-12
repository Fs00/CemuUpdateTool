using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CemuUpdateTool.Forms
{
    /*
     *  DictionaryEditingForm
     *  Allows the user to edit custom files and folders dictionaries by adding/removing entries and checking/unchecking them
     */
    public partial class DictionaryEditingForm : Form
    {
        Dictionary<string, bool> dictionary;        // dictionary that is edited
        IEnumerable<string> forbiddenValues;        // contains the list of keys which aren't allowed to be added (in our case default options)

        public DictionaryEditingForm(Dictionary<string, bool> dictionary, IEnumerable<string> forbiddenValues)
        {
            InitializeComponent();

            this.dictionary = dictionary;
            this.forbiddenValues = forbiddenValues;

            // Populate ListView with dictionary elements
            foreach (var entry in dictionary)
            {
                ListViewItem addedItem = listView.Items.Add(entry.Key);
                addedItem.Checked = entry.Value;
            }
        }

        /*
         *  Adds an element to the dictionary
         *  Uses InputDialog to provide input and validation mechanism
         */
        private void AddElement(object sender, EventArgs e)
        {
            var inputDialog = new InputDialog<string>(IsInputOK);
            DialogResult choice = inputDialog.ShowDialog();
            if (choice == DialogResult.OK)
            {
                ListViewItem addedItem = listView.Items.Add(inputDialog.InputValue);
                // Here we don't need to add the item to the dictionary since the ItemChecked event handler is called, which creates the entry (see below)
                addedItem.Checked = true;
            }
        }

        /*
         *  Removes the selected element from the ListView and from the dictionary
         */
        private void RemoveElement(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                ListViewItem selectedElement = listView.SelectedItems[0];
                listView.Items.Remove(selectedElement);
                dictionary.Remove(selectedElement.Text);
            }
        }

        private void CheckAllItems(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.Items)
                item.Checked = true;
        }

        private void UncheckAllItems(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.Items)
                item.Checked = false;
        }
        
        /*
         *  Validation delegate for the InputDialog.
         *  It checks if the string is a valid file name, it's not included in forbiddenValues and it's not already added
         */
        private bool IsInputOK(string input, out string value, out string reason)
        {
            value = null;
            if (string.IsNullOrEmpty(input))
            {
                reason = "the string is empty";
                return false;
            }
            else if (input.IndexOfAny(System.IO.Path.GetInvalidPathChars()) > -1 || input.EndsWith(@"\") || input.EndsWith("/"))
            {
                reason = "the string is not a valid path or file name. Make sure that the path does not contain forbidden chars and that does not end with a backslash or a slash";
                return false;
            }
            else if (forbiddenValues.Contains(input))
            {
                reason = "this value is already included in default options";
                return false;
            }
            else if (dictionary.ContainsKey(input))
            {
                reason = "this value has already been added";
                return false;
            }

            value = input;
            reason = "";
            return true;
        }

        /*
         *  Handler for the ItemChecked ListView event
         *  Warning: It is called also when an item is added
         */
        private void UpdateCheckedStateInDictionary(object sender, ItemCheckedEventArgs e)
        {
            dictionary[e.Item.Text] = e.Item.Checked;
        }
    }
}
