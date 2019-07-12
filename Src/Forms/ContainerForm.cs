using System;
using System.Windows.Forms;

namespace CemuUpdateTool.Forms
{
    /*
     *  ContainerForm
     *  This is the main window of the program. The other forms are rendered inside a panel called formContainer
     *  This class is designed to be instantiated only once and its operations must be globally available (that's why its public methods are static)
     */
    public partial class ContainerForm : Form
    {
        private static ContainerForm activeInstance;  // this class will only have one instance at a time
        private Form homeForm;                        // the default form for this "container", it must be never disposed
        private Form currentDisplayingForm;           // the form this "container" is currently displaying

        public ContainerForm()
        {
            if (activeInstance != null)
                throw new ApplicationException("There can't be more than one ContainerForm.");
            
            InitializeComponent();
            activeInstance = this;

            // Retrieve the icon from application resources
            IntPtr iconPtr = Properties.Resources.Icon.GetHicon();
            Icon = System.Drawing.Icon.FromHandle(iconPtr);
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
            form.FormBorderStyle = FormBorderStyle.None;
            activeInstance.formContainer.Controls.Add(form);
            form.Parent = activeInstance.formContainer;
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

        public static bool IsCurrentDisplayingForm(Form form)
        {
            return form == activeInstance.currentDisplayingForm;
        }

        /*
         *  Propagates container closing to the current displaying form, so that we can eventually cancel the event in the latter
         */
        private void PropagateContainerFormClosing(object sender, FormClosingEventArgs containerEvt)
        {
            if (currentDisplayingForm != null)
            {
                /* Add this event handler so that this form will be hidden as soon as we know that currentDisplayingForm closing won't certainly be cancelled.
                   This prevents ContainerForm being resized to the minimum size just before its closing (not good looking).
                   Otherwise, if currentDisplayingForm has cancelled the closing event, we must not close the container. */
                FormClosingEventHandler evtHandler = (o, formEvt) => {
                    if (formEvt.Cancel)
                        containerEvt.Cancel = true;
                    else
                        this.Hide();
                };
                currentDisplayingForm.FormClosing += evtHandler;

                currentDisplayingForm.Close();
                // Remove the event handler to avoid duplicates if currentDisplayingForm cancels the closing event
                if (!currentDisplayingForm.IsDisposed)
                    currentDisplayingForm.FormClosing -= evtHandler;
            }
        }
    }
}
