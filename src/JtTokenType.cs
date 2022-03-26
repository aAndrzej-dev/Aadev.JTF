using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System;

namespace Aadev.JTF
{
    public class JtTokenType : IEquatable<JtTokenType?>
    {
        private JtTokenType(int id, string name, Func<JObject, JTemplate, JtToken> instanceFactory, string displayName)
        {
            Id = id;
            Name = name;
            InstanceFactory = instanceFactory;
            DisplayName = displayName;
        }

        public int Id { get; }
        public string Name { get; }
        public Func<JObject, JTemplate, JtToken> InstanceFactory { get; }
        public string DisplayName { get; }

        public static readonly JtTokenType Unknown = new JtTokenType(0, "unknown", (o, t) => new JtUnknown(o, t), nameof(Unknown));
        public static readonly JtTokenType Bool = new JtTokenType(1, "bool", (o, t) => new JtBool(o, t), nameof(Bool));
        public static readonly JtTokenType Byte = new JtTokenType(2, "byte", (o, t) => new JtByte(o, t), nameof(Byte));
        public static readonly JtTokenType Short = new JtTokenType(3, "short", (o, t) => new JtShort(o, t), nameof(Short));
        public static readonly JtTokenType Int = new JtTokenType(4, "int", (o, t) => new JtInt(o, t), nameof(Int));
        public static readonly JtTokenType Long = new JtTokenType(5, "long", (o, t) => new JtLong(o, t), nameof(Long));
        public static readonly JtTokenType Float = new JtTokenType(6, "float", (o, t) => new JtFloat(o, t), nameof(Float));
        public static readonly JtTokenType Double = new JtTokenType(7, "double", (o, t) => new JtDouble(o, t), nameof(Double));
        public static readonly JtTokenType String = new JtTokenType(8, "string", (o, t) => new JtString(o, t), nameof(String));
        public static readonly JtTokenType Block = new JtTokenType(9, "block", (o, t) => new JtBlock(o, t), nameof(Block));
        public static readonly JtTokenType Array = new JtTokenType(10, "array", (o, t) => new JtArray(o, t), nameof(Array));
        public static readonly JtTokenType Enum = new JtTokenType(11, "enum", (o, t) => new JtEnum(o, t), nameof(Enum));


        public static readonly JtTokenType[] Items = new JtTokenType[]
        {
             JtTokenType.Bool,
                JtTokenType.Byte,
                JtTokenType.Short,
                JtTokenType.Int,
                JtTokenType.Long,
                 JtTokenType.Float,
                 JtTokenType.Double,
                 JtTokenType.String,
                JtTokenType.Block,
                 JtTokenType.Array,
                 JtTokenType.Enum,
                JtTokenType.Unknown,

        };


        public static JtTokenType GetById(int id)
        {
            return id switch
            {
                1 => JtTokenType.Bool,
                2 => JtTokenType.Byte,
                3 => JtTokenType.Short,
                4 => JtTokenType.Int,
                5 => JtTokenType.Long,
                6 => JtTokenType.Float,
                7 => JtTokenType.Double,
                8 => JtTokenType.String,
                9 => JtTokenType.Block,
                10 => JtTokenType.Array,
                11 => JtTokenType.Enum,
                _ => JtTokenType.Unknown,
            };
        }
        public static JtTokenType GetByName(string? name)
        {
            name = name?.ToLower();
            return name switch
            {
                "bool" => JtTokenType.Bool,
                "byte" => JtTokenType.Byte,
                "short" => JtTokenType.Short,
                "int" => JtTokenType.Int,
                "long" => JtTokenType.Long,
                "float" => JtTokenType.Float,
                "double" => JtTokenType.Double,
                "string" => JtTokenType.String,
                "block" => JtTokenType.Block,
                "array" => JtTokenType.Array,
                "enum" => JtTokenType.Enum,
                _ => JtTokenType.Unknown,
            };
        }

        public override bool Equals(object? obj) => Equals(obj as JtTokenType);
        public bool Equals(JtTokenType? other) => other != null && Id == other.Id;
        public override int GetHashCode() => HashCode.Combine(Id);
        public override string ToString() => $"{Name}({Id})";


        public static bool operator ==(JtTokenType? left, JtTokenType? right) => left?.Id == right?.Id;
        public static bool operator !=(JtTokenType? left, JtTokenType? right) => !(left == right);

        public static implicit operator int(JtTokenType tokenType) => tokenType.Id;
        public static explicit operator JtTokenType(int id) => GetById(id);
    }
}
