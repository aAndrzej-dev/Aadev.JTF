using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System;

namespace Aadev.JTF
{
    public sealed class JtNodeType : IEquatable<JtNodeType?>
    {
        private readonly Func<IJtNodeParent, JObject, JtNode> instanceFactory;
        private readonly Func<IJtNodeParent, JtNode> emptyInstanceFactory;
        private JtNodeType(int id, string name, Func<IJtNodeParent, JObject, JtNode> instanceFactory, Func<IJtNodeParent, JtNode> emptyInstanceFactory, string displayName, Type? valueType = null)
        {
            Id = id;
            Name = name;
            this.instanceFactory = instanceFactory;
            this.emptyInstanceFactory = emptyInstanceFactory;
            DisplayName = displayName;
            ValueType = valueType;
        }

        public int Id { get; }
        public string Name { get; }
        public string DisplayName { get; }
        public Type? ValueType { get; }


        public static readonly JtNodeType Unknown = new JtNodeType(0, "unknown", (p, s) => new JtUnknown(p, s), (p) => new JtUnknown(p), nameof(Unknown));
        public static readonly JtNodeType Bool = new JtNodeType(1, "bool", (p, s) => new JtBool(p, s), (p) => new JtBool(p), nameof(Bool), typeof(bool));
        public static readonly JtNodeType Byte = new JtNodeType(2, "byte", (p, s) => new JtByte(p, s), (p) => new JtByte(p), nameof(Byte), typeof(byte));
        public static readonly JtNodeType Short = new JtNodeType(3, "short", (p, s) => new JtShort(p, s), (p) => new JtShort(p), nameof(Short), typeof(short));
        public static readonly JtNodeType Int = new JtNodeType(4, "int", (p, s) => new JtInt(p, s), (p) => new JtInt(p), nameof(Int), typeof(int));
        public static readonly JtNodeType Long = new JtNodeType(5, "long", (p, s) => new JtLong(p, s), (p) => new JtLong(p), nameof(Long), typeof(long));
        public static readonly JtNodeType Float = new JtNodeType(6, "float", (p, s) => new JtFloat(p, s), (p) => new JtFloat(p), nameof(Float), typeof(float));
        public static readonly JtNodeType Double = new JtNodeType(7, "double", (p, s) => new JtDouble(p, s), (p) => new JtDouble(p), nameof(Double), typeof(double));
        public static readonly JtNodeType String = new JtNodeType(8, "string", (p, s) => new JtString(p, s), (p) => new JtString(p), nameof(String), typeof(string));
        public static readonly JtNodeType Block = new JtNodeType(9, "block", (p, s) => new JtBlock(p, s), (p) => new JtBlock(p), nameof(Block));
        public static readonly JtNodeType Array = new JtNodeType(10, "array", (p, s) => new JtArray(p, s), (p) => new JtArray(p), nameof(Array));


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
                11 => JtNodeType.String,
                _ => JtNodeType.Unknown,
            };
        }
        public static JtNodeType GetByName(string? name)
        {
            name = name?.ToLowerInvariant();
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
                "enum" => JtNodeType.String,
                _ => JtNodeType.Unknown,
            };
        }
        public JtNode CreateInstance(IJtNodeParent parent, JObject source) => instanceFactory(parent, source);
        public JtNode CreateEmptyInstance(IJtNodeParent parent) => emptyInstanceFactory(parent);
        public override bool Equals(object? obj) => Equals(obj as JtNodeType);
        public bool Equals(JtNodeType? other) => other != null && Id == other.Id;
        public override int GetHashCode() => HashCode.Combine(Id);
        public override string ToString() => $"{Name}({Id})";


        public static bool operator ==(JtNodeType? left, JtNodeType? right) => left?.Id == right?.Id;
        public static bool operator !=(JtNodeType? left, JtNodeType? right) => !(left == right);
    }
}