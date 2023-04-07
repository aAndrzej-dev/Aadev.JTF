using System;

namespace Aadev.JTF
{
    public sealed class ReadOnlyException : Exception
    {
        public ReadOnlyException(string message) : base(message)
        {
        }

        public ReadOnlyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ReadOnlyException()
        {
        }
    }
}
