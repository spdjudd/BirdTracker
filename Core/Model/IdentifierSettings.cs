
using BirdTracker.Core.Utilities;

namespace BirdTracker.Core.Model
{
    public class IdentifierSettings : Bindable
    {
        private IdServiceType _idServiceType;
        public IdServiceType IdServiceType
        {
            get { return _idServiceType; }
            set
            {
                Set(ref _idServiceType, value);
            }
        }

        private IdentifierType _identifierType;
        public IdentifierType IdentifierType
        {
            get { return _identifierType; }
            set
            {
                Set(ref _identifierType, value);
            }
        }

        private string _identifierUrl = "http://192.168.1.2:8888/idbird";
        //private string _identifierUrl = "http://192.168.137.129:8888/idbird";
        public string IdentifierUrl
        {
            get { return _identifierUrl; }
            set
            {
                Set(ref _identifierUrl, value);
            }
        }

        private int _numThreads = 2;
        public int NumThreads
        {
            get { return _numThreads; }
            set
            {
                Set(ref _numThreads, value);
            }
        }

        private int _identifierImageSize = 50;
        public int IdentifierImageSize
        {
            get { return _identifierImageSize; }
            set
            {
                Set(ref _identifierImageSize, value);
            }
        }

        // todo: global image normalisation
    }

    public enum IdServiceType
    {
        Akka,
        Classic
    }

    public enum IdentifierType
    {
        Torch,
        Dummy
    }
}
