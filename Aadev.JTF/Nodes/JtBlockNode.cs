using Aadev.JTF.CustomSources;
using Aadev.JTF.CustomSources.Nodes;
using Aadev.JTF.Tools;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.Nodes;

public sealed class JtBlockNode : JtContainerNode
{
    public override JtNodeCollection Children { get; }
    public override JtContainerType ContainerDisplayType => JtContainerType.Block;
    public override JtNodeType Type => JtNodeType.Block;

    internal static JtBlockNode CreateSelf(IJtNodeParent parent) => new JtBlockNode(parent);
    internal static JtBlockNode CreateSelf(IJtNodeParent parent, JObject source) => new JtBlockNode(parent, source);
    public JtBlockNode(IJtNodeParent parent) : base(parent)
    {
        Children = JtNodeCollection.Create(this);
    }
    internal JtBlockNode(IJtNodeParent parent, JObject source) : base(parent, source)
    {
        Children = JtNodeCollection.Create(this, source["children"]);
    }
    internal JtBlockNode(IJtNodeParent parent, JtBlockNodeSource source, JToken? @override) : base(parent, source, @override)
    {
        Children = source.Children.CreateInstance(this, (JArray?)@override?["children"]);
    }
    public override JtNodeSource CreateSource() => currentSource ??= new JtBlockNodeSource(this);
    internal override void BuildJson(JsonBuilder jb)
    {
        BuildCommonJson(jb);
        if (Base is not null)
        {
            if (Children.IsOverridden())
            {
                jb.AddProperty("children", Children);
            }

            jb.EndBlock();
            return;
        }

        jb.AddProperty("children", Children);
        jb.EndBlock();
    }
}