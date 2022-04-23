using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System;

namespace Aadev.JTF
{
    public class JtTokenType : IEquatable<JtTokenType?>
    {
        private readonly Func<JObject, JTemplate, JtToken> instanceFactory;
        private readonly Func<JTemplate, JtToken> emptyInstanceFactory;
        private JtTokenType(int id, string name, Func<JObject, JTemplate, JtToken> instanceFactory, Func<JTemplate, JtToken> emptyInstanceFactory, string displayName, bool isContainerType = false, bool isNumericType = false)
        {
            Id = id;
            Name = name;
            this.instanceFactory = instanceFactory;
            this.emptyInstanceFactory = emptyInstanceFactory;
            DisplayName = displayName;
            IsContainerType = isContainerType;
            IsNumericType = isNumericType;
        }

        public int Id { get; }
        public string Name { get; }
        public string DisplayName { get; }

        public bool IsContainerType { get; }
        public bool IsNumericType { get; }


        public static readonly JtTokenType Unknown = new JtTokenType(0, "unknown", (o, t) => new JtUnknown(o, t), t => new JtUnknown(t), nameof(Unknown));
        public static readonly JtTokenType Bool = new JtTokenType(1, "bool", (o, t) => new JtBool(o, t), t => new JtBool(t), nameof(Bool));
        public static readonly JtTokenType Byte = new JtTokenType(2, "byte", (o, t) => new JtByte(o, t), t => new JtByte(t), nameof(Byte), false, true);
        public static readonly JtTokenType Short = new JtTokenType(3, "short", (o, t) => new JtShort(o, t), t => new JtShort(t), nameof(Short), false, true);
        public static readonly JtTokenType Int = new JtTokenType(4, "int", (o, t) => new JtInt(o, t), t => new JtInt(t), nameof(Int), false, true);
        public static readonly JtTokenType Long = new JtTokenType(5, "long", (o, t) => new JtLong(o, t), t => new JtLong(t), nameof(Long), false, true);
        public static readonly JtTokenType Float = new JtTokenType(6, "float", (o, t) => new JtFloat(o, t), t => new JtFloat(t), nameof(Float), false, true);
        public static readonly JtTokenType Double = new JtTokenType(7, "double", (o, t) => new JtDouble(o, t), t => new JtDouble(t), nameof(Double), false, true);
        public static readonly JtTokenType String = new JtTokenType(8, "string", (o, t) => new JtString(o, t), t => new JtString(t), nameof(String));
        public static readonly JtTokenType Block = new JtTokenType(9, "block", (o, t) => new JtBlock(o, t), t => new JtBlock(t), nameof(Block), true);
        public static readonly JtTokenType Array = new JtTokenType(10, "array", (o, t) => new JtArray(o, t), t => new JtArray(t), nameof(Array), true);
        public static readonly JtTokenType Enum = new JtTokenType(11, "enum", (o, t) => new JtEnum(o, t), t => new JtEnum(t), nameof(Enum));


        private static readonly JtTokenType[] items = new JtTokenType[]
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
        public static JtTokenType[] Items => items;


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
        public JtToken CreateInstance(JObject obj, JTemplate template) => instanceFactory(obj, template);
        public JtToken CreateEmptyInstance(JTemplate template) => emptyInstanceFactory(template);

        public override bool Equals(object? obj) => Equals(obj as JtTokenType);
        public bool Equals(JtTokenType? other) => other != null && Id == other.Id;
        public override int GetHashCode() => HashCode.Combine(Id);
        public override string ToString() => $"{Name}({Id})";


        public static bool operator ==(JtTokenType? left, JtTokenType? right) => left?.Id == right?.Id;
        public static bool operator !=(JtTokenType? left, JtTokenType? right) => !(left == right);
    }
}
