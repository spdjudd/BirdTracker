using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Common.Logging;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Windows;
using BirdTracker.Core.Utilities;

namespace BirdTracker.Core.Model
{
    public class TrackedObject : Bindable
    {
        private static readonly ILog Log = LogManager.GetLogger<TrackedObject>();
 
        public TrackedObject()
        {
            Frames = new ObservableCollection<TrackedObjectFrame>();
        }

        public void ProcessTrackerResult(Rectangle sourceRect, Image<Bgr, byte> subImage)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                // process asynchronously on UI thread
                Application.Current.Dispatcher.BeginInvoke(new Action<Rectangle, Image<Bgr, Byte>>(ProcessTrackerResult), sourceRect, subImage);
                return;
            }
            CurrentFrame = new TrackedObjectFrame
            {
                RawImage = subImage,
                SourceRectangle = sourceRect
            };
            Frames.Add(CurrentFrame);
        }

        public void ProcessIdResult(TrackedObjectFrame frame)
        {
            if (frame.IdResults == null)
            {
                Log.Warn("Missing frame.IdResults");
                return;
            }
            if (IdResults == null)
            {
                IdResults = frame.IdResults.ToDictionary(x => x.Id, x => new IdScore(x.Id, x.Score));
            }
            else
            {
                foreach (var idr in frame.IdResults)
                {
                    IdResults[idr.Id].Score += idr.Score;
                }
            }
            Identity = IdResults.OrderByDescending(x => x.Value.Score).Select(x => x.Key).First();
            frame.Identity = frame.IdResults.OrderByDescending(x => x.Score).First().Id;
        }

        public uint Id { get; set; }

        private TrackedObjectFrame _currentFrame;
        public TrackedObjectFrame CurrentFrame
        {
            get { return _currentFrame; }
            set
            {
                Set(ref _currentFrame, value);
            }
        }

        public ObservableCollection<TrackedObjectFrame> Frames { get; set; }

        private IDictionary<string, IdScore> _idResults;
        public IDictionary<string, IdScore> IdResults
        {
            get { return _idResults; }
            set
            {
                Set(ref _idResults, value);
            }
        } 

        private string _identity;
        public string Identity
        {
            get { return _identity; }
            set
            {
                Set(ref _identity, value);
            }
        }

        private TrackingState _state;
        public TrackingState TrackingState
        {
            get { return _state; }
            set
            {
                Set(ref _state, value);
            }
        }

        private int _maxSize;
        public int MaxSize
        {
            get { return _maxSize; }
            set
            {
                Set(ref _maxSize, value);
            }
        }

        private DateTime _trackStart;
        public DateTime TrackStart
        {
            get { return _trackStart; }
            set
            {
                Set(ref _trackStart, value);
            }
        }

        private DateTime _trackEnd;
        public DateTime TrackEnd
        {
            get { return _trackEnd; }
            set
            {
                Set(ref _trackEnd, value);
            }
        }
    }

    public enum TrackingState
    {
        New,
        Current,
        Inactive,
        Dead
    }
}
