using System;
using System.ComponentModel;
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

            ComponentResourceManager textResources = new ComponentResourceManager(typeof(HelpForm));
            string loadedRtfString = textResources.GetString("baseRtfText");
            // TODO: da fare in base alla singola form
            loadedRtfString += @"\viewkind4\uc1\pard\f0\fs19" + 
                @"{\b HOW DOES IT WORK?}\par" +
                @"\fs8\par\fs19" +
                @" The functioning of this program is very simple: just select your old Cemu folder, choose the destination folder which contains the newer Cemu version and click Start! The program will do all the work for you, copying only the folders selected in Options and overwriting automatically any existing file with the same name. You can cancel the operation at any time, and in that case the program will ask you if you want to revert back to the original situation, deleting all the file that had been copied before you clicked on the Cancel button.\par" +
                @" Since version 1.1, once migration process has completed the program will ask you if you want to create a shortcut to the newer Cemu version on desktop. You can prevent this behaviour by adding {\i askForDesktopShortcut,false} to settings.dat file (see below) under the line containing '##'. If the entry is already present, you just have to replace {\i true} with {\i false}.\par" +
                @"\par" +
                @"{\b OPTIONS SECTION}\par" +
                @"\fs8\par\fs19" +
                @" In the options form you can choose which folders must be copied to the new Cemu installation. But not only that: you can also make use of other handy functions, such as copying Cemu settings file, deleting destination folders content before copying (useful if you want to keep only the files you really need), and ignoring mlc01 folder when source Cemu version is higher than 1.10.\par" +
                @" For your convenience, the program comes with the opportunity to store program options in a text file, which can be located in the same folder as the executable, or in %AppData% folder. If both files exist, the program will give priority to the file in the executable folder.\par" +
                @" The first section of that file (called settings.dat) includes the paths of Cemu subfolders and whether they have to be copied or not. You can also add your custom folder options {\ul before the line containing '##'} in order to copy the folders you want in the following format:\par" +
                @"\fs11\par\fs19" +
                @"{\i subfolderLocalPath,True}\par" +
                @"\fs11\par\fs19" +
                @" whereas {\i subFolderLocalPath} is the subdirectory path relative to the Cemu installation folder (e.g. /mlc01/usr/title).\par" +
                @" The second section of the file (between '##' and '###') contains extra options added in version 1.1: {\i copyCemuSettingsFile, deleteDestFolderContents, dontCopyMlcFolderFor1.10+} and {\i askForDesktopShortcut.} The only reason you may want to edit this section has been explained above, in all the other circumstances you can just use the Options dialog. If a setting isn't found when reading the file, it is automatically set to false (except for {\i askForDesktopShortcut)}.\par" +
                @" In the line under '###', the program stores the custom mlc01 folder path which is used to create Cemu desktop shortcut for versions higher than 1.10. Please take note that if you decide to use standard mlc01 path in Options, your custom path won't be used.\par" +
                @"\par" +
                @"{\b FOUND A BUG?}\par" +
                @"\fs8\par\fs19" +
                @" If the application printed out a fatal error and then exited, send me the cvmt-crashlog file you find in executable folder. Otherwise you can report any unexpected behavior (there shouldn't be any, hopefully) in the official thread in Cemu forum, or sending me a PM on Discord \fs18 (my username: Fs00#9393).}";

            richTxtBoxHelp.Rtf = loadedRtfString;
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
