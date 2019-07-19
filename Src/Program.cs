using System;
using System.Windows.Forms;
using System.Globalization;
using System.Reflection;
using CemuUpdateTool.Forms;
using CemuUpdateTool.Settings;

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
            AppDomain.CurrentDomain.UnhandledException += HandleFatalExceptionAndExit;
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

            try
            {
                // If the application is launched with the 'prefer-appdata-config' parameter, options are loaded
                // from %AppData% folder even if local file exists
                bool preferAppDataFile = args.Length > 0 && args[0].TrimStart('-', '/') == "prefer-appdata-config";
                if (Options.OptionsFileFound(preferAppDataFile))
                    Options.LoadFromCurrentlySelectedFile();
            }
            catch (Exception exc)
            {
                ShowOptionsLoadErrorDialog(exc);
            }

            Application.Run(new ContainerForm(new HomeForm()));
        }

        public static float GetScreenDPIScaleFactor()
        {
            using (var gfx = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
                return gfx.DpiX / 96;
        }

        private static void ShowOptionsLoadErrorDialog(Exception optionsLoadException)
        {
            string message;
            if (optionsLoadException is OptionsParsingException parsingExc)
                message = "An unexpected error occurred when parsing options file: " +
                          $"{parsingExc.Message.TrimEnd('.')} at line {parsingExc.CurrentLine}.";
            else
                message = optionsLoadException.Message;

            MessageBox.Show(
                message + "\r\nDefault settings will be loaded instead.", "Error in settings.dat",
                MessageBoxButtons.OK, MessageBoxIcon.Warning
            );
        }

        private static void HandleFatalExceptionAndExit(object _, UnhandledExceptionEventArgs eventArgs)
        {
            var fatalException = (Exception) eventArgs.ExceptionObject;
            ShowFatalErrorDialog(fatalException);
            try
            {
                ProduceCrashlog(fatalException);
            }
            catch (Exception crashlogCreationExc)
            {
                MessageBox.Show(
                    $"An error occurred when producing crash log: {crashlogCreationExc.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error
                );
            }

            Environment.Exit(-1);
        }

        private static void ShowFatalErrorDialog(Exception exc)
        {
            string fullExceptionName = exc.GetType().ToString();
            string shortExceptionName = fullExceptionName.Substring(fullExceptionName.LastIndexOf(".") + 1);

            MessageBox.Show(
                $"Unhandled {shortExceptionName} thrown in method {exc.TargetSite.Name}: {exc.Message}\r\n" +
                "For detailed information check the produced crash log in the executable folder.",
                "Fatal error", MessageBoxButtons.OK, MessageBoxIcon.Error
            );
        }

        private static void ProduceCrashlog(Exception fatalException)
        {
            var crashLogger = new CrashlogGenerator(fatalException);
            crashLogger.GenerateLogContent();
            crashLogger.WriteContentToTextFileInCurrentDirectory();
        }
    }
}
