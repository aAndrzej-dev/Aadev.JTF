using System.ComponentModel;
using System.Text;
using Aadev.JTF.CustomSources;
using Aadev.JTF.CustomSources.Nodes;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.Types;

public sealed class JtBoolNode : JtNode
{
    private bool? @default;
    private bool? constant;

    public new JtBoolNodeSource? Base => (JtBoolNodeSource?)base.Base;
    public override JTokenType JsonType => JTokenType.Boolean;
    public override JtNodeType Type => JtNodeType.Bool;

    [DefaultValue(false)] public bool Default { get => @default ?? Base?.Default ?? false; set => @default = value; }
    [DefaultValue(false)] public bool Constant { get => constant ?? Base?.Constant ?? false; set => constant = value; }
    internal static JtBoolNode CreateSelf(IJtNodeParent parent) => new JtBoolNode(parent);
    internal static JtBoolNode CreateSelf(IJtNodeParent parent, JObject source) => new JtBoolNode(parent, source);
    public JtBoolNode(IJtNodeParent parent) : base(parent)
    {
    }
    internal JtBoolNode(IJtNodeParent parent, JObject source) : base(parent, source)
    {
        Default = (bool?)source["default"] ?? false;
        Constant = (bool?)source["constant"] ?? false;
    }
    internal JtBoolNode(IJtNodeParent parent, JtBoolNodeSource source, JToken? @override) : base(parent, source, @override)
    {
        if (@override is null)
            return;
        @default = (bool?)@override["default"];
        constant = (bool?)@override["constant"];
    }

    internal override void BuildJson(StringBuilder sb)
    {
        BuildCommonJson(sb);

        if (Default)
            sb.Append($", \"default\": true");
        if (Constant)
            sb.Append(", \"constant\": true");
        sb.Append('}');
    }
    public override JToken CreateDefaultValue() => new JValue(Default);
    public override JtNodeSource CreateSource() => currentSource ??= new JtBoolNodeSource(this);
}