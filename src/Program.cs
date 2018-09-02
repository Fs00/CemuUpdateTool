using System;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Reflection;

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
            AppDomain.CurrentDomain.UnhandledException += (o, e) => {
                GenerateCrashlog(e.ExceptionObject as Exception);
                Environment.Exit(0);
            };
            #endif

            try
            {
                // Check that ValueTuple types can be instantiated. If it fails, it means that ValueType assembly can't be loaded from local DLL or GAC
                AppDomain.CurrentDomain.CreateInstance("System.ValueTuple, Version = 4.0.3.0, Culture = neutral, PublicKeyToken = cc7b13ffcd2ddd51", "System.ValueTuple");
            }
            catch
            {
                Assembly valueTupleAssembly = null;
                try
                {
                    // Load ValueTuple assembly from resources
                    valueTupleAssembly = Assembly.Load(Properties.Resources.ValueTupleDLL);
                }
                catch
                {
                    // If it fails, show error dialog and quit
                    MessageBox.Show("An error occurred when trying to load a necessary component for the program. This is likely due to corrupted executable file or incompatible OS version.",
                                    "Fatal error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }

                // Register AssemblyResolve event handler (last resort in assembly resolving before failure) to resolve all ValueTuple references
                // Reference: docs.microsoft.com/it-it/dotnet/framework/deployment/best-practices-for-assembly-loading#no-context
                AppDomain.CurrentDomain.AssemblyResolve += (o, e) => {
                    if (e.Name == valueTupleAssembly.FullName)
                        return valueTupleAssembly;

                    return null;
                }; 
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

            try
            {
                // Write crashlog content on file
                File.WriteAllText($@".\cemuUpdateTool-crashlog_{thisMoment.ToString("yyyy-MM-dd_HH.mm.ss")}.txt", crashlogContent);
            }
            catch (Exception fileWriteExc)
            {
                MessageBox.Show($"Error when writing crash log on file: {fileWriteExc.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
