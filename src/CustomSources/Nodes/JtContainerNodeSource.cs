using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Aadev.JTF.Common;
using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.CustomSources.Nodes;

public abstract class JtContainerNodeSource : JtNodeSource, IJtNodeSourceParent, IJtCommonParent
{
    [Browsable(false)] public abstract JtNodeCollectionSource Children { get; }
    [Browsable(false)] public abstract JtContainerType ContainerDisplayType { get; }
    [DefaultValue(false)] public bool DisableCollapse { get; set; }
    public JtContainerType ContainerJsonType { get; set; }

    public override JTokenType JsonType => ContainerJsonType is JtContainerType.Array ? JTokenType.Array : JTokenType.Object;

    [Browsable(false)] public bool HasExternalChildrenSource => Children.IsExternal;


    public JtNodeSource? Owner => this;


    private protected JtContainerNodeSource(IJtNodeSourceParent parent) : base(parent) { ContainerJsonType = ContainerDisplayType; }
    private protected JtContainerNodeSource(JtContainerNode node) : base(node)
    {
        DisableCollapse = node.DisableCollapse;
        ContainerJsonType = node.ContainerJsonType;
    }
    private protected JtContainerNodeSource(IJtNodeSourceParent parent, JObject source) : base(parent, source)
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

        if (ContainerJsonType == ContainerDisplayType && ContainerDisplayType == JtContainerType.Array && (bool?)source["makeObject"] is bool)
        {
            ContainerJsonType = JtContainerType.Block;
        }
    }
    private protected JtContainerNodeSource(IJtNodeSourceParent parent, JtContainerNodeSource @base, JObject? @override) : base(parent, @base, @override)
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
            ContainerJsonType = @base.ContainerJsonType;
        DisableCollapse = (bool)(@override?["disableCollapse"] ?? @base.DisableCollapse);
    }
    private protected override void BuildCommonJson(StringBuilder sb)
    {
        base.BuildCommonJson(sb);
        if (ContainerJsonType != ContainerDisplayType)
            sb.Append($", \"jsonType\": \"{ContainerJsonType.ToLowerString()}\"");
        if (DisableCollapse)
            sb.Append(", \"disableCollapse\": true");
    }
    public override JToken CreateDefaultValue()
    {
        if (ContainerJsonType is JtContainerType.Array)
            return new JArray();
        else
            return new JObject();
    }

    public IEnumerable<IJtCommonContentElement> EnumerateChildrenElements() => ((IJtCommonParent)Children).EnumerateChildrenElements();
    public IJtCommonNodeCollection GetChildrenElementsCollection() => Children;
}
