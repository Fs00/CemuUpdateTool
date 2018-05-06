using System;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    public partial class ContainerForm : Form
    {
        public static ContainerForm activeInstance;  // this class will only have one instance at a time
        private Form homeForm;                       // the default form for this "container", it must be never disposed
        private Form currentDisplayingForm;          // the form this "container" is currently displaying

        public ContainerForm()
        {
            if (activeInstance != null)
                throw new ApplicationException("There can't be more than one ContainerForm.");
            
            InitializeComponent();
            activeInstance = this;
        }

        public ContainerForm(Form homeForm) : this()
        {
            this.homeForm = homeForm;
            ShowForm(homeForm);
        }

        public static void ShowForm(Form form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            // Render the requested form
            form.TopLevel = false;
            form.AutoScroll = true;
            form.FormBorderStyle = FormBorderStyle.None;
            activeInstance.formContainer.Controls.Add(form);
            form.Show();

            // We must do these operations here otherwise the container resizing would be messed up
            if (activeInstance.currentDisplayingForm != null)
            {
                // Remove the previous form from the container...
                activeInstance.formContainer.Controls.RemoveAt(0);
                // ...and dispose it only if it isn't the home form
                if (activeInstance.currentDisplayingForm != activeInstance.homeForm)
                    activeInstance.currentDisplayingForm.Dispose();
            }
            activeInstance.currentDisplayingForm = form;
        }

        public static void ShowHomeForm()
        {
            if (activeInstance.homeForm == null)
                throw new InvalidOperationException("This container has no homeForm set.");

            ShowForm(activeInstance.homeForm);
        }
    }
}
