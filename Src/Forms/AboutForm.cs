﻿using System.Windows.Forms;
using System.Drawing;
using System.Reflection;

namespace CemuUpdateTool.Forms
{
    /*
     *  AboutForm
     *  Displays info about the program
     */
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            Icon = SystemIcons.Information;     // set form icon
            lblVersion.Text += Application.ProductVersion + $" ({GetReleaseDate()})";

            // Add links to labels
            linklblForum.Links.Add(36, 26, "http://forum.cemu.info/showthread.php/684");
            linkLblDonate.Links.Add(27, 15, "https://paypal.me/Fs00");
        }

        private string GetReleaseDate()
        {
            var attributes = GetType().Assembly.GetCustomAttributes(typeof(AssemblyReleaseDateAttribute), false);
            if (attributes != null)
                return (attributes[0] as AssemblyReleaseDateAttribute).ToString();

            return "";
        }

        private void LinkLabelClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start((string)e.Link.LinkData);
        }
    }
}
