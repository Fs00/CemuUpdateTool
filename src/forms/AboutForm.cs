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

            linklblForum.Links.Add(0, 17, "http://forum.cemu.info/forumdisplay.php/15-Guides-amp-modifications");    // add link to link label
            // Position link label according to the other label position to avoid excessive distance caused by scaling
            var newLinkLblBounds = linklblForum.Bounds;
            newLinkLblBounds.X = lblUpdates.Bounds.Right - 3;
            linklblForum.Bounds = newLinkLblBounds;
        }

        private void LinkLabelClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start((string)e.Link.LinkData);
        }
    }
}
