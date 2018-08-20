using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    public partial class ListEditingForm<T> : Form
    {
        IReadOnlyList<T> originalList;
        IEnumerable<T> forbiddenValues;

        List<T> addedToOriginal, removedFromOriginal;

        public ListEditingForm(List<T> originalList, List<T> addedToOriginal, List<T> removedFromOriginal, IEnumerable<T> forbiddenValues)
        {
            InitializeComponent();

            this.originalList = originalList.AsReadOnly();
            this.addedToOriginal = addedToOriginal;
            this.removedFromOriginal = removedFromOriginal;
            this.forbiddenValues = forbiddenValues;

            // Populate ListView with original list elements
            foreach (T element in originalList)
                lstView.Items.Add(element.ToString());
        }

        private void AddElement(object sender, EventArgs e)
        {
            // TODO
        }

        private void RemoveElement(object sender, EventArgs e)
        {
            if (lstView.SelectedItems.Count > 0)
            {
                var selectedElement = lstView.SelectedItems[0];
                lstView.Items.Remove(selectedElement);

                T removedElement = (T) TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(selectedElement.Text);
                // If the element is present in original list, then it must be added to removed list
                if (originalList.Contains(removedElement))
                    removedFromOriginal.Add(removedElement);
                // Otherwise, if the element wasn't in original list and the user added it, we must remove it from the added list
                else if (addedToOriginal.Contains(removedElement))
                    addedToOriginal.Remove(removedElement);
            }
        }
    }
}
