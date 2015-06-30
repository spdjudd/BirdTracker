using BirdTracker.Core.Model;
using BirdTracker.Core.Utilities;

namespace BirdTracker.ViewModel
{
    public class TrackDetailViewModel : Bindable
    {
        public TrackedObject TrackedObject { get; set; }

        private TrackedObjectFrame _selectedFrame;
        public TrackedObjectFrame SelectedFrame
        {
            get { return _selectedFrame; }
            set
            {
                Set(ref _selectedFrame, value);
            }
        }
    }
}
