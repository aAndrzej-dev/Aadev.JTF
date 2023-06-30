using System;
using System.Collections.Generic;
using Aadev.JTF.CustomSources;
using Aadev.JTF.CustomSources.Nodes;
using Aadev.JTF.Nodes;
using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF;

public sealed class JtNodeType : IEquatable<JtNodeType?>
{
    internal delegate JtNode EmptyJtNodeConstructor(IJtNodeParent parent);
    internal delegate JtNode JtNodeConstructor(IJtNodeParent parent, JObject source);
    internal delegate JtNodeSource EmptyJtNodeSourceConstructor(IJtNodeSourceParent parent);
    internal delegate JtNodeSource JtNodeSourceConstructor(IJtNodeSourceParent parent, JObject source);

    private readonly JtNodeConstructor instanceFactory;
    private readonly EmptyJtNodeConstructor emptyInstanceFactory;
    private readonly JtNodeSourceConstructor sourceFactory;
    private readonly EmptyJtNodeSourceConstructor emptySourceFactory;

    private JtNodeType(int id, string name, JtNodeConstructor instanceFactory, EmptyJtNodeConstructor emptyInstanceFactory, JtNodeSourceConstructor sourceFactory, EmptyJtNodeSourceConstructor emptySourceFactory, string displayName, Type? valueType = null)
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


    public static readonly JtNodeType Unknown = new JtNodeType(0, "unknown", JtUnknownNode.CreateSelf, JtUnknownNode.CreateSelf, JtUnknownNodeSource.CreateSelf, JtUnknownNodeSource.CreateSelf, nameof(Unknown));
    public static readonly JtNodeType Bool = new JtNodeType(1, "bool", JtBoolNode.CreateSelf, JtBoolNode.CreateSelf, JtBoolNodeSource.CreateSelf, JtBoolNodeSource.CreateSelf, nameof(Bool), typeof(bool));
    public static readonly JtNodeType Byte = new JtNodeType(2, "byte", JtByteNode.CreateSelf, JtByteNode.CreateSelf, JtByteNodeSource.CreateSelf, JtByteNodeSource.CreateSelf, nameof(Byte), typeof(byte));
    public static readonly JtNodeType Short = new JtNodeType(3, "short", JtShortNode.CreateSelf, JtShortNode.CreateSelf, JtShortNodeSource.CreateSelf, JtShortNodeSource.CreateSelf, nameof(Short), typeof(short));
    public static readonly JtNodeType Int = new JtNodeType(4, "int", JtIntNode.CreateSelf, JtIntNode.CreateSelf, JtIntNodeSource.CreateSelf, JtIntNodeSource.CreateSelf, nameof(Int), typeof(int));
    public static readonly JtNodeType Long = new JtNodeType(5, "long", JtLongNode.CreateSelf, JtLongNode.CreateSelf, JtLongNodeSource.CreateSelf, JtLongNodeSource.CreateSelf, nameof(Long), typeof(long));
    public static readonly JtNodeType Float = new JtNodeType(6, "float", JtFloatNode.CreateSelf, JtFloatNode.CreateSelf, JtFloatNodeSource.CreateSelf, JtFloatNodeSource.CreateSelf, nameof(Float), typeof(float));
    public static readonly JtNodeType Double = new JtNodeType(7, "double", JtDoubleNode.CreateSelf, JtDoubleNode.CreateSelf, JtDoubleNodeSource.CreateSelf, JtDoubleNodeSource.CreateSelf, nameof(Double), typeof(double));
    public static readonly JtNodeType String = new JtNodeType(8, "string", JtStringNode.CreateSelf, JtStringNode.CreateSelf, JtStringNodeSource.CreateSelf, JtStringNodeSource.CreateSelf, nameof(String), typeof(string));
    public static readonly JtNodeType Block = new JtNodeType(9, "block", JtBlockNode.CreateSelf, JtBlockNode.CreateSelf, JtBlockNodeSource.CreateSelf, JtBlockNodeSource.CreateSelf, nameof(Block));
    public static readonly JtNodeType Array = new JtNodeType(10, "array", JtArrayNode.CreateSelf, JtArrayNode.CreateSelf, JtArrayNodeSource.CreateSelf, JtArrayNodeSource.CreateSelf, nameof(Array));


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