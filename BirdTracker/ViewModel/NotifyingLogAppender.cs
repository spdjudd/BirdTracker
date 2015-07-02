using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using log4net;
using log4net.Appender;
using log4net.Core;

namespace BirdTracker.ViewModel
{
    public class NotifyingLogAppender : AppenderSkeleton, INotifyPropertyChanged
    {
        public static NotifyingLogAppender Appender
        {
            get
            {
                return LogManager.GetCurrentLoggers()
                    .SelectMany(log => log.Logger.Repository.GetAppenders())
                    .OfType<NotifyingLogAppender>()
                    .FirstOrDefault();
            }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                if (_message == value) return;
                _message = value;
                OnPropertyChanged("Message");
            }
        }

        private Level _level;
        public Level Level
        {
            get { return _level; }
            set
            {
                if (_level == value) return;
                _level = value;
                OnPropertyChanged("Level");
            }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            var writer = new StringWriter(CultureInfo.InvariantCulture);
            Layout.Format(writer, loggingEvent);
            Message = writer.ToString();
            Level = loggingEvent.Level;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var handlers = PropertyChanged;
            if (handlers == null) return;
            if (!Dispatcher.CurrentDispatcher.CheckAccess())
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action<string>(OnPropertyChanged), propertyName);
                return;
            }
            handlers(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
