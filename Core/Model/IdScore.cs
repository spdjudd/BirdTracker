
using BirdTracker.Core.Utilities;

namespace BirdTracker.Core.Model
{
    public class IdScore : Bindable
    {
        public IdScore() { }

        public IdScore(string id, double score)
        {
            _id = id;
            _score = score;
        }

        private string _id;
        public string Id
        {
            get { return _id; }
            set
            {
                Set(ref _id, value);
            }
        }

        private double _score;
        public double Score
        {
            get { return _score; }
            set
            {
                Set(ref _score, value);
            }
        }
    }
}
