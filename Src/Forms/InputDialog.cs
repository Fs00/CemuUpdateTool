using System;
using System.Windows.Forms;

namespace CemuUpdateTool.Forms
{
    /*
     *  Dialog which lets the user to insert a value and checks for its validity.
     *  The function that validates the input is provided by the caller.
     */
    public partial class InputDialog<T> : Form
    {
        public T InputValue { private set; get; }

        public delegate bool ValidationDelegate(string input, out T value, out string reason);
        private readonly ValidationDelegate InputIsValid;

        public InputDialog(ValidationDelegate validationDelegate)
        {
            InitializeComponent();
            InputIsValid = validationDelegate;
            SetMinAndMaxSizeAccordingToScreenDPI();
        }

        private void SetMinAndMaxSizeAccordingToScreenDPI()
        {
            // Avoids vertical resizing of the window on higher DPIs
            float scaleFactor = Program.GetScreenDPIScaleFactor();
            MinimumSize = new System.Drawing.Size((int)(240 * scaleFactor), Size.Height);
            MaximumSize = new System.Drawing.Size((int)(600 * scaleFactor), Size.Height);
        }

        private void CancelInput(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ValidateInputAndCloseIfCorrect(object sender, EventArgs e)
        {
            if (InputIsValid(txtBoxInput.Text.Trim(' '), out T value, out string reason))
            {
                InputValue = value;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
                MessageBox.Show($"The value you inserted is not valid: {reason}.", "Invalid input",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
