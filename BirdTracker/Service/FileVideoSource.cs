using System;
using System.Threading;
using BirdTracker.Core.Model;
using BirdTracker.Core.Utilities;
using Common.Logging;
using Emgu.CV;

namespace BirdTracker.Service
{
    public class FileVideoSource : Bindable, IVideoSource
    {
        private static readonly ILog Log = LogManager.GetLogger<FileVideoSource>();
        private readonly Profile _profile;
        private Capture _capture;
        private DateTime _nextFrame;

        public FileVideoSource(Profile profile)
        {
            _profile = profile;
            Initialise();
        }

        public event EventHandler<ImageEventArgs> NewFrame;

        public void Start()
        {
            if (VideoState == VideoState.Running) return;
            if (VideoState != VideoState.Stopped) throw new InvalidOperationException("Cannot start video - not initialised");
            _nextFrame = DateTime.Now.AddMilliseconds(_profile.FrameSpacingMs);
            _capture.Start();
            VideoState = VideoState.Running;
        }

        public void Stop()
        {
            if (VideoState == VideoState.Stopped) return;
            if (VideoState != VideoState.Running) throw new InvalidOperationException("Cannot stop video - not initialised");
            _capture.Stop();
            VideoState = VideoState.Stopped;
            FrameHeadroomMs = 0;
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
            Dispose();
            Initialise();
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

        private double _frameHeadroomMs;
        public double FrameHeadroomMs
        {
            get { return _frameHeadroomMs; }
            set { Set(ref _frameHeadroomMs, value); }
        }

        private void CaptureOnImageGrabbed(object sender, EventArgs e)
        {
            var handler = NewFrame;
            if (handler == null) return;

            if (VideoState == VideoState.Running && _profile.FrameSpacingMs > 0)
            {
                var timeToNextFrame = _nextFrame - DateTime.Now;
                FrameHeadroomMs = timeToNextFrame.TotalMilliseconds;
                if (timeToNextFrame > TimeSpan.Zero)
                {
                    Thread.Sleep(timeToNextFrame);
                }
                else
                {
                    Log.WarnFormat("Slow frame rate - missed by {0}ms", -timeToNextFrame.TotalMilliseconds);
                }
            }
            else
            {
                FrameHeadroomMs = 0;
            }
            _nextFrame = DateTime.Now.AddMilliseconds(_profile.FrameSpacingMs);
            var image = _capture.RetrieveBgrFrame();

            handler(this, new ImageEventArgs { Image = image });
        }

        private void Initialise()
        {
            try
            {
                _capture = new Capture(_profile.FilePath);
                _capture.ImageGrabbed += CaptureOnImageGrabbed;
                VideoState = VideoState.Stopped;
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Unable to open file {0}", ex, _profile.FilePath);
                throw;
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
