using System;
using System.Diagnostics;
using Common.Logging;

namespace BirdTracker.Core.Utilities
{
    public class ScopeLog : IDisposable
    {
        private static readonly ILog Logger = LogManager.GetLogger<ScopeLog>();

        private readonly string _label;
        private readonly Stopwatch _stopwatch;
        private readonly Action<string> _logAction;

        public ScopeLog(string label, bool logDebug = true)
        {
            _logAction = logDebug ? new Action<string>(Logger.Debug) : Logger.Info;
            _label = label;
            _stopwatch = new Stopwatch();
            _logAction(string.Format("{0} ==> Enter", _label));
            _stopwatch.Start();
        }

        public void Log(string message)
        {
            _logAction(string.Format("{0}: {1}, {2}ms", _label, message, _stopwatch.ElapsedMilliseconds));
        }

        public void Log(string format, params object[] parameters)
        {
            var message = string.Format(format, parameters);
            Log(message);
        }

        public void Dispose()
        {
            _logAction(string.Format("{0}: <== Exit, {1}ms", _label, _stopwatch.ElapsedMilliseconds));
            _stopwatch.Stop();
        }
    }
}
