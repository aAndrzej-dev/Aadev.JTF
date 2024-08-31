using System.ComponentModel;
using Aadev.JTF.Nodes;
using Aadev.JTF.Tools;
using Newtonsoft.Json.Linq;
using ValueType = System.Single;

namespace Aadev.JTF.CustomSources.Nodes;

public sealed class JtFloatNodeSource : JtValueNodeSource
{
    private IJtSuggestionCollectionSource? suggestions;

    public override JtNodeType Type => JtNodeType.Float;

    [DefaultValue(ValueType.MaxValue)] public ValueType Max { get; set; }
    [DefaultValue(ValueType.MinValue)] public ValueType Min { get; set; }
    [DefaultValue(0)] public ValueType Default { get; set; }
    public override IJtSuggestionCollectionSource Suggestions => suggestions ??= JtSuggestionCollectionSource<ValueType>.Create(this);

    public override JTokenType JsonType => JTokenType.Float;

    internal static JtFloatNodeSource CreateSelf(IJtNodeSourceParent parent) => new JtFloatNodeSource(parent);
    internal static JtFloatNodeSource CreateSelf(IJtNodeSourceParent parent, JObject source) => new JtFloatNodeSource(parent, source);
    public JtFloatNodeSource(IJtNodeSourceParent parent) : base(parent)
    {
        Max = ValueType.MaxValue;
        Min = ValueType.MinValue;
    }
    internal JtFloatNodeSource(JtFloatNode node) : base(node)
    {
        Max = node.Max;
        Min = node.Min;
        Default = node.Default;
        suggestions = node.TryGetSuggestions()?.CreateSource(this);
    }
    internal JtFloatNodeSource(IJtNodeSourceParent parent, JObject source) : base(parent, source)
    {
        Min = (ValueType)(source["min"] ?? ValueType.MinValue);
        Max = (ValueType)(source["max"] ?? ValueType.MaxValue);
        Default = (ValueType)(source["default"] ?? 0);
        suggestions = JtSuggestionCollectionSource<ValueType>.TryCreate(this, source["suggestions"]);
    }
    internal JtFloatNodeSource(IJtNodeSourceParent parent, JtFloatNodeSource @base, JObject? @override) : base(parent, @base, @override)
    {
        Min = (ValueType)(@override?["minLength"] ?? @base.Min);
        Max = (ValueType)(@override?["maxLength"] ?? @base.Max);
        Default = (ValueType)(@override?["default"] ?? @base.Default);
        suggestions = @base.Suggestions;
    }
    internal override void BuildJsonDeclaration(JsonBuilder jb)
    {
        BuildCommonJson(jb);
        if (Max != ValueType.MaxValue)
            jb.AddProperty("max", Max);
        if (Min != ValueType.MinValue)
            jb.AddProperty("min", Min);
        if (Default != 0)
            jb.AddProperty("default", Default);
        if (!Suggestions.IsEmpty)
            jb.AddProperty("suggestions", Suggestions);

        jb.EndBlock();
    }
    public override JtNode CreateInstance(IJtNodeParent parent, JToken? @override) => new JtFloatNode(parent, this, @override);
    public override JtNodeSource CreateOverride(IJtNodeSourceParent parent, JObject? @override) => new JtFloatNodeSource(parent, this, @override);
    public override JToken CreateDefaultValue() => new JValue(Default);
    internal override IJtSuggestionCollectionSource? TryGetSuggestions() => suggestions;
}