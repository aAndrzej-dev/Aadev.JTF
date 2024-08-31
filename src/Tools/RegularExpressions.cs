using System.Text.RegularExpressions;

namespace Aadev.JTF.Tools;
internal partial class RegularExpressions
{
#if NET7_0_OR_GREATER
    [GeneratedRegex("^[a-z]+[a-z0-9_]*$", RegexOptions.Compiled)]
    private static partial Regex IdentifierRegex();

    public static readonly Regex identifierRegex = IdentifierRegex();

#else
    public static readonly Regex identifierRegex = new Regex("^[a-z]+[a-z0-9_]*$", RegexOptions.Compiled);
#endif

}
