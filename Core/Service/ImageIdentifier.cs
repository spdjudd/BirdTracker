using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BirdTracker.Core.Interfaces;
using BirdTracker.Core.Model;
using Common.Logging;
using Emgu.CV;
using Emgu.CV.Structure;
using Newtonsoft.Json;

namespace BirdTracker.Core.Service
{
    public class ImageIdentifier : IImageIdentifier
    {
        private static readonly ILog Log = LogManager.GetLogger<ImageIdentifier>();

        private readonly IdentifierSettings _settings;
        
        public ImageIdentifier(IdentifierSettings settings)
        {
            _settings = settings;
        }

        public async Task<IList<IdScore>> IdentifyAsync(Image<Bgr, Byte> image)
        {
            var reducedImage = image.Resize(_settings.IdentifierImageSize, _settings.IdentifierImageSize, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
            var encoded = Convert.ToBase64String(reducedImage.Bytes);

            encoded = WebUtility.UrlEncode(encoded);
            var data = "img=" + encoded;

            using (var wc = new HttpClient())
            {
                try
                {
                    var json = await wc.PostAsync(_settings.IdentifierUrl, new StringContent(data, new UTF8Encoding(), "application/x-www-form-urlencoded"));
                    var result = JsonConvert.DeserializeObject<List<IdentifyResult>>(await json.Content.ReadAsStringAsync());
                    return result.Select(ir => new IdScore{Id = ir.result, Score = ir.p}).ToList();
                }
                catch (Exception ex)
                {
                    Log.Error("Exception calling identifier: ", ex);
                }
            }
            return null;  
        }
    }

    /// <summary>
    /// Part of external interface of this identifier implementation
    /// </summary>
    internal class IdentifyResult
    {
        // ReSharper disable once InconsistentNaming
        public double p;
        // ReSharper disable once InconsistentNaming
        public string result;
    }
}
