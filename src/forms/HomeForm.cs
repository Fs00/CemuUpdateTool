﻿using System;
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

            // Set button icons with the correct size
            var resourceMgr = new System.ComponentModel.ComponentResourceManager(GetType());
            Bitmap image = (Bitmap) resourceMgr.GetObject("btnMigrate.Image");
            btnMigrate.Image = new Bitmap(image, new Size(125, 125));

            image = (Bitmap) resourceMgr.GetObject("btnDlMigrate.Image");
            btnDlMigrate.Image = new Bitmap(image, new Size(125, 125));
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
