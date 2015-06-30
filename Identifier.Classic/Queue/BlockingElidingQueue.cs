using System.Collections.Generic;
using System.Threading;
using BirdTracker.Core.Utilities;
using Common.Logging;

namespace BirdTracker.Identifier.Classic.Queue
{
    public class BlockingElidingQueue<TKey, TValue> where TValue : class
    {
        private static readonly ILog Log = LogManager.GetLogger < BlockingElidingQueue<TKey, TValue>>();

        private readonly Queue<TKey> _keyQueue;
        private readonly IDictionary<TKey, TValue> _values;
        private readonly object _lock;
        private readonly AutoResetEvent _pendingMessages;

        public BlockingElidingQueue()
        {
            Log.Info("Creating queue");
            _keyQueue = new Queue<TKey>();
            _values = new Dictionary<TKey, TValue>();
            _lock = new object();
            _pendingMessages = new AutoResetEvent(false);
        }

        public void Enqueue(TKey key, TValue newValue)
        {
            using (var scopeLog = new ScopeLog("Enqueue"))
            {
                lock (_lock)
                {
                    scopeLog.Log("Got lock");
                    TValue value;
                    if (!_values.TryGetValue(key, out value))
                    {
                        // we don't have this key - add it to the queue
                        _keyQueue.Enqueue(key);
                    }
                    // add/replace the value
                    _values[key] = newValue;
                }
                _pendingMessages.Set();
            }
        }

        public TValue Dequeue()
        {
            TValue value;
            using (var scopeLog = new ScopeLog("Dequeue"))
            {
                bool moreMessages;
                _pendingMessages.WaitOne();
                scopeLog.Log("Got pending messages");
                lock (_lock)
                {
                    scopeLog.Log("Got lock");
                    if (_keyQueue.Count == 0) return null;
                    var key = _keyQueue.Dequeue();
                    moreMessages = _keyQueue.Count > 0;
                    value = _values[key];
                    _values.Remove(key);
                }
                if (moreMessages)
                {
                    scopeLog.Log("Signalling pending messages");
                    _pendingMessages.Set();
                }
            }
            return value;
        }
    }
}
