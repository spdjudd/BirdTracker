using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media;
using BirdTracker.Core.Utilities;
using Emgu.CV;
using Emgu.CV.Structure;

namespace BirdTracker.Core.Model
{
    public class TrackedObjectFrame : Bindable
    {
        public IList<IdScore> IdResults { get; set; }

        private string _identity;

        public string Identity
        {
            get { return _identity; }
            set
            {
                Set(ref _identity, value);
            }
        }

        private ImageSource _image;
        public ImageSource Image
        {
            get { return _image; }
            set
            {
                Set(ref _image, value);
            }
        }

        public Image<Bgr, Byte> RawImage { get; set; }

        public bool IdRequested { get; set; }

        public Rectangle SourceRectangle { get; set; }
    }
}
