using System;
using BirdTracker.Core.Model;

namespace BirdTracker.Service
{
    public static class VideoSourceFactory
    {
        public static IVideoSource Create(Profile profile)
        {
            if (profile.UseFile) return new FileVideoSource(profile);
            return new LiveVideoSource();
        }
    }
}
