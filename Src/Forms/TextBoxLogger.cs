using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CemuUpdateTool.Forms
{
    /*
     * Performs asynchronous logging on a TextBox.
     * Once started, buffered log contents are written into the textBox in a separate thread every TIME_BETWEEN_UPDATE_ITERATIONS ms.
     * The same instance can be reused after starting and stopping.
     */
    public class TextBoxLogger
    {
        private readonly StringBuilder logBuffer;
        private readonly TextBox logTextBox;

        private volatile bool continueUpdating;
        private readonly EventWaitHandle updaterLoopFinished = new EventWaitHandle(false, EventResetMode.AutoReset);

        private const int TIME_BETWEEN_UPDATE_ITERATIONS_MS = 40;

        public bool IsRunning => continueUpdating;
        
        public TextBoxLogger(TextBox logTextBox)
        {
            this.logTextBox = logTextBox;
            logBuffer = new StringBuilder(1000);
        }
        
        public void Start()
        {
            var updaterThread = new Thread(RunUpdaterLoop) {
                IsBackground = true,
                Name = "LogTextboxUpdater"
            };
            continueUpdating = true;
            updaterThread.Start();
        }

        private void RunUpdaterLoop()
        {
            while (continueUpdating)
            {
                Thread.Sleep(TIME_BETWEEN_UPDATE_ITERATIONS_MS);
                if (logTextBox.Visible)
                    UpdateTextBox();
            }
            updaterLoopFinished.Set();
        }

        /*
         *  Prints log buffer content in the textBox and flushes the buffer.
         *  Lock avoids race conditions with AppendLogMessage.
         */
        private void UpdateTextBox()
        {
            if (logBuffer.Length > 0)
            {
                string logTextToWrite;
                lock (logBuffer)
                {
                    logTextToWrite = logBuffer.ToString();
                    logBuffer.Clear();
                }
                logTextBox.AppendText(logTextToWrite);
            }
        }

        /*
         *  Appends a message to the buffer to be subsequently written in the textBox
         */
        public void AppendLogMessage(string message, bool newLine = true)
        {
            // Lock avoids race conditions with UpdateTextBox
            lock (logBuffer)
            {
                logBuffer.Append(message);
                if (newLine)
                    logBuffer.Append("\r\n");
            }
        }

        public void StopAndPrintAllBufferedContent()
        {
            continueUpdating = false;
            updaterLoopFinished.WaitOne();
            UpdateTextBox();
        }
    }
}
