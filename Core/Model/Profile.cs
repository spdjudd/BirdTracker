
using BirdTracker.Core.Utilities;

namespace BirdTracker.Core.Model
{
    public class Profile : Bindable
    {
        private string _name = "Default";
        public string Name
        {
            get { return _name;}
            set 
            { 
                Set(ref _name, value);
            }
        }

        private bool _useFile = true;
        public bool UseFile
        {
            get { return _useFile; }
            set
            {
                Set(ref _useFile, value);
            }
        }

        private string _filePath = "c:\\wkspc\\ml\\birds\\feeder.mp4";
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                Set(ref _filePath, value);
            }
        }

        private int _frameSpacingMs = 40;

        public int FrameSpacingMs
        {
            get { return _frameSpacingMs; }
            set { Set(ref _frameSpacingMs, value);  }
        }

        private bool _backgroundId;
        public bool BackgroundId
        {
            get { return _backgroundId;}
            set
            {
                Set(ref _backgroundId, value);
            }
        }

        private TrackerSettings _trackerSettings = new TrackerSettings();
        public TrackerSettings TrackerSettings
        {
            get { return _trackerSettings; }
            set { _trackerSettings = value; }
        }

        private IdentifierSettings _identifierSettings = new IdentifierSettings();
        public IdentifierSettings IdentifierSettings
        {
            get { return _identifierSettings; }
            set { _identifierSettings = value; }
        }
    }
}
