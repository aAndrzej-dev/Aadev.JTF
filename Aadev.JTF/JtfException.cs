using System;

namespace Aadev.JTF;

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
    public JtfException(string? message, IJtFile? jtfFile) : base(message)
    {
        JTFFile = jtfFile;
    }
    public JtfException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
    public JtfException(string? message, Exception? innerException, IJtFile? jtfFile) : base(message, innerException)
    {
        JTFFile = jtfFile;
    }

    public IJtFile? JTFFile { get; }
}
