using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Aadev.JTF
{
    public interface IJtFile
    {
        int Version { get; }
        JtFileType Type { get; }
    }

    public sealed class JtFileType
    {
        private JtFileType(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
        }

        public string Name { get; }
        public string DisplayName { get; }

        public static readonly JtFileType Template = new JtFileType("main", nameof(Template));
        public static readonly JtFileType CustomSource = new JtFileType("customsource", nameof(CustomSource));
        public static readonly JtFileType CustomValueDictionary = new JtFileType("valuesdictionary", nameof(CustomValueDictionary));
        [DebuggerStepThrough]
        public bool IsValidType([NotNullWhen(true)] string? type) => type != null && Name.Equals(type, System.StringComparison.OrdinalIgnoreCase);
        [DebuggerStepThrough]
        public void ThorwIfInvalidType([NotNull]string? type, string filename)
        {
            if (!IsValidType(type))
                throw new JtfException($"Invalid jtf file type: '{type}' (required: '{Name}') in {filename}; ");
        }
    }
}
