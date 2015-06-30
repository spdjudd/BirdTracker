using System;
using BirdTracker.Core.Utilities;
using Common.Logging;
using Emgu.CV;

namespace BirdTracker.Service
{
    // todo: combine with FileVideoSource
    public class LiveVideoSource : Bindable, IVideoSource
    {
        private static readonly ILog Log = LogManager.GetLogger<FileVideoSource>();

        private Capture _capture;

        public LiveVideoSource()
        {
            try
            {
                _capture = new Capture();
                _capture.ImageGrabbed += CaptureOnImageGrabbed;
            }
            catch (Exception ex)
            {
                Log.Error("Unable to capture from default camera", ex);
                throw;
            }
        }
        public event EventHandler<ImageEventArgs> NewFrame;

        public void Start()
        {
            if (VideoState == VideoState.Running) return;
            if (VideoState != VideoState.Stopped) throw new InvalidOperationException("Cannot start video - not initialised");
            _capture.Start();
            VideoState = VideoState.Running;
        }

        private void CaptureOnImageGrabbed(object sender, EventArgs eventArgs)
        {
            var handler = NewFrame;
            if (handler == null) return;
            var image = _capture.RetrieveBgrFrame();

            handler(this, new ImageEventArgs { Image = image });
        }

        public void Stop()
        {
            if (VideoState == VideoState.Stopped) return;
            if (VideoState != VideoState.Running) throw new InvalidOperationException("Cannot stop video - not initialised");
            _capture.Stop();
            VideoState = VideoState.Stopped;
        }

        public void Frame()
        {
            if (VideoState == VideoState.Unknown) throw new InvalidOperationException("Cannot freeze frame - not initialised");
            if (VideoState == VideoState.Running)
            {
                Stop();
                return;
            }
            _capture.Grab();
        }

        public void Reset()
        {
            Stop();
        }

        private VideoState _videoState;
        public VideoState VideoState
        {
            get { return _videoState; }
            private set
            {
                Set(ref _videoState, value);
            }
        }

        public void Dispose()
        {
            if (_capture == null) return;
            _capture.Stop();
            _capture.ImageGrabbed -= CaptureOnImageGrabbed;
            _capture.Dispose();
            _capture = null;
            VideoState = VideoState.Unknown;
        }
    }
}
