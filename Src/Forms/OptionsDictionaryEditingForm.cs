using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CemuUpdateTool.Forms
{
    /*
     *  Allows the user to edit custom files and folders dictionaries (and more in general any <string, bool> dictionary)
     *  by adding/removing entries and checking/unchecking them
     */
    public partial class OptionsDictionaryEditingForm : Form
    {
        private readonly Dictionary<string, bool> editedDictionary;
        private readonly IEnumerable<string> forbiddenValues;

        public OptionsDictionaryEditingForm(string formTitle,
                                            Dictionary<string, bool> editedDictionary,
                                            IEnumerable<string> forbiddenValues)
        {
            InitializeComponent();

            Text = formTitle;
            this.editedDictionary = editedDictionary;
            this.forbiddenValues = forbiddenValues;

            PopulateListView();
        }

        private void PopulateListView()
        {
            foreach (var entry in editedDictionary)
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
            using (var inputDialog = new InputDialog<string>(IsInputValid))
            {
                DialogResult choice = inputDialog.ShowDialog();
                if (choice == DialogResult.OK)
                {
                    ListViewItem addedItem = listView.Items.Add(inputDialog.InputValue);
                    // We don't need to add the item to the dictionary manually since the ItemChecked event handler does it (see below)
                    addedItem.Checked = true;
                }
            }
        }

        private void RemoveElement(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                ListViewItem selectedElement = listView.SelectedItems[0];
                listView.Items.Remove(selectedElement);
                editedDictionary.Remove(selectedElement.Text);
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
         *  Validation function for the InputDialog
         */
        private bool IsInputValid(string input, out string value, out string reason)
        {
            value = null;
            if (string.IsNullOrEmpty(input))
                reason = "the string is empty";
            else if (input.IndexOfAny(System.IO.Path.GetInvalidPathChars()) > -1 || input.EndsWith(@"\") || input.EndsWith("/"))
                reason = "the string is not a valid path or file name. Make sure that the path does not contain forbidden chars " +
                         "and that does not end with a backslash or a slash";
            else if (forbiddenValues.Contains(input))
                reason = "this value is already included in default options";
            else if (editedDictionary.ContainsKey(input))
                reason = "this value has already been added";
            else
            {
                value = input;
                reason = null;
            }

            return value != null;
        }

        /*
         *  Handler for the ItemChecked ListView event
         *  Take note that it is called also when an item is added
         */
        private void UpdateCheckedStateInDictionary(object sender, ItemCheckedEventArgs evt)
        {
            editedDictionary[evt.Item.Text] = evt.Item.Checked;
        }

        private void ResizeListViewColumnOnFormResize(object sender, EventArgs e)
        {
            listView.Columns[0].Width = listView.Width - 7;
        }
    }
}
