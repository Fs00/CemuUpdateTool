using System;
using System.Resources;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    public partial class HelpForm : Form
    {
        public HelpForm(Form launcher)
        {
            InitializeComponent();

            Icon = System.Drawing.SystemIcons.Question;
            linkLblForum.Links.Add(0, 17, "http://forum.cemu.info/forumdisplay.php/15-Guides-amp-modifications");
            linkLblDiscord.Links.Add(0, 7, "http://discordapp.com");

            // Initialize the resource manager used to load text
            ResourceManager textResources = new ResourceManager("CemuUpdateTool.src.forms.HelpFormTexts", typeof(HelpForm).Assembly);

            // Load the RTF text according to the form that launched HelpForm
            string loadedRtfString = textResources.GetString("baseRtfText");
            if (launcher is HomeForm)
                loadedRtfString += textResources.GetString("homeFormText");
            else if (launcher is MigrationForm)
                loadedRtfString += textResources.GetString("migrationFormText");
            else if (launcher is OptionsForm)
                loadedRtfString += textResources.GetString("optionsFormText");  // TODO: da scrivere
            richTxtBoxHelp.Rtf = loadedRtfString;

            // Set textbox left/right padding
            richTxtBoxHelp.SelectAll();
            richTxtBoxHelp.SelectionIndent += 10;
            richTxtBoxHelp.SelectionRightIndent += 10;
            richTxtBoxHelp.DeselectAll();
        }

        private void Exit(object sender, EventArgs e)
        {
            Close();
        }

        private void LinkLabelClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start((string)e.Link.LinkData);
        }
    }
}
