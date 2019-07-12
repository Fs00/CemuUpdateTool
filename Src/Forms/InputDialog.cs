using System;
using System.Windows.Forms;

namespace CemuUpdateTool.Forms
{
    /*
     *  InputDialog
     *  Dialog which lets the user to input a value and checks for its validity. The function that validates the input is provided by the caller
     *  In this program it's used only for strings, but potentially could be used for any data type
     */
    public partial class InputDialog<T> : Form
    {
        public T InputValue { private set; get; }

        public delegate bool ValidationDelegate(string input, out T value, out string reason);
        ValidationDelegate InputIsValidated;

        public InputDialog(ValidationDelegate validationDelegate)
        {
            InitializeComponent();
            InputIsValidated = validationDelegate;

            // Set minimum and maximum size according to scale factor to avoid vertical resizing of the window
            float scaleFactor = Program.GetDPIScaleFactor();
            MinimumSize = new System.Drawing.Size((int)(240 * scaleFactor), Size.Height);
            MaximumSize = new System.Drawing.Size((int)(600 * scaleFactor), Size.Height);
        }

        private void CancelInput(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ValidateInputAndClose(object sender = null, EventArgs e = null)
        {
            if (InputIsValidated(txtBoxInput.Text.Trim(' '), out T value, out string reason))
            {
                InputValue = value;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
                MessageBox.Show($"The value you inserted is not valid: {reason}.", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
