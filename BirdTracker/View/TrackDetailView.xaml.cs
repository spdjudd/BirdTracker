using System.Windows;
using BirdTracker.Core.Model;
using BirdTracker.ViewModel;
using MahApps.Metro.Controls;

namespace BirdTracker.View
{
    /// <summary>
    /// Interaction logic for TrackDetailView.xaml
    /// </summary>
    public partial class TrackDetailView : MetroWindow
    {
        public TrackDetailView(TrackedObject trackedObject)
        {
            InitializeComponent();
            DataContext = new TrackDetailViewModel {TrackedObject = trackedObject};
        }
    }
}
