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
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();

            InitializeLogger();
            this.Closing += new CancelEventHandler(UninitializeLogger);

            log.Error("test");
        }

        #region Log System

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool logWatching = true;
        private Thread logWatcher;
        private log4net.Appender.MemoryAppender memoryAppender;
        private log4net.Appender.RollingFileAppender rollingFileAppender;

        private void InitializeLogger()
        {
            try
            {
                log4net.Layout.PatternLayout layout = new log4net.Layout.PatternLayout("%date %-5level - %message%newline");

                memoryAppender = new log4net.Appender.MemoryAppender();
                memoryAppender.Name = "MyMemoryAppender";
                memoryAppender.Layout = layout;

                rollingFileAppender = new log4net.Appender.RollingFileAppender();
                rollingFileAppender.Name = "MyRollingFileAppender";
                rollingFileAppender.File = "logs";
                rollingFileAppender.MaxSizeRollBackups = 100;
                rollingFileAppender.AppendToFile = true;
                rollingFileAppender.StaticLogFileName = false;
                rollingFileAppender.DatePattern = "yyyyMMdd";
                rollingFileAppender.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Date;
                rollingFileAppender.Layout = layout;

                log4net.Config.BasicConfigurator.Configure(memoryAppender, rollingFileAppender);

                logWatcher = new Thread(new ThreadStart(LogWatcher));
                logWatcher.Start();
            }
            catch (Exception ex)
            {

            }
        }

        private void UninitializeLogger(object sender, CancelEventArgs e)
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
                log4net.Core.LoggingEvent[] events = memoryAppender.GetEvents();
                memoryAppender.Clear();
                if (events != null && events.Length > 0)
                {
                    foreach (log4net.Core.LoggingEvent ev in events)
                    {
                        string line = ev.RenderedMessage + "\r\n";
                        AppendLog(line);
                    }
                }
                Thread.Sleep(500);
            }
        }

        #endregion
    }
}
