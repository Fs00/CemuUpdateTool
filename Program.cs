using System;
using System.Windows.Forms;
using System.IO;

namespace CemuUpdateTool
{
    static class Program
    {
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(new MainForm());
            }
            catch(Exception exc)
            {
                DateTime thisMoment = DateTime.Now;
                string crashlogContent = "";

                MessageBox.Show("Unhandled " + exc.GetType().ToString().Substring(exc.GetType().ToString().IndexOf(".")+1) + " thrown in method " + exc.TargetSite.Name + ": " + exc.Message + 
                    "\nFor detailed information check the latest cvmt-crashlog in executable folder.", "Fatal error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                crashlogContent += "Cemu Version Migration Tool crashlog - " + thisMoment.ToString(@"yyyy/MM/dd HH:mm:ss");
                crashlogContent += "\r\n------------------------------------------------------------\r\n\r\n";
                crashlogContent += "Method \"" + exc.TargetSite + "\" threw a " + exc.GetType().ToString() + " due to the following reason:\r\n" + exc.Message;
                crashlogContent += "\r\n\r\nAdditional error information\r\n";
                crashlogContent += "------------------------------\r\n";
                crashlogContent += "Inner exception: ";
                if (exc.InnerException == null)
                    crashlogContent += "none\r\n";
                else
                    crashlogContent += exc.InnerException.GetType().ToString() + " in method \"" + exc.TargetSite + "\": " + exc.Message + "\r\n";
                crashlogContent += "HResult: " + exc.HResult + "\r\n";
                crashlogContent += "Stack trace:\r\n" + exc.StackTrace;

                File.WriteAllText(@".\cvmt-crashlog_" + thisMoment.ToString("yyyy-MM-dd_HH.mm.ss") + ".txt", crashlogContent);
                Application.Exit();
            }
        }
    }
}
