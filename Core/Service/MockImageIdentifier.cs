using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BirdTracker.Core.Interfaces;
using BirdTracker.Core.Model;
using Emgu.CV.Structure;

namespace BirdTracker.Core.Service
{
    public class MockImageIdentifier : IImageIdentifier
    {
        private static IList<IdScore> GetResult()
        {
            return new List<IdScore>
            {
                new IdScore("Longer id name", -0.001d),
                new IdScore("Test2", -0.34d),
                new IdScore("Test3", -3.79)
            };
        }

        public async Task<IList<IdScore>> IdentifyAsync(Emgu.CV.Image<Bgr, Byte> image)
        {
            await Task.Delay(100);
            return GetResult();
        }
    }
}
