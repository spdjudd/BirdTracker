using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using BirdTracker.Core.Interfaces;
using BirdTracker.Core.Model;
using BirdTracker.Core.Service;
using BirdTracker.Core.Utilities;
using BirdTracker.Identifier.Classic.Service;
using BirdTracker.Service;
using BirdTracker.Utilities;
using Common.Logging;
using Emgu.CV;
using Emgu.CV.Structure;
using Identifier.Akka.Service;

namespace BirdTracker.Model
{
    public class MainModel : Bindable, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger<MainModel>();

        private ObjectTracker _objectTracker;
        private IIdentificationService _identificationService;

        public MainModel()
        {
            CurrentTrackedObjects = new ObservableCollection<TrackedObject>();
            ArchivedObjects = new ObservableCollection<TrackedObject>();
        }

        public IVideoSource VideoSource { get; set; }
        public ObservableCollection<TrackedObject> CurrentTrackedObjects { get; set; }
        public ObservableCollection<TrackedObject> ArchivedObjects { get; set; } 
        
        private Profile _profile;
        public Profile Profile
        {
            get { return _profile; }
            private set
            {
                Set(ref _profile, value);
            }
        }

        private BitmapSource _mainImageSource;
        public BitmapSource MainImageSource
        {
            get { return _mainImageSource; }
            set
            {
                Set(ref _mainImageSource, value);
            }
        }

        // todo - move to vm
        private TrackedObject _selectedObject;
        public TrackedObject SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                if (value == null && ArchivedObjects.Count > 0) return;
                Set(ref _selectedObject, value);
            }
        }

        public void LoadProfile(Profile profile)
        {
            CloseCurrentProfile();
            CurrentTrackedObjects.Clear();
            ArchivedObjects.Clear();
            Profile = profile;
            var identifier = CreateIdentifier();
            _identificationService = CreateIdService(identifier);
            _objectTracker = new ObjectTracker(profile.TrackerSettings);
            VideoSource = VideoSourceFactory.Create(profile);
            VideoSource.NewFrame += VideoSourceOnNewFrame;
        }

        private IImageIdentifier CreateIdentifier()
        {
            switch (Profile.IdentifierSettings.IdentifierType)
            {
                case IdentifierType.Torch:
                    return new ImageIdentifier(Profile.IdentifierSettings);
                case IdentifierType.Dummy:
                    return new MockImageIdentifier();
                default:
                    throw new NotSupportedException(string.Format("Identifier {0} is not supported", Profile.IdentifierSettings.IdentifierType));
            }
        }

        private IIdentificationService CreateIdService(IImageIdentifier identifier)
        {
            switch (Profile.IdentifierSettings.IdServiceType)
            {
                case IdServiceType.Akka:
                    return new AkkaIdentificationService(Profile.IdentifierSettings, identifier);
                case IdServiceType.Classic:
                    return new QueuedIdentificationService(Profile.IdentifierSettings, identifier);
                default:
                    throw new NotSupportedException(string.Format("Id Service {0} is not supported", Profile.IdentifierSettings.IdServiceType));
            }
        }

        private void VideoSourceOnNewFrame(object sender, ImageEventArgs imageEventArgs)
        {
            _objectTracker.Update(imageEventArgs.Image);

            ProcessTracks(imageEventArgs.Image);

            SubmitIdRequests();
        }

        private void ProcessTracks(Image<Bgr, Byte> image)
        {
            if (_objectTracker == null) return;
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                // run synchronously on UI thread
                Application.Current.Dispatcher.Invoke(new Action<Image<Bgr, Byte>>(ProcessTracks), image);
                return;
            }

            foreach (var trackedObject in _objectTracker.Tracks)
            {
                switch (trackedObject.TrackingState)
                {
                    case TrackingState.New:
                        CurrentTrackedObjects.Add(trackedObject);
                        trackedObject.TrackingState = TrackingState.Inactive;
                        break;
                    case TrackingState.Inactive:
                        ArchivedObjects.Add(trackedObject);
                        CurrentTrackedObjects.Remove(trackedObject);
                        trackedObject.TrackingState = TrackingState.Dead;
                        break;
                    case TrackingState.Current:
                        trackedObject.TrackingState = TrackingState.Inactive;
                        break;
                }
                if (trackedObject.TrackingState == TrackingState.Dead) continue;
                trackedObject.CurrentFrame.Image =
                    BitmapSourceConvert.ToBitmapSource(trackedObject.CurrentFrame.RawImage);
            }

            // draw tracks on frame
            foreach (var track in CurrentTrackedObjects)
            {
                ImageAnnotator.DrawTrack(image, track);
            }

            // update image source
            MainImageSource = BitmapSourceConvert.ToBitmapSource(image);
        }

        private void SubmitIdRequests()
        {
            if (_identificationService == null) return;
            foreach (var track in CurrentTrackedObjects.Where(t => !(t.CurrentFrame.IdRequested)))
            {
                track.CurrentFrame.IdRequested = true;
                _identificationService.Identify(track);
            }
        }

        public void Start()
        {
            if (VideoSource == null) return;
            VideoSource.Start();
        }

        public void Stop()
        {
            if (VideoSource == null) return;
            VideoSource.Stop();            
        }

        public void Reset()
        {
            if (VideoSource == null) return;
            VideoSource.Reset();
            CurrentTrackedObjects.Clear();
            ArchivedObjects.Clear();
            MainImageSource = null;
        }

        public void Frame()
        {
            if (VideoSource == null) return;
            VideoSource.Frame();
        }

        public void Dispose()
        {
            CloseCurrentProfile();
        }

        private void CloseCurrentProfile()
        {
            if (VideoSource == null) return;
            VideoSource.NewFrame -= VideoSourceOnNewFrame;
            VideoSource.Dispose();
            VideoSource = null;
            _objectTracker.Dispose();
            _objectTracker = null;
            _identificationService.Dispose();
            _identificationService = null;
        }
    }
}
