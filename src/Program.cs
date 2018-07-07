using System;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace CemuUpdateTool
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en");

            // Load options from file
            OptionsManager opts;
            if (args.Length > 0 && args[0].TrimStart('-', '/') == "force-appdata-config" && FileUtils.FileExists(OptionsManager.AppDataFilePath))
                opts = new OptionsManager(OptionsManager.AppDataFilePath);
            else
                opts = new OptionsManager();

            #if DEBUG
            int res = GetProcessDpiAwareness(IntPtr.Zero, out int value);
            System.Diagnostics.Debug.WriteLine($"DpiAwareness: {(res == 0 ? value.ToString() : "ERR")}");
            Application.Run(new ContainerForm(new HomeForm(opts)));

            #else
            try
            {
                Application.Run(new ContainerForm(new HomeForm(opts)));
            }
            catch (Exception exc)
            {
                /*
                 *   UNHANDLED EXCEPTIONS MANAGEMENT in order to create a crashlog
                 */
                DateTime thisMoment = DateTime.Now;
                string exceptionName = exc.GetType().ToString();
                string crashlogContent = "";

                // Show fatal error dialog
                MessageBox.Show($"Unhandled {exceptionName.Substring(exceptionName.LastIndexOf(".")+1)} thrown in method {exc.TargetSite.Name}: {exc.Message}" + 
                    "\nFor detailed information check the latest cemut-crashlog in executable folder.", "Fatal error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Build crashlog file content
                crashlogContent += $"Cemu Update Tool v{Application.ProductVersion} crashlog - {thisMoment.ToString(@"yyyy/MM/dd HH:mm:ss")}" +
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
                File.WriteAllText($@".\cemut-crashlog_{thisMoment.ToString("yyyy-MM-dd_HH.mm.ss")}.txt", crashlogContent);
                Application.Exit();
            }
            #endif
        }

        #if DEBUG
        [System.Runtime.InteropServices.DllImport("shcore.dll")]
        public static extern int GetProcessDpiAwareness(IntPtr proc, out int value);
        #endif
    }
}
