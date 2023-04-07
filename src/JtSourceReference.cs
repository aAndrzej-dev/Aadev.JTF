using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Aadev.JTF
{
    [DebuggerDisplay("{this.ToString()}")]
    public readonly struct JtSourceReference : IEquatable<JtSourceReference>
    {
        public readonly JtIdentifier Identifier { get; }
        public readonly bool IsEmpty => Type is JtSourceReferenceType.None || Identifier.IsEmpty;
        public readonly JtSourceReferenceType Type { get; }
        public static JtSourceReference Empty => new JtSourceReference(null);

        public JtSourceReference(string? identifier)
        {
            if (identifier is null || string.IsNullOrWhiteSpace(identifier))
            {
                Type = JtSourceReferenceType.None;
                Identifier = null;
            }
            else if (identifier.StartsWith('@'))
            {
                Type = JtSourceReferenceType.External;
                Identifier = identifier[1..];
            }
            else if (identifier.StartsWith('$'))
            {
                Type = JtSourceReferenceType.Dynamic;
                Identifier = identifier[1..];
            }
            else if (identifier.StartsWith('#'))
            {
                Type = JtSourceReferenceType.Direct;
                Identifier = identifier[1..];
            }
            else
            {
                Identifier = identifier;
                Type = JtSourceReferenceType.Local;
            }
        }

        public JtSourceReference(JtIdentifier identifier, JtSourceReferenceType type)
        {
            Identifier = identifier;
            Type = type;
        }

        public static implicit operator JtSourceReference(string? identifier) => new JtSourceReference(identifier);

        public static bool operator ==(JtSourceReference left, JtSourceReference right) => left.Equals(right);
        public static bool operator !=(JtSourceReference left, JtSourceReference right) => !(left == right);

        public override readonly bool Equals(object? obj) => obj is JtSourceReference identifier && Equals(identifier);
        public readonly bool Equals(JtSourceReference other) => Identifier == other.Identifier && Type == other.Type;
        public override readonly int GetHashCode() => HashCode.Combine(Identifier, Type);

        public override readonly string? ToString()
        {
            return Type switch
            {
                JtSourceReferenceType.Local => Identifier.ToString(),
                JtSourceReferenceType.Dynamic => $"${Identifier}",
                JtSourceReferenceType.External => $"@{Identifier}",
                JtSourceReferenceType.Direct => $"#{Identifier}",
                _ => null,
            };
        }

        public static JtSourceReference FromString(string? identifier) => new JtSourceReference(identifier);
    }
    public enum JtSourceReferenceType
    {
        None,
        Local,
        Dynamic,
        External,
        Direct
    }
}
