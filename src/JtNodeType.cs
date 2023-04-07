using Aadev.JTF.CustomSources;
using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Aadev.JTF
{
    public sealed class JtNodeType : IEquatable<JtNodeType?>
    {
        private readonly Func<IJtNodeParent, JObject, JtNode> instanceFactory;
        private readonly Func<IJtNodeParent, JtNode> emptyInstanceFactory;
        private readonly Func<IJtNodeSourceParent, JObject, JtNodeSource> sourceFactory;
        private readonly Func<IJtNodeSourceParent, JtNodeSource> emptySourceFactory;

        private JtNodeType(int id, string name, Func<IJtNodeParent, JObject, JtNode> instanceFactory, Func<IJtNodeParent, JtNode> emptyInstanceFactory, Func<IJtNodeSourceParent, JObject, JtNodeSource> sourceFactory, Func<IJtNodeSourceParent, JtNodeSource> emptySourceFactory, string displayName, Type? valueType = null)
        {
            Id = id;
            Name = name;
            this.instanceFactory = instanceFactory;
            this.emptyInstanceFactory = emptyInstanceFactory;
            this.sourceFactory = sourceFactory;
            this.emptySourceFactory = emptySourceFactory;
            DisplayName = displayName;
            ValueType = valueType;
        }

        public int Id { get; }
        public string Name { get; }
        public string DisplayName { get; }
        public Type? ValueType { get; }


        public static readonly JtNodeType Unknown = new JtNodeType(0, "unknown", (p, s) => new JtUnknownNode(p, s), (p) => new JtUnknownNode(p), (p, s) => new JtUnknownNodeSource(p, s), (p) => new JtUnknownNodeSource(p), nameof(Unknown));
        public static readonly JtNodeType Bool = new JtNodeType(1, "bool", (p, s) => new JtBoolNode(p, s), (p) => new JtBoolNode(p), (p, s) => new JtBoolNodeSource(p, s), (p) => new JtBoolNodeSource(p), nameof(Bool), typeof(bool));
        public static readonly JtNodeType Byte = new JtNodeType(2, "byte", (p, s) => new JtByteNode(p, s), (p) => new JtByteNode(p), (p, s) => new JtByteNodeSource(p, s), (p) => new JtByteNodeSource(p), nameof(Byte), typeof(byte));
        public static readonly JtNodeType Short = new JtNodeType(3, "short", (p, s) => new JtShortNode(p, s), (p) => new JtShortNode(p), (p, s) => new JtShortNodeSource(p, s), (p) => new JtShortNodeSource(p), nameof(Short), typeof(short));
        public static readonly JtNodeType Int = new JtNodeType(4, "int", (p, s) => new JtIntNode(p, s), (p) => new JtIntNode(p), (p, s) => new JtIntNodeSource(p, s), (p) => new JtIntNodeSource(p), nameof(Int), typeof(int));
        public static readonly JtNodeType Long = new JtNodeType(5, "long", (p, s) => new JtLongNode(p, s), (p) => new JtLongNode(p), (p, s) => new JtLongNodeSource(p, s), (p) => new JtLongNodeSource(p), nameof(Long), typeof(long));
        public static readonly JtNodeType Float = new JtNodeType(6, "float", (p, s) => new JtFloatNode(p, s), (p) => new JtFloatNode(p), (p, s) => new JtFloatNodeSource(p, s), (p) => new JtFloatNodeSource(p), nameof(Float), typeof(float));
        public static readonly JtNodeType Double = new JtNodeType(7, "double", (p, s) => new JtDoubleNode(p, s), (p) => new JtDoubleNode(p), (p, s) => new JtDoubleNodeSource(p, s), (p) => new JtDoubleNodeSource(p), nameof(Double), typeof(double));
        public static readonly JtNodeType String = new JtNodeType(8, "string", (p, s) => new JtStringNode(p, s), (p) => new JtStringNode(p), (p, s) => new JtStringNodeSource(p, s), (p) => new JtStringNodeSource(p), nameof(String), typeof(string));
        public static readonly JtNodeType Block = new JtNodeType(9, "block", (p, s) => new JtBlockNode(p, s), (p) => new JtBlockNode(p), (p, s) => new JtBlockNodeSource(p, s), (p) => new JtBlockNodeSource(p), nameof(Block));
        public static readonly JtNodeType Array = new JtNodeType(10, "array", (p, s) => new JtArrayNode(p, s), (p) => new JtArrayNode(p), (p, s) => new JtArrayNodeSource(p, s), (p) => new JtArrayNodeSource(p), nameof(Array));


        private static readonly IReadOnlyCollection<JtNodeType> items = System.Array.AsReadOnly(new JtNodeType[]
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
        });
        public static IReadOnlyCollection<JtNodeType> Items => items;


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
        public JtNodeSource CreateSource(IJtNodeSourceParent parent, JObject source) => sourceFactory(parent, source);
        public JtNodeSource CreateEmptySource(IJtNodeSourceParent parent) => emptySourceFactory(parent);
        public override bool Equals(object? obj) => Equals(obj as JtNodeType);
        public bool Equals(JtNodeType? other) => other is not null && Id == other.Id;
        public override int GetHashCode() => HashCode.Combine(Id);
        public override string ToString() => $"{Name}({Id})";


        public static bool operator ==(JtNodeType? left, JtNodeType? right) => left?.Id == right?.Id;
        public static bool operator !=(JtNodeType? left, JtNodeType? right) => !(left == right);
    }
}