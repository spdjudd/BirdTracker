using System;
using System.Collections.Generic;
using System.Drawing;
using BirdTracker.Core.Model;
using Common.Logging;
using Emgu.CV;
using Emgu.CV.Cvb;
using Emgu.CV.Structure;
using Emgu.CV.VideoSurveillance;

namespace BirdTracker.Service
{
    public class ObjectTracker : IObjectTracker
    {
        private static readonly ILog Log = LogManager.GetLogger<ObjectTracker>();

        private readonly TrackerSettings _settings;
        private readonly Dictionary<uint, TrackedObject> _trackedObjectIdentities;
        
        private BackgroundSubtractor _foregroundDetector;
        private CvBlobDetector _blobDetector;
        private CvBlobs _blobs;
        private CvTracks _tracks;
        private uint _nextTrackId = 1;
        private int _imageWidth;
        private int _imageHeight;

        public ObjectTracker(
            TrackerSettings settings)
        {
            _settings = settings;
            _foregroundDetector = new BackgroundSubtractorMOG2(_settings.BackgroundSubtractorHistory.Value, _settings.BackgroundSubtractorMaxComponents.Value, false);
            _blobDetector = new CvBlobDetector();
            _blobs = new CvBlobs();
            _tracks = new CvTracks();
            _trackedObjectIdentities = new Dictionary<uint, TrackedObject>();
        }

        public void Update(Image<Bgr, Byte> image)
        {
            if (_foregroundDetector == null)
            {
                Log.Warn("Updating when disposed - ignoring");
                return;
            }

            if (image == null)
            {
                return;
            }

            InitialiseFromFrame(image);

            // update the foreground detector
            _foregroundDetector.Update(image);//, 0.005);

            // erode/dilate to remove noise
            var fg = _foregroundDetector.ForegroundMask.Erode(2);
            fg = fg.Dilate(2);

            // detect bloba
            _blobDetector.Detect(fg, _blobs);

            // link blobs to tracks
            _tracks.Update(_blobs, _settings.DistanceThreshold.Value, _settings.MaxInactiveFrames.Value, _settings.ActivityThreshold.Value);

            // process the tracks
            foreach (var track in _tracks.Values)
            {
                ProcessTrack(track, image);
            }
        }

        public IEnumerable<TrackedObject> Tracks { get { return _trackedObjectIdentities.Values; } } 

        /// <summary>
        /// Here: mark active as active, set crop, public enumerable
        /// UI thread: iterate observable - throw out any not active, mark all active inactive, add any new, send new images for id, create imagesource
        /// </summary>
        /// <param name="track"></param>
        /// <param name="image"></param>
        private void ProcessTrack(CvTrack track, Image<Bgr, Byte> image)
        {
            // can this happen for tracks that later resume?
            if (track.Active <= 0) return;

            // get the tracked object currently using this id, if any
            TrackedObject myTrack;
            _trackedObjectIdentities.TryGetValue(track.Id, out myTrack);

            if (track.Lifetime <= 1 && myTrack != null && myTrack.TrackingState == TrackingState.Dead)
            {
                _trackedObjectIdentities.Remove(track.Id);
                myTrack = null;
            }                

            if (track.Lifetime < _settings.MinFramesToStartIdentifying.Value)
            {
                return;
            }

            if (myTrack == null)
            {
                myTrack = new TrackedObject{Id = _nextTrackId++, TrackingState = TrackingState.New};
            }
            else
            {
                myTrack.TrackingState = TrackingState.Current;
            }
            try
            {
                myTrack.MaxSize = Math.Max(myTrack.MaxSize,
                    Math.Max(track.BoundingBox.Height, track.BoundingBox.Width));
                if (myTrack.MaxSize < _settings.MinSizeToIdentify.Value) return;
                if (myTrack.TrackingState == TrackingState.New)
                    _trackedObjectIdentities[track.Id] = myTrack;
                SetTrackedImage(track, image, myTrack);
            }
            catch (Exception ex)
            {
                Log.Error("Exception processing track", ex);
            }
        }

        private void SetTrackedImage(CvTrack track, Image<Bgr, byte> image, TrackedObject myTrack)
        {
            var windowSize = _settings.SubImageMultiplier.Value > 1 
                ? _settings.SubImageMultiplier.Value * (1 + (myTrack.MaxSize / _settings.SubImageMultiplier.Value))
                : myTrack.MaxSize;
            var x = Math.Max(0, Convert.ToInt32(track.Centroid.x) - windowSize / 2);
            x = Math.Min(_imageWidth - windowSize, x);
            var y = Math.Max(0, Convert.ToInt32(track.Centroid.y) - windowSize / 2);
            y = Math.Min(_imageHeight - windowSize, y);
            var sourceRect = new Rectangle(x, y, windowSize, windowSize);
            var subImage = image.Copy(sourceRect);
            myTrack.ProcessTrackerResult(sourceRect, subImage);
        }

        private void InitialiseFromFrame(Image<Bgr, Byte> image)
        {
            if (_imageHeight > 0) return;
            _imageHeight = image.Height;
            _imageWidth = image.Width;
        }

        public void Dispose()
        {
            if (_foregroundDetector == null) return;
            try
            {
                _blobDetector.Dispose();
                _blobs.Dispose();
                _tracks.Dispose();
                ((IDisposable)_foregroundDetector).Dispose();
            }
            catch (Exception ex)
            {
                Log.Error("Exception disposing foreground detector", ex);
            }
            _blobDetector = null;
            _blobs = null;
            _tracks = null;
            _foregroundDetector = null;
        }
    }
}
