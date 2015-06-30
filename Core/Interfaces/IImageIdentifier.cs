using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BirdTracker.Core.Model;
using Emgu.CV;
using Emgu.CV.Structure;

namespace BirdTracker.Core.Interfaces
{
    public interface IImageIdentifier
    {
        Task<IList<IdScore>> IdentifyAsync(Image<Bgr, Byte> image);
    }
}
