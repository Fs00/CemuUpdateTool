using System;
using System.Drawing;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    public partial class HomeForm : Form
    {
        OptionsManager opts;

        public HomeForm()
        {
            InitializeComponent();
            opts = new OptionsManager();    // load program options

            float scaleFactor = Graphics.FromHwnd(IntPtr.Zero).DpiX / 96;
            var resourceMgr = new System.ComponentModel.ComponentResourceManager(GetType());

            // Set button icons with the correct size according to current system DPI
            // We must do that in the constructor because neither auto-sizing nor ScaleControl override work
            Bitmap image = (Bitmap) resourceMgr.GetObject("btnMigrate.Image");
            btnMigrate.Image = new Bitmap(image, new Size((int)(125 * scaleFactor), (int)(125 * scaleFactor)));
            image = (Bitmap) resourceMgr.GetObject("btnDlMigrate.Image");
            btnDlMigrate.Image = new Bitmap(image, new Size((int)(125 * scaleFactor), (int)(125 * scaleFactor)));
        }

        private void ShowMigrateForm(object sender, EventArgs e)
        {
            ContainerForm.ShowForm(new MigrationForm(opts, false));
        }

        private void ShowDownloadMigrateForm(object sender, EventArgs e)
        {
            ContainerForm.ShowForm(new MigrationForm(opts, true));
        }

        private void ShowOptionsForm(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new OptionsForm(opts).ShowDialog();
        }

        private void ShowAboutForm(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new AboutForm().ShowDialog();
        }

        private void ShowHelpForm(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new HelpForm(this).Show();
        }
    }
}
