using System;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    public partial class ContainerForm : Form
    {
        public static ContainerForm activeInstance;

        public ContainerForm()
        {
            if (activeInstance != null)
                throw new ApplicationException("There can't be more than one ContainerForm.");
            
            InitializeComponent();
            activeInstance = this;
        }

        public ContainerForm(Form startingForm) : this()
        {
            ShowForm(startingForm);
        }

        public static void ShowForm(Form form)
        {
            activeInstance.formContainer.Controls.Clear();
            form.TopLevel = false;
            form.AutoScroll = true;
            form.FormBorderStyle = FormBorderStyle.None;
            activeInstance.formContainer.Controls.Add(form);
            form.Show();
        }
    }
}
