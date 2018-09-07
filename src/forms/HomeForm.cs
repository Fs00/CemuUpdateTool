﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    /*
     *  HomeForm
     *  The first form to be shown in ContainerForm. Allows the user to browse through the application functionalities
     */
    public partial class HomeForm : Form
    {
        OptionsManager options;

        public HomeForm(OptionsManager opts)
        {
            InitializeComponent();
            options = opts;

            float scaleFactor = Program.GetDPIScaleFactor();
            var resourceMgr = new System.ComponentModel.ComponentResourceManager(GetType());

            // Set button icons with the correct size according to current system DPI
            // We must do that in the constructor because neither auto-sizing nor ScaleControl override work
            var image = (Bitmap) resourceMgr.GetObject("btnMigrate.Image");
            btnMigrate.Image = new Bitmap(image, new Size((int)(125 * scaleFactor), (int)(125 * scaleFactor)));
            image = (Bitmap) resourceMgr.GetObject("btnDlMigrate.Image");
            btnDlMigrate.Image = new Bitmap(image, new Size((int)(125 * scaleFactor), (int)(125 * scaleFactor)));
            image = (Bitmap)resourceMgr.GetObject("btnUpdate.Image");
            btnUpdate.Image = new Bitmap(image, new Size((int)(125 * scaleFactor), (int)(125 * scaleFactor)));
        }

        private void ShowMigrateForm(object sender, EventArgs e)
        {
            ContainerForm.ShowForm(new MigrationForm(options, false));
        }

        private void ShowDownloadMigrateForm(object sender, EventArgs e)
        {
            ContainerForm.ShowForm(new MigrationForm(options, true));
        }

        private void ShowUpdateForm(object sender, EventArgs e)
        {
            ContainerForm.ShowForm(new UpdateForm(options));
        }

        private void ShowOptionsForm(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new OptionsForm(options).ShowDialog();
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
