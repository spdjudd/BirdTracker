using System.Collections.Generic;
using BirdTracker.Core.Model;

namespace Identifier.Akka.Message
{
    public class IdentificationResult
    {
        public IdentificationResult(IdentificationContext context, IList<IdScore> result)
        {
            Context = context;
            Result = result;
        }

        public IdentificationContext Context { get; private set; }
        public IList<IdScore> Result { get; private set; } 
    }
}
