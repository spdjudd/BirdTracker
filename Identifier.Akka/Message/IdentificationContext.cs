using BirdTracker.Core.Model;

namespace Identifier.Akka.Message
{
    public class IdentificationContext
    {
        public IdentificationContext(TrackedObject trackedObject)
        {
            TrackedObject = trackedObject;
            Frame = trackedObject.CurrentFrame;
        }
        public TrackedObject TrackedObject { get; private set; }
        public TrackedObjectFrame Frame { get; private set; }
    }
}
