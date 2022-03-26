using System;

namespace Aadev.JTF
{
    [Serializable]
    public class InvalidJtfFileTypeException : Exception
    {
        public InvalidJtfFileTypeException(string filename, string requredType, string? invalidType) : base($"File {filename} is {invalidType} insted of {requredType}!") { }
    }
}