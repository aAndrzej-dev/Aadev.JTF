using System.ComponentModel;
using Aadev.JTF.CustomSources;
using Aadev.JTF.CustomSources.Nodes;
using Aadev.JTF.Tools;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.Nodes;

public sealed class JtArrayNode : JtContainerNode
{
    private int? maxSize;
    private bool? singleType;

    public new JtArrayNodeSource? Base => (JtArrayNodeSource?)base.Base;
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)] public override JtNodeCollection Children => Prefabs;
    public override JtContainerType ContainerDisplayType => JtContainerType.Array;
    public override JtNodeType Type => JtNodeType.Array;

    [Browsable(false)] public JtNodeCollection Prefabs { get; }
    [DefaultValue(-1)] public int MaxSize { get => maxSize ?? Base?.MaxSize ?? -1; set => maxSize = value; }
    [DefaultValue(false)] public bool SingleType { get => singleType ?? Base?.SingleType ?? false; set => singleType = value; }


    [Browsable(false)] public bool MakeAsObject => ContainerJsonType is JtContainerType.Block;


    internal static JtArrayNode CreateSelf(IJtNodeParent parent) => new JtArrayNode(parent);
    internal static JtArrayNode CreateSelf(IJtNodeParent parent, JObject source) => new JtArrayNode(parent, source);


    public JtArrayNode(IJtNodeParent parent) : base(parent)
    {
        MaxSize = -1;
        Prefabs = JtNodeCollection.Create(this);
    }
    internal JtArrayNode(IJtNodeParent parent, JObject source) : base(parent, source)
    {
        SingleType = (bool?)source["singleType"] ?? false;
        MaxSize = (int?)source["maxSize"] ?? -1;


        Prefabs = JtNodeCollection.Create(this, source["prefabs"]);
        if (ContainerDisplayType == ContainerJsonType)
            ContainerJsonType = (bool)(source["makeObject"] ?? false) ? JtContainerType.Block : JtContainerType.Array;
    }
    internal JtArrayNode(IJtNodeParent parent, JtArrayNodeSource source, JToken? @override) : base(parent, source, @override)
    {
        Prefabs = source.Prefabs.CreateInstance(this, @override?["prefabs"]);
        if (@override is null)
            return;
        singleType = (bool?)@override["singleType"];
        maxSize = (int?)@override["maxSize"];
    }
    public override JtNodeSource CreateSource() => currentSource ??= new JtArrayNodeSource(this);
    internal override void BuildJson(JsonBuilder jb)
    {
        BuildCommonJson(jb);

        if (Base is not null)
        {
            if (Children.IsOverridden())
            {
                jb.AddProperty("prefabs", Prefabs);
            }

            if (MaxSize != Base.MaxSize)
                jb.AddProperty("maxSize", MaxSize);
            if (SingleType != Base.SingleType)
                jb.AddProperty("singleType", SingleType);
            jb.EndBlock();
            return;
        }



        if (MaxSize >= 0)
            jb.AddProperty("maxSize", MaxSize);
        if (SingleType)
            jb.AddProperty("singleType", SingleType);

        jb.AddProperty("prefabs", Prefabs);

        jb.EndBlock();
    }
}