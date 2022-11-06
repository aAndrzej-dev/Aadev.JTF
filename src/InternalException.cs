using System;
using System.Runtime.Serialization;

namespace Aadev.JTF
{
    [Serializable]
    internal sealed class InternalException : Exception
    {
        public InternalException() : base("Internal error")
        {
        }

        public InternalException(string? message) : base(message)
        {
        }

        public InternalException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}