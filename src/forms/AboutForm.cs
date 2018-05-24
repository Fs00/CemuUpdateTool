using System.Windows.Forms;
using System.Drawing;
using System;
using System.Reflection;

namespace CemuUpdateTool
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            Icon = SystemIcons.Information;     // set form icon

            // Get program version from assembly attribute
            var fileVersionAttribute = Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyInformationalVersionAttribute));
            lblVersion.Text += ((AssemblyInformationalVersionAttribute) fileVersionAttribute).InformationalVersion;

            PageLinkLbl.Links.Add(0, 17, "http://forum.cemu.info/forumdisplay.php/15-Guides-amp-modifications");    // add link to link label
        }

        private void LinkLabelClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start((string)e.Link.LinkData);
        }
    }
}
