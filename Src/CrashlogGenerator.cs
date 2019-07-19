using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    class CrashlogGenerator
    {
        private readonly Exception exceptionToLog;

        private readonly DateTime thisMoment = DateTime.Now;
        private readonly StringBuilder logContent = new StringBuilder(1000);

        public CrashlogGenerator(Exception exceptionToLog)
        {
            this.exceptionToLog = exceptionToLog;
        }

        public void GenerateLogContent()
        {
            GenerateSummary();
            GenerateAdditionalInformation();
        }

        public void WriteContentToTextFileInCurrentDirectory()
        {
            File.WriteAllText(GenerateCrashlogFileName(), logContent.ToString());
        }

        private string GenerateCrashlogFileName()
        {
            return $"cemuUpdateTool-crashlog_{thisMoment.ToString("yyyy-MM-dd_HH.mm.ss")}.txt";
        }

        private void GenerateSummary()
        {
            logContent.AppendLine(
                $"Cemu Update Tool v{Application.ProductVersion} crashlog - {thisMoment.ToString(@"yyyy/MM/dd HH:mm:ss")}"
            );
            logContent.AppendLine("------------------------------------------------------");

            logContent.AppendLine(
                $@"Method ""{exceptionToLog.TargetSite}"" threw a {GetFullExceptionName(exceptionToLog)} due to the following reason:"
            );
            logContent.AppendLine(exceptionToLog.Message);
        }

        private void GenerateAdditionalInformation()
        {
            logContent.AppendLine();
            logContent.AppendLine("Additional error information");
            logContent.AppendLine("------------------------------");

            GenerateInnerExceptionDetails();

            logContent.AppendLine(string.Format("HResult: 0x{0:X8}", exceptionToLog.HResult));
            logContent.AppendLine("Stack trace:");
            logContent.AppendLine(exceptionToLog.StackTrace);
        }

        private void GenerateInnerExceptionDetails()
        {
            logContent.Append("Inner exception: ");
            if (exceptionToLog.InnerException == null)
                logContent.AppendLine("none");
            else
                logContent.AppendLine(
                    $"{GetFullExceptionName(exceptionToLog.InnerException)} in method " +
                    $@"""{exceptionToLog.InnerException.TargetSite}"": {exceptionToLog.InnerException.Message}"
                );
        }

        private static string GetFullExceptionName(Exception exc) => exc.GetType().ToString();
    }
}
