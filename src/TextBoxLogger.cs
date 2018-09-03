using System;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Forms;

namespace CemuUpdateTool
{
    public class TextBoxLogger
    {
        StringBuilder logBuffer;        // buffer used to store log messages that must be written into textbox
        Dispatcher workDispatcher;      // used to update textbox on another thread
        TextBox logTextbox;             // textbox used as a log

        public bool IsReady => !(workDispatcher == null || workDispatcher.HasShutdownStarted);
        public bool IsStopped => workDispatcher.HasShutdownStarted;

        public TextBoxLogger(TextBox logTextbox)
        {
            this.logTextbox = logTextbox;
            logBuffer = new StringBuilder(1000);
            Start();
        }

        /*
         *  Start the dispatcher and wait synchronously for it to be ready
         */
        private void Start(int maxWaitingCycles = 100)
        {
            // Create and start the thread on which the dispatcher will run
            Thread dispatcherThread = new Thread(() => Dispatcher.Run());
            dispatcherThread.IsBackground = true;
            dispatcherThread.Name = "LogTextboxUpdater";
            dispatcherThread.Start();

            // Wait until dispatcher is running
            int cycleIndex = 0;
            do
            {
                Thread.Sleep(10);
                workDispatcher = Dispatcher.FromThread(dispatcherThread);
                Debug.WriteLine((workDispatcher == null) ? "Couldn't get dispatcher. Retrying..." : "Dispatcher obtained.");
                cycleIndex++;
            }
            while (workDispatcher == null && cycleIndex < maxWaitingCycles);

            // If dispatcher is still null, it means that we had some unknown problems
            if (workDispatcher == null)
            {
                dispatcherThread.Abort();
                throw new TimeoutException("Failed to start dispatcher due to some unknown problems.");
            }
        }

        /*
         *  Prints log buffer content asynchronously in the textbox and flushes the buffer.
         *  Lock avoids race conditions with AppendLogMessage.
         */
        public void UpdateTextBox()
        {
            if (logTextbox.Visible)
            {
                lock (logBuffer)
                {
                    if (logBuffer.Length > 0)
                    {
                        string log = logBuffer.ToString();
                        workDispatcher.InvokeAsync(() => logTextbox.AppendText(log));
                        logBuffer.Clear();
                    }
                }
            }
        }

        /*
         *  Appends a message to the buffer to be subsequently written in the textbox
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

        /*
         *  Wait for the Dispatcher to do all its work and stop, then update the textbox with the remaining logBuffer content
         */
        public void StopAndWaitShutdown()
        {
            workDispatcher.InvokeAsync(() => workDispatcher.InvokeShutdown()).Wait();
            logTextbox.AppendText(logBuffer.ToString());
            logBuffer.Clear();
        }

        public void StopAbruptly()
        {
            workDispatcher.InvokeShutdown();
        }
    }
}
