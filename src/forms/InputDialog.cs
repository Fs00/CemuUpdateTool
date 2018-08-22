using System;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    public partial class InputDialog<T> : Form
    {
        public T InputValue { private set; get; }

        public delegate bool ValidationDelegate(string input, out T value, out T reason);
        ValidationDelegate InputIsValidated;

        public InputDialog(ValidationDelegate validationDelegate)
        {
            InitializeComponent();
            InputIsValidated = validationDelegate;
        }

        private void CancelInput(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ValidateInputAndClose(object sender = null, EventArgs e = null)
        {
            if (InputIsValidated(txtBoxInput.Text.Trim(' '), out T value, out T reason))
            {
                InputValue = value;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
                MessageBox.Show($"The value you inserted is not valid: {reason}.", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ValidateOnEnterKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                ValidateInputAndClose();
        }
    }
}
