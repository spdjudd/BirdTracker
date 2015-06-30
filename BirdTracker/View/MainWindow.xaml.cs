using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BirdTracker.Core.Model;
using BirdTracker.ViewModel;
using MahApps.Metro.Controls;

namespace BirdTracker.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            Closed += OnClosed;
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }

        private void OnClosed(object sender, EventArgs eventArgs)
        {
            _viewModel.StopCommand.Execute(null);
            _viewModel.Dispose();
        }

        private void TrackedObjectListOnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TrackedObjectListOnGotFocus(sender, null);
            _viewModel.OpenDetailCommand.Execute(null);
        }

        private void TrackedObjectListOnGotFocus(object sender, RoutedEventArgs e)
        {
            var listView = sender as ListView;
            if (listView == null) return;
            var selectedItem = listView.SelectedItem as TrackedObject;
            _viewModel.Model.SelectedObject = selectedItem;
        }
    }
}
