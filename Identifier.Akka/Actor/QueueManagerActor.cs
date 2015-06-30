using System.Collections.Generic;
using Akka.Actor;
using Identifier.Akka.Message;

namespace Identifier.Akka.Actor
{
    public class QueueManagerActor : ReceiveActor
    {
        private readonly Queue<uint> _objectQueue;
        private readonly Dictionary<uint, IdentificationContext> _latestContexts;
        private readonly Queue<IActorRef> _freeIdentifiers;

        public QueueManagerActor()
        {
            _objectQueue = new Queue<uint>();
            _latestContexts = new Dictionary<uint, IdentificationContext>();
            _freeIdentifiers = new Queue<IActorRef>();
            Receive<IdentificationContext>(ic => Identify(ic));
            Receive<IdentifierStatus>(s => UpdateStatus());
        }

        private void Identify(IdentificationContext context)
        {
            if (!_latestContexts.ContainsKey(context.TrackedObject.Id))
            {
                // we don't have a pending request for this object - add it to the q
                _objectQueue.Enqueue(context.TrackedObject.Id);
            }
            // update the context for this object
            _latestContexts[context.TrackedObject.Id] = context;
            TrySend();
        }

        private void UpdateStatus()
        {
            // identifier is free
            if (_objectQueue.Count == 0)
            {
                // nothing to send, queue the identifier
                _freeIdentifiers.Enqueue(Sender);
                return;
            }
            SendIdentification(Sender);
        }

        private void TrySend()
        {
            // see if there's a free identifier and send
            if (_freeIdentifiers.Count == 0) return;
            SendIdentification(_freeIdentifiers.Dequeue());
        }

        private void SendIdentification(IActorRef identifier)
        {
            var item = _objectQueue.Dequeue();
            var id = _latestContexts[item];
            _latestContexts.Remove(item);
            identifier.Tell(id);
        }
    }
}
