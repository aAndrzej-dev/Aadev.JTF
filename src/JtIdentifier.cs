using System;
using System.Diagnostics.CodeAnalysis;

namespace Aadev.JTF
{
    public readonly struct JtIdentifier : IEquatable<JtIdentifier>
    {
        internal static readonly JtIdentifier Empty = new JtIdentifier(null);

        public readonly string? Identifier { get; }

        public JtIdentifier(string? identifier)
        {
            Identifier = identifier?.ToLowerInvariant();
        }
#if NET5_0_OR_GREATER
        [MemberNotNullWhen(false, "Identifier")]
#endif
        public readonly bool IsEmpty => string.IsNullOrEmpty(Identifier);

        public static implicit operator JtIdentifier(string? identifier) => new JtIdentifier(identifier);
        public static implicit operator string?(JtIdentifier identifier) => identifier.Identifier;


        public static bool operator ==(JtIdentifier left, JtIdentifier right) => left.Equals(right);
        public static bool operator !=(JtIdentifier left, JtIdentifier right) => !(left == right);

        public override readonly bool Equals(object? obj) => obj is JtIdentifier identifier && Equals(identifier);
        public readonly bool Equals(JtIdentifier other) => Identifier == other.Identifier;
        public override readonly int GetHashCode() => HashCode.Combine(Identifier);
        public override readonly string? ToString() => Identifier;

        public static JtIdentifier FromString(string? identifier) => new JtIdentifier(identifier);
    }
}
