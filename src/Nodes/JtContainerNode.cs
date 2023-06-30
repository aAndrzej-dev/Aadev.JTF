using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Aadev.JTF.Common;
using Aadev.JTF.CustomSources.Nodes;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.Types;

public abstract class JtContainerNode : JtNode, IJtNodeParent
{
    private bool? disableCollapse;

    public new JtContainerNodeSource? Base => (JtContainerNodeSource?)base.Base;
    public override JTokenType JsonType => ContainerJsonType is JtContainerType.Array ? JTokenType.Array : JTokenType.Object;

    [Browsable(false)] public abstract JtNodeCollection Children { get; }
    [Browsable(false)] public abstract JtContainerType ContainerDisplayType { get; }

    [DefaultValue(false)] public bool DisableCollapse { get => disableCollapse ?? Base?.DisableCollapse ?? false; set => disableCollapse = value; }
    public JtContainerType ContainerJsonType { get; set; }

    private protected JtContainerNode(IJtNodeParent parent) : base(parent)
    {
        ContainerJsonType = ContainerDisplayType;
    }
    private protected JtContainerNode(IJtNodeParent parent, JObject source) : base(parent, source)
    {
        DisableCollapse = (bool?)source["disableCollapse"] ?? false;
        if (source["jsonType"] is JValue jt)
        {
            ContainerJsonType = (string?)jt.Value switch
            {
                "array" => JtContainerType.Array,
                "block" => JtContainerType.Block,
                _ => ContainerDisplayType,
            };
        }
        else
            ContainerJsonType = ContainerDisplayType;
    }
    private protected JtContainerNode(IJtNodeParent parent, JtContainerNodeSource source, JToken? @override) : base(parent, source, @override)
    {
        if (@override?["jsonType"] is JValue jt)
        {
            ContainerJsonType = (string?)jt.Value switch
            {
                "array" => JtContainerType.Array,
                "block" => JtContainerType.Block,
                _ => ContainerDisplayType,
            };
        }
        else
            ContainerJsonType = source.ContainerJsonType;
        disableCollapse = (bool?)(@override?["disableCollapse"]);
    }

    public override JToken CreateDefaultValue()
    {
        if (ContainerJsonType is JtContainerType.Array)
            return new JArray();
        else
            return new JObject();
    }

    private protected override void BuildCommonJson(StringBuilder sb)
    {
        base.BuildCommonJson(sb);
        if (ContainerJsonType != ContainerDisplayType)
            sb.Append($", \"jsonType\": \"{ContainerJsonType.ToLowerString()}\"");
        if (DisableCollapse)
            sb.Append(", \"disableCollapse\": true");
    }

    IdentifiersManager IJtNodeParent.GetIdentifiersManagerForChild() => IdentifiersManager;


    ICustomSourceProvider IHaveCustomSourceProvider.SourceProvider => this;
    JtContainerNode IJtNodeParent.Owner => this;

    [Browsable(false)] public bool HasExternalChildrenSource => Children.IsExternal;

    JtNodeCollection IJtNodeParent.OwnersMainCollection => Children;

    public IReadOnlyList<JtNode> GetNodes() => new ReadOnlyCollection<JtNode>(Children.Nodes!);
    public JtNode GetNode(int index) => Children.Nodes![index];

    IEnumerable<IJtCommonContentElement> IJtCommonParent.EnumerateChildrenElements() => ((IJtCommonParent)Children).EnumerateChildrenElements();
    IJtCommonNodeCollection IJtCommonParent.GetChildrenElementsCollection() => Children;
}

public enum JtContainerType
{
    Array,
    Block
}
internal static class JtContainerTypeEx
{
    public static string ToLowerString(this JtContainerType containerType)
    {
        return containerType switch
        {
            JtContainerType.Array => "array",
            JtContainerType.Block => "block",
            _ => throw new ArgumentOutOfRangeException(nameof(containerType)),
        };
    }
}
