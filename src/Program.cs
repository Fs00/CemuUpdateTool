using System;
using System.Windows.Forms;
using System.IO;

namespace CemuUpdateTool
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            #if DEBUG
            Application.Run(new MainForm());

            #else
            try
            {
                Application.Run(new MainForm());
            }
            catch(Exception exc)
            {
                /*
                 *   UNHANDLED EXCEPTIONS MANAGEMENT in order to create a crashlog
                 */
                DateTime thisMoment = DateTime.Now;
                string exceptionName = exc.GetType().ToString();
                string crashlogContent = "";

                // Show fatal error dialog
                MessageBox.Show($"Unhandled {exceptionName.Substring(exceptionName.LastIndexOf(".")+1)} thrown in method {exc.TargetSite.Name}: {exc.Message}" + 
                    "\nFor detailed information check the latest cvmt-crashlog in executable folder.", "Fatal error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Build crashlog file content
                crashlogContent += $"Cemu Version Migration Tool crashlog - {thisMoment.ToString(@"yyyy/MM/dd HH:mm:ss")}" +
                                   "\r\n------------------------------------------------------------\r\n\r\n" +
                                   $"Method \"{exc.TargetSite}\" threw a {exceptionName} due to the following reason:\r\n{exc.Message}" +
                                   "\r\n\r\nAdditional error information\r\n" +
                                   "------------------------------\r\n" +
                                   "Inner exception: ";

                if (exc.InnerException == null)
                    crashlogContent += "none\r\n";
                else
                    crashlogContent += $"{exc.InnerException.GetType().ToString()} in method \"{exc.InnerException.TargetSite}\": {exc.InnerException.Message}\r\n";

                crashlogContent += string.Format("HResult: 0x{0:X8}\r\n", exc.HResult) +
                                   $"Stack trace:\r\n{exc.StackTrace}";

                // Write crashlog content on file
                File.WriteAllText($@".\cvmt-crashlog_{thisMoment.ToString("yyyy-MM-dd_HH.mm.ss")}.txt", crashlogContent);
                Application.Exit();
            }
            # endif
        }
    }
}
