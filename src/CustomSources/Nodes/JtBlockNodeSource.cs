﻿using System.Text;
using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.CustomSources.Nodes;

public sealed class JtBlockNodeSource : JtContainerNodeSource
{
    public override JtContainerType ContainerDisplayType => JtContainerType.Block;
    public override JtNodeType Type => JtNodeType.Block;


    public override JtNodeCollectionSource Children { get; }
    internal static JtBlockNodeSource CreateSelf(IJtNodeSourceParent parent) => new JtBlockNodeSource(parent);
    internal static JtBlockNodeSource CreateSelf(IJtNodeSourceParent parent, JObject source) => new JtBlockNodeSource(parent, source);
    public JtBlockNodeSource(IJtNodeSourceParent parent) : base(parent)
    {
        Children = JtNodeCollectionSource.Create(this);
    }
    internal JtBlockNodeSource(JtBlockNode node) : base(node)
    {
        Children = node.Children.CreateSource();
    }
    internal JtBlockNodeSource(IJtNodeSourceParent parent, JObject source) : base(parent, source)
    {
        Children = JtNodeCollectionSource.Create(this, source["children"]);
    }
    internal JtBlockNodeSource(IJtNodeSourceParent parent, JtBlockNodeSource @base, JObject? @override) : base(parent, @base, @override)
    {
        Children = @base.Children.CreateOverride(this, (JArray?)@override?["children"]);
    }

    internal override void BuildJsonDeclaration(StringBuilder sb)
    {
        BuildCommonJson(sb);
        sb.Append(", \"children\": ");
        Children.BuildJson(sb);
        sb.Append('}');
    }
    public override JtNode CreateInstance(IJtNodeParent parent, JToken? @override) => new JtBlockNode(parent, this, @override);
    public override JtNodeSource CreateOverride(IJtNodeSourceParent parent, JObject? @override) => new JtBlockNodeSource(parent, this, @override);
}