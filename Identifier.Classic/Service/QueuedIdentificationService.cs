using System;
using System.Threading;
using System.Threading.Tasks;
using BirdTracker.Core.Interfaces;
using BirdTracker.Core.Model;
using BirdTracker.Core.Utilities;
using BirdTracker.Identifier.Classic.Queue;
using Common.Logging;

namespace BirdTracker.Identifier.Classic.Service
{
    public class QueuedIdentificationService : IIdentificationService
    {
        private static readonly ILog Log = LogManager.GetLogger<QueuedIdentificationService>();

        private readonly BlockingElidingQueue<uint, Tuple<TrackedObject, TrackedObjectFrame>> _queue;
        private readonly IImageIdentifier _imageIdentifier;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public QueuedIdentificationService(IdentifierSettings settings, IImageIdentifier imageIdentifier)
        {
            _imageIdentifier = imageIdentifier;
            _queue = new BlockingElidingQueue<uint, Tuple<TrackedObject, TrackedObjectFrame>>();
            _cancellationTokenSource = new CancellationTokenSource();
            for (var i = 0; i < settings.NumThreads; ++i)
            {
                Task.Run((Action)Loop, _cancellationTokenSource.Token);
            }
        }

        public void Identify(TrackedObject trackedObject)
        {
            _queue.Enqueue(trackedObject.Id, new Tuple<TrackedObject, TrackedObjectFrame>(trackedObject, trackedObject.CurrentFrame));
        }

        private void Loop()
        {
            while (true)
            {
                using (var scopeLog = new ScopeLog("QueuedIdentificationService Loop"))
                {
                    var tuple = _queue.Dequeue();
                    if (tuple == null) continue;
                    scopeLog.Log("Got item from queue");
                    var trackedObject = tuple.Item1;
                    var frame = tuple.Item2;
                    frame.IdResults = _imageIdentifier.IdentifyAsync(tuple.Item2.RawImage).Result;
                    trackedObject.ProcessIdResult(frame);
                }
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}
