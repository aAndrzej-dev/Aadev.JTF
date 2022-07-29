using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System;

namespace Aadev.JTF
{
    public class JtNodeType : IEquatable<JtNodeType?>
    {
        private readonly Func<JObject, JTemplate, IIdentifiersManager, JtNode> instanceFactory;
        private readonly Func<JTemplate, IIdentifiersManager, JtNode> emptyInstanceFactory;
        private JtNodeType(int id, string name, Func<JObject, JTemplate, IIdentifiersManager, JtNode> instanceFactory, Func<JTemplate, IIdentifiersManager, JtNode> emptyInstanceFactory, string displayName, bool isContainerType = false, bool isNumericType = false)
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


        public static readonly JtNodeType Unknown = new JtNodeType(0, "unknown", (o, t, i) => new JtUnknown(o, t, i), (t, i) => new JtUnknown(t, i), nameof(Unknown));
        public static readonly JtNodeType Bool = new JtNodeType(1, "bool", (o, t, i) => new JtBool(o, t, i), (t, i) => new JtBool(t, i), nameof(Bool));
        public static readonly JtNodeType Byte = new JtNodeType(2, "byte", (o, t, i) => new JtByte(o, t, i), (t, i) => new JtByte(t, i), nameof(Byte), false, true);
        public static readonly JtNodeType Short = new JtNodeType(3, "short", (o, t, i) => new JtShort(o, t, i), (t, i) => new JtShort(t, i), nameof(Short), false, true);
        public static readonly JtNodeType Int = new JtNodeType(4, "int", (o, t, i) => new JtInt(o, t, i), (t, i) => new JtInt(t, i), nameof(Int), false, true);
        public static readonly JtNodeType Long = new JtNodeType(5, "long", (o, t, i) => new JtLong(o, t, i), (t, i) => new JtLong(t, i), nameof(Long), false, true);
        public static readonly JtNodeType Float = new JtNodeType(6, "float", (o, t, i) => new JtFloat(o, t, i), (t, i) => new JtFloat(t, i), nameof(Float), false, true);
        public static readonly JtNodeType Double = new JtNodeType(7, "double", (o, t, i) => new JtDouble(o, t, i), (t, i) => new JtDouble(t, i), nameof(Double), false, true);
        public static readonly JtNodeType String = new JtNodeType(8, "string", (o, t, i) => new JtString(o, t, i), (t, i) => new JtString(t, i), nameof(String));
        public static readonly JtNodeType Block = new JtNodeType(9, "block", (o, t, i) => new JtBlock(o, t, i), (t, i) => new JtBlock(t, i), nameof(Block), true);
        public static readonly JtNodeType Array = new JtNodeType(10, "array", (o, t, i) => new JtArray(o, t, i), (t, i) => new JtArray(t, i), nameof(Array), true);
        public static readonly JtNodeType Enum = new JtNodeType(11, "enum", (o, t, i) => new JtEnum(o, t, i), (t, i) => new JtEnum(t, i), nameof(Enum));


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
        public JtNode CreateInstance(JObject obj, JTemplate template, IIdentifiersManager identifiersManager) => instanceFactory(obj, template, identifiersManager);
        public JtNode CreateEmptyInstance(JTemplate template, IIdentifiersManager identifiersManager) => emptyInstanceFactory(template, identifiersManager);
        public override bool Equals(object? obj) => Equals(obj as JtNodeType);
        public bool Equals(JtNodeType? other) => other != null && Id == other.Id;
        public override int GetHashCode() => HashCode.Combine(Id);
        public override string ToString() => $"{Name}({Id})";


        public static bool operator ==(JtNodeType? left, JtNodeType? right) => left?.Id == right?.Id;
        public static bool operator !=(JtNodeType? left, JtNodeType? right) => !(left == right);
    }
}