using System;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using Microsoft.Win32;

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
            // Attach event handler to generate a crashlog when an unhandled exception is thrown
            AppDomain.CurrentDomain.UnhandledException += (o, e) => GenerateCrashlog(e.ExceptionObject as Exception);
            #endif

            // Load ValueTuple DLL from resources if .NET Framework version is < 4.7
            try
            {
                if (IsDotNetVersionLessThan47())
                    System.Reflection.Assembly.Load(Properties.Resources.ValueTupleDLL);
            }
            catch
            {
                MessageBox.Show("An error occurred when trying to load a necessary component for the program. This is likely due to corrupted executable file or incompatible OS version.",
                                "Fatal error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            // Load options
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

        /*
         *  Checks a registry key to determine if .NET framework version is < 4.7
         *  Reference: docs.microsoft.com/en-us/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed
         */
        private static bool IsDotNetVersionLessThan47()
        {
            int releaseCode = (int)Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\NET Framework Setup\NDP\v4\Full").GetValue("Release");
            return releaseCode < 460798;
        }
    }
}
