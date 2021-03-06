using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System;

namespace Aadev.JTF
{
    public class JtNodeType : IEquatable<JtNodeType?>
    {
        private readonly Func<JObject, JTemplate, JtNode> instanceFactory;
        private readonly Func<JTemplate, JtNode> emptyInstanceFactory;
        private JtNodeType(int id, string name, Func<JObject, JTemplate, JtNode> instanceFactory, Func<JTemplate, JtNode> emptyInstanceFactory, string displayName, bool isContainerType = false, bool isNumericType = false)
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


        public static readonly JtNodeType Unknown = new JtNodeType(0, "unknown", (o, t) => new JtUnknown(o, t), t => new JtUnknown(t), nameof(Unknown));
        public static readonly JtNodeType Bool = new JtNodeType(1, "bool", (o, t) => new JtBool(o, t), t => new JtBool(t), nameof(Bool));
        public static readonly JtNodeType Byte = new JtNodeType(2, "byte", (o, t) => new JtByte(o, t) , t => new JtByte(t), nameof(Byte), false, true);
        public static readonly JtNodeType Short = new JtNodeType(3, "short", (o, t) => new JtShort(o, t), t => new JtShort(t), nameof(Short), false, true);
        public static readonly JtNodeType Int = new JtNodeType(4, "int", (o, t) => new JtInt(o, t), t => new JtInt(t), nameof(Int), false, true);
        public static readonly JtNodeType Long = new JtNodeType(5, "long", (o, t) => new JtLong(o, t), t => new JtLong(t), nameof(Long), false, true);
        public static readonly JtNodeType Float = new JtNodeType(6, "float", (o, t) => new JtFloat(o, t), t => new JtFloat(t), nameof(Float), false, true);
        public static readonly JtNodeType Double = new JtNodeType(7, "double", (o, t) => new JtDouble(o, t), t => new JtDouble(t), nameof(Double), false, true);
        public static readonly JtNodeType String = new JtNodeType(8, "string", (o, t) => new JtString(o, t), t => new JtString(t), nameof(String));
        public static readonly JtNodeType Block = new JtNodeType(9, "block", (o, t) => new JtBlock(o, t), t => new JtBlock(t), nameof(Block), true);
        public static readonly JtNodeType Array = new JtNodeType(10, "array", (o, t) => new JtArray(o, t), t => new JtArray(t), nameof(Array), true);
        public static readonly JtNodeType Enum = new JtNodeType(11, "enum", (o, t) => new JtEnum(o, t), t => new JtEnum(t), nameof(Enum));


        private static readonly JtNodeType[] items = new JtNodeType[]
        {
             JtNodeType.Bool,
             JtNodeType.Byte,
             JtNodeType.Short,
             JtNodeType.Int,
             JtNodeType.Long,
             JtNodeType.Float,
             JtNodeType.Double,
             JtNodeType.String,
             JtNodeType.Block,
             JtNodeType.Array,
             JtNodeType.Enum,
             JtNodeType.Unknown,

        };
        public static JtNodeType[] Items => items;


        public static JtNodeType GetById(int id)
        {
            return id switch
            {
                1 => JtNodeType.Bool,
                2 => JtNodeType.Byte,
                3 => JtNodeType.Short,
                4 => JtNodeType.Int,
                5 => JtNodeType.Long,
                6 => JtNodeType.Float,
                7 => JtNodeType.Double,
                8 => JtNodeType.String,
                9 => JtNodeType.Block,
                10 => JtNodeType.Array,
                11 => JtNodeType.Enum,
                _ => JtNodeType.Unknown,
            };
        }
        public static JtNodeType GetByName(string? name)
        {
            name = name?.ToLower();
            return name switch
            {
                "bool" => JtNodeType.Bool,
                "byte" => JtNodeType.Byte,
                "short" => JtNodeType.Short,
                "int" => JtNodeType.Int,
                "long" => JtNodeType.Long,
                "float" => JtNodeType.Float,
                "double" => JtNodeType.Double,
                "string" => JtNodeType.String,
                "block" => JtNodeType.Block,
                "array" => JtNodeType.Array,
                "enum" => JtNodeType.Enum,
                _ => JtNodeType.Unknown,
            };
        }
        public JtNode CreateInstance(JObject obj, JTemplate template) => instanceFactory(obj, template);
        public JtNode CreateEmptyInstance(JTemplate template) => emptyInstanceFactory(template);

        public override bool Equals(object? obj) => Equals(obj as JtNodeType);
        public bool Equals(JtNodeType? other) => other != null && Id == other.Id;
        public override int GetHashCode() => HashCode.Combine(Id);
        public override string ToString() => $"{Name}({Id})";


        public static bool operator ==(JtNodeType? left, JtNodeType? right) => left?.Id == right?.Id;
        public static bool operator !=(JtNodeType? left, JtNodeType? right) => !(left == right);
    }
}
