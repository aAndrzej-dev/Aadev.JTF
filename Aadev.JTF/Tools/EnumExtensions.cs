using System;

namespace Aadev.JTF.Tools;
internal static class EnumExtensions
{
    public static string ToLowerString(this JtContainerType containerType)
    {
        return containerType switch
        {
            JtContainerType.Array => "array",
            JtContainerType.Block => "block",
            _ => throw new ArgumentOutOfRangeException(nameof(containerType)),
        };
    }
}
