using System.Runtime.Serialization;

namespace BTrees.Nodes
{
    public sealed class LockFailedException
        : Exception
    {
        public LockFailedException()
        {
        }

        public LockFailedException(string? message) : base(message)
        {
        }

        public LockFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public LockFailedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
