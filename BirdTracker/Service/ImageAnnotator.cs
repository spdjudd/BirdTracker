using System;
using Emgu.CV;
using Emgu.CV.Structure;
using BirdTracker.Core.Model;

namespace BirdTracker.Service
{
    public static class ImageAnnotator
    {
        private static MCvFont _font = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_PLAIN, 1.0, 1.0);
        private static readonly Bgr White = new Bgr(255.0, 255.0, 255.0);

        public static void DrawTrack(Image<Bgr, Byte> image, TrackedObject trackedObject)
        {
            image.Draw(trackedObject.CurrentFrame.SourceRectangle, White, 1);
            if (string.IsNullOrEmpty(trackedObject.Identity)) return;
            image.Draw(trackedObject.Identity, ref _font, trackedObject.CurrentFrame.SourceRectangle.Location, White);
        }
    }
}
