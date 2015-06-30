
namespace Identifier.Akka.Message
{
    public class IdentifierStatus
    {
        public IdentifierStatus(string path, int queueLength)
        {
            Path = path;
            QueueLength = queueLength;
        }
        public string Path { get; private set; }
        public int QueueLength { get; private set; }
    }
}
