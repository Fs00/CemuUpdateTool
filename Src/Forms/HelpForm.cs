using System;
using System.Resources;
using System.Windows.Forms;

namespace CemuUpdateTool.Forms
{
    /*
     *  Displays an RTF-formatted help text related to the form that opened it
     */
    public partial class HelpForm : Form
    {
        public HelpForm(Form launcher)
        {
            InitializeComponent();
            Icon = System.Drawing.SystemIcons.Question;
            AddLinksToBottomLabel();
            LoadHelpText(launcher);
            SetTextBoxPadding();
        }

        private void AddLinksToBottomLabel()
        {
            linkLblQuestions.Links.Add(48, 21, "http://forum.cemu.info/showthread.php/684");
            linkLblQuestions.Links.Add(76, 6, "http://github.com/Fs00/CemuUpdateTool/issues");
        }

        private void LoadHelpText(Form launcher)
        {
            var textResources = new ResourceManager("CemuUpdateTool.resources.HelpFormTexts", GetType().Assembly);
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
        }

        private void SetTextBoxPadding()
        {
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
