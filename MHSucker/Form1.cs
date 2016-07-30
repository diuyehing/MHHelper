using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace MHSucker
{
    public partial class MainForm : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool logWatching = true;
        private log4net.Appender.MemoryAppender logger;
        private Thread logWatcher;

        public MainForm()
        {
            InitializeComponent();

            this.Closing += new CancelEventHandler(MainFormClosing);
            logger = new log4net.Appender.MemoryAppender();

            log4net.Config.BasicConfigurator.Configure(logger);

            logWatcher = new Thread(new ThreadStart(LogWatcher));
            logWatcher.Start();
        }

        void MainFormClosing(object sender, CancelEventArgs e)
        {
            logWatching = false;
            logWatcher.Join();
        }

        private void AppendLog(string msg)
        {
            if (LogTextBox.InvokeRequired)
            {
                BeginInvoke(new Action<string>(DoAppendLog), msg);
            }
            else
            {
                DoAppendLog(msg);
            }
        }

        private void DoAppendLog(string msg)
        {
            StringBuilder builder;
            if (LogTextBox.Lines.Length > 99)
            {
                builder = new StringBuilder(LogTextBox.Text);
                builder.Remove(0, LogTextBox.Text.IndexOf('\r', 3000) + 2);
                builder.Append(msg);
                LogTextBox.Clear();
                LogTextBox.AppendText(builder.ToString());
            }
            else
            {
                LogTextBox.AppendText(msg);
            }
        }

        private void LogWatcher()
        {
            while (logWatching)
            {
                log4net.Core.LoggingEvent[] events = logger.GetEvents();
                logger.Clear();
                if (events != null && events.Length > 0)
                {
                    foreach (log4net.Core.LoggingEvent ev in events)
                    {
                        string line = ev.LoggerName + ": " + ev.RenderedMessage + "\r\n";
                        AppendLog(line);
                    }
                }
                Thread.Sleep(500);
            }
        }
    }
}
