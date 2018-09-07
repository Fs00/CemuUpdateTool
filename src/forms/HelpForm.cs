using System;
using System.Resources;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    /*
     *  HelpForm
     *  Displays an RTF-formatted help text related to the form that opened it
     */
    public partial class HelpForm : Form
    {
        public HelpForm(Form launcher)
        {
            InitializeComponent();

            Icon = System.Drawing.SystemIcons.Question;
            linkLblQuestions.Links.Add(62, 26, "http://forum.cemu.info/showthread.php/684");
            linkLblQuestions.Links.Add(44, 7, "http://discordapp.com");

            // Initialize the resource manager used to load text
            ResourceManager textResources = new ResourceManager("CemuUpdateTool.resources.HelpFormTexts", GetType().Assembly);

            // Load the RTF text according to the form that launched HelpForm
            string loadedRtfString = textResources.GetString("baseRtfText");
            if (launcher is HomeForm)
                loadedRtfString += textResources.GetString("homeFormText");
            else if (launcher is MigrationForm)
                loadedRtfString += textResources.GetString("migrationFormText");
            else if (launcher is OptionsForm)
                loadedRtfString += textResources.GetString("optionsFormText");
            else if (launcher is UpdateForm)
                loadedRtfString += textResources.GetString("updateFormText");
            richTxtBoxHelp.Rtf = loadedRtfString;

            // Set textbox left/right padding
            richTxtBoxHelp.SelectAll();
            richTxtBoxHelp.SelectionIndent += 8;
            richTxtBoxHelp.SelectionRightIndent += 8;
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
