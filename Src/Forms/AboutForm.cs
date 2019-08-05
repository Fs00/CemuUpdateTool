using System.Windows.Forms;
using System.Drawing;
using System.Reflection;

namespace CemuUpdateTool.Forms
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            Icon = SystemIcons.Information;
            lblVersion.Text += Application.ProductVersion + $" ({GetReleaseDate()})";
            AddLinksToLabels();
        }

        private string GetReleaseDate()
        {
            object[] attributes = GetType().Assembly.GetCustomAttributes(typeof(AssemblyReleaseDateAttribute), false);
            if (attributes.Length > 0)
                return ((AssemblyReleaseDateAttribute) attributes[0]).ToString();

            return "";
        }

        private void AddLinksToLabels()
        {
            linkLblMadeBy.Links.Add(25, 4, "http://github.com/Fs00");
            linklblForum.Links.Add(36, 26, "http://forum.cemu.info/showthread.php/684");
            linkLblDonate.Links.Add(27, 15, "https://paypal.me/Fs00");
        }

        private void LinkLabelClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start((string)e.Link.LinkData);
        }
    }
}
