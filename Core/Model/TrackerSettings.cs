
namespace BirdTracker.Core.Model
{
    public class TrackerSettings
    {
        public bool ArchiveTrackedObjects { get; set; }

        private RangeValue<double> _distanceThreshold = new RangeValue<double>{Default = 100.0, Max = 500.0, Min = 0.0, Value = 100.0};
        public RangeValue<double> DistanceThreshold
        {
            get { return _distanceThreshold; }
            set { _distanceThreshold = value; }
        }

        private RangeValue<uint> _maxInactiveFrames = new RangeValue<uint> {Default = 10, Min = 0, Max = 1000, Value = 10};
        public RangeValue<uint> MaxInactiveFrames
        {
            get { return _maxInactiveFrames; }
            set { _maxInactiveFrames = value; }
        }

        private RangeValue<uint> _activityThreshold = new RangeValue<uint> {Default = 25, Min = 0, Max = 1000, Value = 25};
        public RangeValue<uint> ActivityThreshold
        {
            get { return _activityThreshold;}
            set { _activityThreshold = value; }
        }

        private RangeValue<int> _minFramesToStartIdentifying = new RangeValue<int> {Default = 5, Min = 0, Max = 1000, Value=5};
        public RangeValue<int> MinFramesToStartIdentifying
        {
            get { return _minFramesToStartIdentifying; }
            set { _minFramesToStartIdentifying = value; }
        }

        private RangeValue<int> _minSizeToIdentify = new RangeValue<int> { Default = 30, Min = 0, Max = 1000, Value = 30};
        public RangeValue<int> MinSizeToIdentify
        {
            get { return _minSizeToIdentify; }
            set { _minSizeToIdentify = value; }
        }

        private RangeValue<int> _subImageMultiplier = new RangeValue<int> { Default = 50, Min = 0, Max = 1000, Value = 50};
        public RangeValue<int> SubImageMultiplier
        {
            get { return _subImageMultiplier; }
            set { _subImageMultiplier = value; }
        }

        // non-volatile
        private RangeValue<int> _backgroundSubtractorHistory = new RangeValue<int> { Default = 5000, Min = 0, Max = 100000, Value = 5000};
        public RangeValue<int> BackgroundSubtractorHistory
        {
            get { return _backgroundSubtractorHistory; }
            set { _backgroundSubtractorHistory = value; }
        }

        // non-volatile
        private RangeValue<float> _backgroundSubtractorMaxComponents = new RangeValue<float> { Default = 15f, Min = 0f, Max = 250f, Value = 15f };
        public RangeValue<float> BackgroundSubtractorMaxComponents
        {
            get { return _backgroundSubtractorMaxComponents; }
            set { _backgroundSubtractorMaxComponents = value; }
        }
    }
}
