using System;
using System.ComponentModel;
using Emgu.CV;
using Emgu.CV.Structure;

namespace BirdTracker.Service
{
    public interface IVideoSource : IDisposable, INotifyPropertyChanged
    {
        event EventHandler<ImageEventArgs> NewFrame;
        void Start();
        void Stop();
        void Frame();
        void Reset();
        VideoState VideoState { get; }
    }

    public class ImageEventArgs : EventArgs
    {
        public Image<Bgr, Byte> Image { get; set; }
    }

    public enum VideoState
    {
        Unknown,
        Running,
        Stopped
    }
}
