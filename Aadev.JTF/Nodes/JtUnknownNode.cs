using Aadev.JTF.CustomSources;
using Aadev.JTF.CustomSources.Nodes;
using Aadev.JTF.Tools;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.Nodes;

public sealed class JtUnknownNode : JtNode
{
    public override JTokenType JsonType => JTokenType.Undefined;
    public override JtNodeType Type => JtNodeType.Unknown;

    internal static JtUnknownNode CreateSelf(IJtNodeParent parent) => new JtUnknownNode(parent);
    internal static JtUnknownNode CreateSelf(IJtNodeParent parent, JObject source) => new JtUnknownNode(parent, source);
    public JtUnknownNode(IJtNodeParent parent) : base(parent)
    {

    }
    internal JtUnknownNode(IJtNodeParent parent, JObject source) : base(parent, source)
    {

    }

    internal JtUnknownNode(IJtNodeParent parent, JtUnknownNodeSource source, JToken? @override) : base(parent, source, @override)
    {

    }


    public override JToken CreateDefaultValue() => JValue.CreateUndefined();
    public override JtNodeSource CreateSource() => currentSource ??= new JtUnknownNodeSource(this);
    internal override void BuildJson(JsonBuilder jb)
    {
        BuildCommonJson(jb);
        jb.EndBlock();
    }
}