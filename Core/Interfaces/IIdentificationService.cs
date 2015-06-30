using System;
using BirdTracker.Core.Model;

namespace BirdTracker.Core.Interfaces
{
    public interface IIdentificationService : IDisposable
    {
        void Identify(TrackedObject trackedObject);
    }
}
