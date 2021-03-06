﻿using System;
using System.Windows.Forms;
using System.Globalization;
using CemuUpdateTool.Forms;
using CemuUpdateTool.Settings;

namespace CemuUpdateTool
{
    static class Program
    {
        public static float GetScreenDPIScaleFactor()
        {
            using (var gfx = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
                return gfx.DpiX / 96;
        }

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en");

            #if !DEBUG
            AppDomain.CurrentDomain.UnhandledException += HandleFatalExceptionAndExit;
            #endif

            TryLoadOptionsFromFile(args);
            Application.Run(new ContainerForm(new HomeForm()));
        }

        private static void TryLoadOptionsFromFile(string[] launchArgs)
        {
            try
            {
                // If the application is launched with the 'prefer-appdata-config' parameter, options are loaded
                // from %AppData% folder even if local file exists
                bool preferAppDataFile = launchArgs.Length > 0 && launchArgs[0].TrimStart('-', '/') == "prefer-appdata-config";
                if (Options.OptionsFileFound(preferAppDataFile))
                    Options.LoadFromCurrentlySelectedFile();
            }
            catch (Exception exc)
            {
                ShowOptionsLoadErrorDialog(exc);
            }
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
            var fatalException = (Exception)eventArgs.ExceptionObject;
            ShowFatalErrorDialog(fatalException);
            TryProduceCrashlog(fatalException);
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

        private static void TryProduceCrashlog(Exception fatalException)
        {
            try
            {
                var crashLogger = new CrashlogGenerator(fatalException);
                crashLogger.GenerateLogContent();
                crashLogger.WriteContentToTextFileInCurrentDirectory();
            }
            catch (Exception crashlogCreationExc)
            {
                MessageBox.Show(
                    $"An error occurred when producing crash log: {crashlogCreationExc.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error
                );
            }
        }
    }
}
