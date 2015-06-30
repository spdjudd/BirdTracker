using System;
using System.Threading.Tasks;
using Akka.Actor;
using BirdTracker.Core.Interfaces;
using BirdTracker.Core.Utilities;
using Identifier.Akka.Message;

namespace Identifier.Akka.Actor
{
    public class IdentifyActor : ReceiveActor
    {
        private readonly IActorRef _queueManager;
        private readonly IActorRef _resultProcessor;
        private IImageIdentifier _imageIdentifier;

        public IdentifyActor()
        {
            _queueManager = Context.ActorSelection("akka://IdentificationSystem/user/QueueManager").ResolveOne(TimeSpan.FromSeconds(1)).Result;
            _resultProcessor = Context.ActorSelection("akka://IdentificationSystem/user/ResultProcessor").ResolveOne(TimeSpan.FromSeconds(1)).Result;
            Receive<IdentificationContext>(ic => Identify(ic));
            Receive<IImageIdentifier>(s => Configure(s));
            SendStatus();
        }

        private void Configure(IImageIdentifier identifier)
        {
            _imageIdentifier = identifier;
        }

        private async Task Identify(IdentificationContext context)
        {
            using (new ScopeLog("IdentifyActor.Identify"))
            {
                var result = await _imageIdentifier.IdentifyAsync(context.Frame.RawImage);
                _resultProcessor.Tell(new IdentificationResult(context, result));
                SendStatus();                
            }
        }

        private void SendStatus()
        {
            _queueManager.Tell(new IdentifierStatus(Self.Path.Name, 0));
        }
    }
}
