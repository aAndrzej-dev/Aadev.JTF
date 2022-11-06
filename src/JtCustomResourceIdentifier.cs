using System;

namespace Aadev.JTF
{
    public readonly struct JtCustomResourceIdentifier : IEquatable<JtCustomResourceIdentifier>
    {
        public readonly JtIdentifier Identifier { get; }
        public readonly bool IsEmpty => Type is JtCustomResourceIdentifierType.None || Identifier.IsEmpty;
        public readonly JtCustomResourceIdentifierType Type { get; }
        public static JtCustomResourceIdentifier Empty => new JtCustomResourceIdentifier(string.Empty);

        public JtCustomResourceIdentifier(string? identifier)
        {
            if (identifier is null || string.IsNullOrWhiteSpace(identifier))
            {
                Type = JtCustomResourceIdentifierType.None;
                Identifier = null;
            }
            else if (identifier.StartsWith('@'))
            {
                Type = JtCustomResourceIdentifierType.External;
                Identifier = identifier[1..];
                ;
            }
            else if (identifier.StartsWith('$'))
            {
                Type = JtCustomResourceIdentifierType.Dynamic;
                Identifier = identifier[1..];
            }
            else if (identifier.StartsWith('#'))
            {
                Type = JtCustomResourceIdentifierType.Direct;
                Identifier = identifier[1..];
            }
            else
            {
                Identifier = identifier;
                Type = JtCustomResourceIdentifierType.Local;
            }
        }

        public static implicit operator JtCustomResourceIdentifier(string? identifier) => new JtCustomResourceIdentifier(identifier);

        public static bool operator ==(JtCustomResourceIdentifier left, JtCustomResourceIdentifier right) => left.Equals(right);
        public static bool operator !=(JtCustomResourceIdentifier left, JtCustomResourceIdentifier right) => !(left == right);

        public readonly override bool Equals(object? obj) => obj is JtCustomResourceIdentifier identifier && Equals(identifier);
        public readonly bool Equals(JtCustomResourceIdentifier other) => Identifier == other.Identifier && Type == other.Type;
        public readonly override int GetHashCode() => HashCode.Combine(Identifier, Type);

        public readonly override string? ToString()
        {
            return Type switch
            {
                JtCustomResourceIdentifierType.Local => Identifier.ToString(),
                JtCustomResourceIdentifierType.Dynamic => $"${Identifier}",
                JtCustomResourceIdentifierType.External => $"@{Identifier}",
                JtCustomResourceIdentifierType.Direct => $"#{Identifier}",
                _ => null,
            };
        }

        public static JtCustomResourceIdentifier FromString(string? identifier) => new JtCustomResourceIdentifier(identifier);
    }
    public enum JtCustomResourceIdentifierType
    {
        None,
        Local,
        Dynamic,
        External,
        Direct
    }
}
