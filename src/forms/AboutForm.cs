using System.Windows.Forms;
using System.Drawing;

namespace CemuUpdateTool
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            Icon = SystemIcons.Information;     // set form icon
            lblVersion.Text += Application.ProductVersion;

            PageLinkLbl.Links.Add(0, 17, "http://forum.cemu.info/forumdisplay.php/15-Guides-amp-modifications");    // add link to link label
        }

        private void LinkLabelClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start((string)e.Link.LinkData);
        }
    }
}
