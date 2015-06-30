using System;
using Emgu.CV;
using Emgu.CV.Structure;

namespace BirdTracker.Service
{
    interface IObjectTracker : IDisposable
    {
        void Update(Image<Bgr, Byte> image);
    }
}
