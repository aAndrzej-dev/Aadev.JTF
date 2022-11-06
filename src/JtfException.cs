using System;

namespace Aadev.JTF
{
    /// <summary>
    /// JTF format exception
    /// </summary>
    public sealed class JtfException : Exception
    {
        public JtfException()
        {
        }

        public JtfException(string? message) : base(message)
        {
        }

        public JtfException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
