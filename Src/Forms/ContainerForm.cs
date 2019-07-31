using System;
using System.Threading;
using System.Windows.Forms;

namespace CemuUpdateTool.Forms
{
    /*
     *  This is the main window of the program. The other forms are rendered inside a panel called formContainer
     *  This class is designed to be a singleton and its operations must be globally available (that's why its public methods are static)
     */
    public partial class ContainerForm : Form
    {
        private static ContainerForm activeInstance;
        private readonly Form homeForm;       // the default form for this container, it must be never disposed
        private Form currentInnerForm;        // the form this container is currently rendering inside its body

        private ContainerForm()
        {
            if (activeInstance != null)
                throw new ApplicationException("There can't be more than one ContainerForm.");
            
            InitializeComponent();
            activeInstance = this;
            SetApplicationIconAsContainerIcon();
        }
        
        public ContainerForm(Form homeForm) : this()
        {
            this.homeForm = homeForm ?? throw new ArgumentNullException(nameof(homeForm));
            ShowForm(homeForm);
        }

        private void SetApplicationIconAsContainerIcon()
        {
            IntPtr iconPtr = Properties.Resources.Icon.GetHicon();
            Icon = System.Drawing.Icon.FromHandle(iconPtr);
        }

        public static void ShowForm(Form form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            // The new form must be rendered before removing the previous one, otherwise container size would be messed up
            RenderNewInnerForm(form);
            if (activeInstance.currentInnerForm != null)
                RemovePreviousInnerForm();
            
            activeInstance.currentInnerForm = form;
        }
        
        private static void RenderNewInnerForm(Form form)
        {
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            activeInstance.formContainer.Controls.Add(form);
            form.Parent = activeInstance.formContainer;
            form.Show();
        }

        private static void RemovePreviousInnerForm()
        {
            activeInstance.formContainer.Controls.RemoveAt(0);
            if (activeInstance.currentInnerForm != activeInstance.homeForm)
                activeInstance.currentInnerForm.Dispose();
        }

        public static void ShowHomeForm()
        {
            ShowForm(activeInstance.homeForm);
        }

        public static bool IsFormCurrentlyDisplayed(Form form)
        {
            return form == activeInstance.currentInnerForm;
        }
        
        private void PropagateContainerClosing(object sender, FormClosingEventArgs containerEvent)
        {
            if (currentInnerForm == null)
                return;
            
            /*
             * This closing event handler for the current inner form serves 3 purposes:
             *  - propagating the container closing to the inner form, so that the latter can eventually cancel it
             *  - cancelling container closing if the inner form cancels the event
             *  - preventing the user from seeing that the container resizes to the minimum size just before its closing
             *    (not good looking) by hiding it
             */
            void InnerFormClosingEventHandler(object o, FormClosingEventArgs innerFormEvent)
            {
                if (innerFormEvent.Cancel)
                    containerEvent.Cancel = true;
                else
                {
                    this.Hide();
                    // Give the container the time to hide before closing
                    Thread.Sleep(500);
                }
            }

            currentInnerForm.FormClosing += InnerFormClosingEventHandler;
            currentInnerForm.Close();
            
            // Remove the event handler to avoid being re-added multiple times if the inner form cancels the closing event
            if (!currentInnerForm.IsDisposed)
                currentInnerForm.FormClosing -= InnerFormClosingEventHandler;
        }
    }
}
