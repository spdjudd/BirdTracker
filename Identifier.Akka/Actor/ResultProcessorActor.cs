using Akka.Actor;
using Identifier.Akka.Message;

namespace Identifier.Akka.Actor
{
    /// <summary>
    /// Probably not needed
    /// </summary>
    public class ResultProcessorActor : ReceiveActor
    {
        public ResultProcessorActor()
        {
            Receive<IdentificationResult>(r => ProcessResult(r));
        }

        private void ProcessResult(IdentificationResult result)
        {
            var frame = result.Context.Frame;
            frame.IdResults = result.Result;
            result.Context.TrackedObject.ProcessIdResult(frame);
        }
    }
}
