using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Aadev.JTF;

[TypeConverter("Aadev.JTF.Design.JtIdentifierConverter, Aadev.JTF.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4bb879fd89b07a65")]
[DebuggerDisplay("{Value}")]
public readonly struct JtIdentifier : IEquatable<JtIdentifier>
{
    internal static readonly JtIdentifier Empty = new JtIdentifier(null);

    public readonly string? Value { get; }

    public JtIdentifier(string? identifier)
    {
        Value = identifier?.ToLowerInvariant();
    }
#if NET5_0_OR_GREATER
    [MemberNotNullWhen(false, nameof(Value))]
#endif
    public readonly bool IsEmpty => string.IsNullOrEmpty(Value);

    public static implicit operator JtIdentifier(string? identifier) => new JtIdentifier(identifier);
    public static implicit operator string?(JtIdentifier identifier) => identifier.Value;


    public static bool operator ==(JtIdentifier left, JtIdentifier right) => left.Equals(right);
    public static bool operator !=(JtIdentifier left, JtIdentifier right) => !(left == right);

    public override readonly bool Equals(object? obj) => obj is JtIdentifier identifier && Equals(identifier);
    public readonly bool Equals(JtIdentifier other) => Value == other.Value;
    public override readonly int GetHashCode() => HashCode.Combine(Value);
    public override readonly string? ToString() => Value;

    public static JtIdentifier FromString(string? identifier) => new JtIdentifier(identifier);
}
