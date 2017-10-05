using System;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();

            Icon = System.Drawing.SystemIcons.Question;
            linkLblForum.Links.Add(0, 17, "http://forum.cemu.info/forumdisplay.php/15-Guides-amp-modifications");
            linkLblDiscord.Links.Add(0, 7, "http://discordapp.com");

            richTxtBoxHelp.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1040{\fonttbl{\f0\fnil\fcharset0 Microsoft Sans Serif;}}" +
                @"\viewkind4\uc1\pard\f0\fs19" +
                @"{\b HOW DOES IT WORK?}\par" +
                @"\fs8\par\fs19" +
                @" The functioning of this program is very simple: just select your old Cemu folder, choose the destination folder which contains the newer Cemu version and click Start! The program will do all the work for you, copying only the folders selected in Options and overwriting automatically any existing file with the same name. You can cancel the operation at any time, and in that case the program will ask you if you want to revert back to the original situation, deleting all the file that had been copied before you clicked on the Cancel button.\par" +
                @"\par" +
                @"{\b OPTIONS SECTION}\par" +
                @"\fs8\par\fs19" +
                @" In the options form you can choose which folders must be copied to the new Cemu installation. There's one option that can't be checked - don't worry, it'll be implemented in a next update.\par" +
                @" You can also decide whether to save the program options file in the same folder as the executable (default setting), or in %AppData% folder. If both files exist, the program will give priority to the file in the executable folder.\par" +
                @" That file (called settings.dat) includes the paths of Cemu subfolders and whether they have to be copied or not. You can also add your custom folder options {\ul before the line containing '##'} in order to copy the folders you want in the following format:\par" +
                @"\fs11\par\fs19" +
                @"{\i subfolderLocalPath,True}\par" +
                @"\fs11\par\fs19" +
                @" whereas {\i subFolderLocalPath} is the subdirectory path relative to the Cemu installation folder (e.g. /mlc01/usr/title).\par" +
                @"\par" +
                @"{\b FOUND A BUG?}\par" +
                @"\fs8\par\fs19" +
                @" If the application crashed printing out an error message, send me the cvmt-crashlog file you find in executable folder. Otherwise you can report any unexpected behavior (there shouldn't be any, hopefully) in the official thread in Cemu forum, or sending me a PM on Discord \fs18 (my username: Fs00#9393).}";   
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LinkLabelClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start((string)e.Link.LinkData);
        }
    }
}
