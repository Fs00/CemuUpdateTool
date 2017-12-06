using System.Windows.Forms;
using System.Drawing;

namespace CemuUpdateTool
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            Icon = SystemIcons.Information;
            PageLinkLbl.Links.Add(0, 17, "http://forum.cemu.info/forumdisplay.php/15-Guides-amp-modifications");
        }

        private void LinkLabelClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start((string)e.Link.LinkData);
        }
    }
}
