using System.ComponentModel;
using Aadev.JTF.Nodes;
using Aadev.JTF.Tools;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.CustomSources.Nodes;

public sealed class JtBoolNodeSource : JtNodeSource
{
    public override JtNodeType Type => JtNodeType.Bool;

    [DefaultValue(false)] public bool Default { get; set; }
    [DefaultValue(false)] public bool Constant { get; set; }

    public override JTokenType JsonType => JTokenType.Boolean;

    internal static JtBoolNodeSource CreateSelf(IJtNodeSourceParent parent) => new JtBoolNodeSource(parent);
    internal static JtBoolNodeSource CreateSelf(IJtNodeSourceParent parent, JObject source) => new JtBoolNodeSource(parent, source);
    public JtBoolNodeSource(IJtNodeSourceParent parent) : base(parent) { }
    internal JtBoolNodeSource(JtBoolNode node) : base(node)
    {
        Default = node.Default;
        Constant = node.Constant;
    }
    internal JtBoolNodeSource(IJtNodeSourceParent parent, JObject source) : base(parent, source)
    {
        Default = (bool?)source["default"] ?? false;
        Constant = (bool?)source["constant"] ?? false;
    }
    internal JtBoolNodeSource(IJtNodeSourceParent parent, JtBoolNodeSource @base, JObject? @override) : base(parent, @base, @override)
    {
        Default = (bool)(@override?["default"] ?? @base.Default);
        Constant = (bool)(@override?["constant"] ?? @base.Constant);
    }





    internal override void BuildJsonDeclaration(JsonBuilder jb)
    {
        BuildCommonJson(jb);
        if (Default)
            jb.AddProperty("default", true);
        if (Constant)
            jb.AddProperty("constant", true);
        jb.EndBlock();
    }
    public override JtNode CreateInstance(IJtNodeParent parent, JToken? @override) => new JtBoolNode(parent, this, @override);
    public override JtNodeSource CreateOverride(IJtNodeSourceParent parent, JObject? @override) => new JtBoolNodeSource(parent, this, @override);
    public override JToken CreateDefaultValue() => new JValue(Default);
}