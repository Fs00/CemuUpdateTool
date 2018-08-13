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

            #if !DEBUG
            // Attach event handler to generate a crashlog and exit the program when an unhandled exception is thrown
            var appDomain = AppDomain.CurrentDomain;
            appDomain.UnhandledException += (o, e) => {
                GenerateCrashlog(e.ExceptionObject as Exception);
                Application.Exit();
            };

            // Make the application crash if ValueTuple isn't present (to be improved)
            ValueTuple.Create();
            #endif

            // LOAD OPTIONS
            OptionsManager opts;
            try
            {
                // If the application is launched with the 'force-appdata-config' parameter, options are loaded from %AppData% even if local file exists
                bool preferAppDataFile = args.Length > 0 && args[0].TrimStart('-', '/') == "force-appdata-config";
                string optionsFilePath = OptionsManager.LookForOptionsFile(preferAppDataFile);
                opts = new OptionsManager(optionsFilePath);
            }
            catch (Exception exc)
            {
                string message;
                if (exc is OptionsParsingException parsingExc)
                    message = $"An unexpected error occurred when parsing options file: {parsingExc.Message.TrimEnd('.')} at line {parsingExc.CurrentLine}.";
                else
                    message = exc.Message;

                MessageBox.Show(message + "\r\nDefault settings will be loaded instead.", "Error in settings.dat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                opts = new OptionsManager();
            }

            /*int res = GetProcessDpiAwareness(IntPtr.Zero, out int value);
            System.Diagnostics.Debug.WriteLine($"DpiAwareness: {(res == 0 ? value.ToString() : "ERR")}");*/
            Application.Run(new ContainerForm(new HomeForm(opts)));
        }

        /*
         *  Create and save a crash log with information on the thrown exception
         */
        private static void GenerateCrashlog(Exception exc)
        {
            DateTime thisMoment = DateTime.Now;
            string exceptionName = exc.GetType().ToString();
            string crashlogContent = "";

            // Show fatal error dialog
            MessageBox.Show($"Unhandled {exceptionName.Substring(exceptionName.LastIndexOf(".") + 1)} thrown in method {exc.TargetSite.Name}: {exc.Message}" +
                "\nFor detailed information check the produced crash log in executable folder.", "Fatal error", MessageBoxButtons.OK, MessageBoxIcon.Error);

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
            File.WriteAllText($@".\cemuUpdateTool-crashlog_{thisMoment.ToString("yyyy-MM-dd_HH.mm.ss")}.txt", crashlogContent);
        }

        #if DEBUG
        [System.Runtime.InteropServices.DllImport("shcore.dll")]
        public static extern int GetProcessDpiAwareness(IntPtr proc, out int value);
        #endif
    }
}
