using System;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    public partial class HomeForm : Form
    {
        OptionsManager opts;

        public HomeForm()
        {
            InitializeComponent();
            opts = new OptionsManager();
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
    }
}
