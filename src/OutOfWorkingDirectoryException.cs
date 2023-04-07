using System;

namespace Aadev.JTF
{
    public sealed class OutOfWorkingDirectoryException : Exception
    {
        public OutOfWorkingDirectoryException(string message) : base(message)
        {
        }

        public OutOfWorkingDirectoryException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public OutOfWorkingDirectoryException() : base("File is outside of working directory")
        {
        }
    }
}
