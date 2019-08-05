using System;
using System.Drawing;
using System.Windows.Forms;

namespace CemuUpdateTool.Forms
{
    /*
     *  The first form to be shown in ContainerForm. Allows the user to browse between application sections
     */
    public partial class HomeForm : Form
    {
        public HomeForm()
        {
            InitializeComponent();
            SetButtonsImagesAccordingToScreenDPI();
        }

        // This is a workaround because neither auto-sizing nor ScaleControl override work
        private void SetButtonsImagesAccordingToScreenDPI()
        {
            float scaleFactor = Program.GetScreenDPIScaleFactor();
            var resourceMgr = new System.ComponentModel.ComponentResourceManager(GetType());
            int imageSize = (int) (125 * scaleFactor);

            var image = (Bitmap)resourceMgr.GetObject("btnMigrate.Image");
            btnMigrate.Image = new Bitmap(image, new Size(imageSize, imageSize));
            image = (Bitmap)resourceMgr.GetObject("btnDlMigrate.Image");
            btnDlMigrate.Image = new Bitmap(image, new Size(imageSize, imageSize));
            image = (Bitmap)resourceMgr.GetObject("btnUpdate.Image");
            btnUpdate.Image = new Bitmap(image, new Size(imageSize, imageSize));
        }

        private void ShowMigrateForm(object sender, EventArgs e)
        {
            ContainerForm.ShowForm(new MigrationForm(false));
        }

        private void ShowDownloadMigrateForm(object sender, EventArgs e)
        {
            ContainerForm.ShowForm(new MigrationForm(true));
        }

        private void ShowUpdateForm(object sender, EventArgs e)
        {
            ContainerForm.ShowForm(new UpdateForm());
        }

        private void ShowOptionsForm(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var form = new OptionsForm())
                form.ShowDialog();
        }

        private void ShowAboutForm(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var form = new AboutForm())
                form.ShowDialog();
        }

        private void ShowHelpForm(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new HelpForm(this).Show();
        }
    }
}
