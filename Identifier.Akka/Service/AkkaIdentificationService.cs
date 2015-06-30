using System;
using System.Collections.Generic;
using Akka.Actor;
using BirdTracker.Core.Interfaces;
using BirdTracker.Core.Model;
using Common.Logging;
using Identifier.Akka.Actor;
using Identifier.Akka.Message;

namespace Identifier.Akka.Service
{
    public class AkkaIdentificationService : IIdentificationService
    {
        private static readonly ILog Log = LogManager.GetLogger<AkkaIdentificationService>();
        private static readonly TimeSpan StopTimeout = TimeSpan.FromSeconds(1);
        private readonly ActorSystem _actorSystem;
        private readonly IActorRef _queueManagerActor;
        private readonly List<IActorRef> _identifiers;
        private readonly IActorRef _resultsProcessorActor;

        public AkkaIdentificationService(IdentifierSettings settings, IImageIdentifier imageIdentifier)
        {
            Log.Info("Creating actor system");
            _actorSystem = ActorSystem.Create("IdentificationSystem");
            _resultsProcessorActor = _actorSystem.ActorOf<ResultProcessorActor>("ResultProcessor");
            _queueManagerActor = _actorSystem.ActorOf<QueueManagerActor>("QueueManager");
            _identifiers = new List<IActorRef>();
            Log.InfoFormat("Creating {0} IdentifyActors", settings.NumThreads);
            for (var i = 0; i < settings.NumThreads; ++i)
            {
                var identifier = _actorSystem.ActorOf<IdentifyActor>(string.Format("Identifier_{0}", i));
                _identifiers.Add(identifier);
                identifier.Tell(imageIdentifier);
            }
        }

        public void Identify(TrackedObject trackedObject)
        {
            _queueManagerActor.Tell(new IdentificationContext(trackedObject));                
        }

        public void Dispose()
        {
            Log.Info("Dispose");
            // stop actors
            foreach (var identifier in _identifiers)
            {
                StopActor(identifier);
            }
            StopActor(_queueManagerActor);
            StopActor(_resultsProcessorActor);

            // dispose the system
            _actorSystem.Dispose();
        }

        private void StopActor(IActorRef actor)
        {
            var result = actor.GracefulStop(StopTimeout).Result;
            if (!result)
            {
                Log.WarnFormat("Failed to stop {0} gracefully", actor.Path.Name);
            }
        }
    }
}
