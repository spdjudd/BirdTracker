
using BirdTracker.Core.Utilities;

namespace BirdTracker.Core.Model
{
    public class RangeValue<T> : Bindable
    {
        // static properties
        public T Min { get; set; }
        public T Max { get; set; }
        public T Default { get; set; }

        // variable
        private T _value;

        public T Value
        {
            get { return _value;}
            set
            {
                Set(ref _value, value);
            }
        }
    }
}
