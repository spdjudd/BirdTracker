using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using BirdTracker.Command;
using BirdTracker.Core.Model;
using BirdTracker.Model;
using BirdTracker.Service;
using BirdTracker.View;
using Common.Logging;

namespace BirdTracker.ViewModel
{
    public class MainViewModel : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger<MainViewModel>();

        public MainViewModel()
        {
            LogAppender = NotifyingLogAppender.Appender;
            Model = new MainModel();
            var defaultProfile = TryOpen("Default") ?? new Profile();
            Model.LoadProfile(defaultProfile);
        }

        public MainModel Model { get; private set; }

        public NotifyingLogAppender LogAppender { get; set; }

        private bool CanStart(object ignore)
        {
            return Model.VideoSource.VideoState == VideoState.Stopped;
        }

        private void Start()
        {
            Model.Start();
            _startCommand.RaiseCanExecuteChanged();
            _stopCommand.RaiseCanExecuteChanged();
            Log.Info("Started video");
        }

        private bool CanStop(object ignore)
        {
            return Model.VideoSource.VideoState == VideoState.Running;
        }

        private void Stop()
        {
            Model.Stop();
            _startCommand.RaiseCanExecuteChanged();
            _stopCommand.RaiseCanExecuteChanged();
            Log.Info("Stopped video");
        }

        private bool CanFrame(object ignore)
        {
            return Model.VideoSource.VideoState != VideoState.Unknown;
        }

        private void Frame()
        {
            Model.Frame();
            _startCommand.RaiseCanExecuteChanged();
            _stopCommand.RaiseCanExecuteChanged();
            Log.Info("Captured video frame");
        }

        private void Reset()
        {
            Model.Reset();
            _startCommand.RaiseCanExecuteChanged();
            _stopCommand.RaiseCanExecuteChanged();
            Log.Info("Reset video");
        }

        private Profile TryOpen(string profileName)
        {
            var filePath = Path.GetFullPath(string.Format("{0}.xml", profileName));
            if (!File.Exists(filePath)) return null;
            var serializer = new XmlSerializer(typeof(Profile));
            var profile = (Profile)serializer.Deserialize(File.OpenRead(filePath));
            return profile;
        }

        private void Open()
        {
            try
            {
                var profile = TryOpen(Model.Profile.Name);
                if (profile == null) throw new FileNotFoundException();
                Model.LoadProfile(profile);
                Log.InfoFormat("Opened {0}", Model.Profile.Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Failed to open {0} [{1}]", Model.Profile.Name, ex.Message));
            }
        }

        private void Save()
        {
            var filePath = Path.GetFullPath(string.Format("{0}.xml", Model.Profile.Name));
            try
            {
                var serializer = new XmlSerializer(typeof(Profile));
                serializer.Serialize(File.OpenWrite(filePath), Model.Profile);
                Log.InfoFormat("Saved {0}", Model.Profile.Name);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error saving {0}", ex,  Model.Profile.Name);
                MessageBox.Show("Failed to save {0}", filePath);
            }
        }

        private void Reload()
        {
            Stop();
            Model.LoadProfile(Model.Profile);
        }

        private void OpenDetail()
        {
            if (Model.SelectedObject == null) return;
            var detailWindow = new TrackDetailView(Model.SelectedObject);
            detailWindow.Show();
        }

        private RelayCommand _startCommand;
        public ICommand StartCommand
        {
            get { return _startCommand ?? (_startCommand = new RelayCommand(Start, CanStart)); }
        }

        private RelayCommand _stopCommand;
        public ICommand StopCommand
        {
            get { return _stopCommand ?? (_stopCommand = new RelayCommand(Stop, CanStop)); }
        }

        private RelayCommand _frameCommand;
        public ICommand FrameCommand
        {
            get { return _frameCommand ?? (_frameCommand = new RelayCommand(Frame, CanFrame)); }
        }

        private RelayCommand _resetCommand;
        public ICommand ResetCommand
        {
            get { return _resetCommand ?? (_resetCommand = new RelayCommand(Reset)); }
        }

        private RelayCommand _openCommand;
        public ICommand OpenCommand
        {
            get { return _openCommand ?? (_openCommand = new RelayCommand(Open)); }
        }

        private RelayCommand _saveCommand;
        public ICommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new RelayCommand(Save)); }
        }

        private RelayCommand _reloadCommand;
        public ICommand ReloadCommand
        {
            get { return _reloadCommand ?? (_reloadCommand = new RelayCommand(Reload)); }
        }

        private RelayCommand _openDetailCommand;
        public ICommand OpenDetailCommand
        {
            get { return _openDetailCommand ?? (_openDetailCommand = new RelayCommand(OpenDetail)); }
        }

        public void Dispose()
        {
            Model.Dispose();
        }
    }
}
