using System.ComponentModel;
using System.Text;
using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using ValueType = System.Int16;

namespace Aadev.JTF.CustomSources.Nodes;

public sealed class JtShortNodeSource : JtValueNodeSource
{
    private IJtSuggestionCollectionSource? suggestions;
    public override JtNodeType Type => JtNodeType.Short;

    [DefaultValue(ValueType.MaxValue)] public ValueType Max { get; set; }
    [DefaultValue(ValueType.MinValue)] public ValueType Min { get; set; }
    [DefaultValue(0)] public ValueType Default { get; set; }
    public override IJtSuggestionCollectionSource Suggestions => suggestions ??= JtSuggestionCollectionSource<ValueType>.Create(this);


    public override JTokenType JsonType => JTokenType.Integer;

    internal static JtShortNodeSource CreateSelf(IJtNodeSourceParent parent) => new JtShortNodeSource(parent);
    internal static JtShortNodeSource CreateSelf(IJtNodeSourceParent parent, JObject source) => new JtShortNodeSource(parent, source);
    public JtShortNodeSource(IJtNodeSourceParent parent) : base(parent)
    {
        Max = ValueType.MaxValue;
        Min = ValueType.MinValue;
    }
    internal JtShortNodeSource(JtShortNode node) : base(node)
    {
        Max = node.Max;
        Min = node.Min;
        Default = node.Default;
        suggestions = node.TryGetSuggestions()?.CreateSource(this);
    }
    internal JtShortNodeSource(IJtNodeSourceParent parent, JObject source) : base(parent, source)
    {
        Min = (ValueType)(source["min"] ?? ValueType.MinValue);
        Max = (ValueType)(source["max"] ?? ValueType.MaxValue);
        Default = (ValueType)(source["default"] ?? 0);
        suggestions = JtSuggestionCollectionSource<ValueType>.TryCreate(this, source["suggestions"]);
    }
    internal JtShortNodeSource(IJtNodeSourceParent parent, JtShortNodeSource @base, JObject? @override) : base(parent, @base, @override)
    {
        Min = (ValueType)(@override?["minLength"] ?? @base.Min);
        Max = (ValueType)(@override?["maxLength"] ?? @base.Max);
        Default = (ValueType)(@override?["default"] ?? @base.Default);
        suggestions = @base.Suggestions;
    }


    internal override void BuildJsonDeclaration(StringBuilder sb)
    {
        BuildCommonJson(sb);
        if (Max != ValueType.MaxValue)
            sb.Append($", \"max\": {Max}");
        if (Min != ValueType.MinValue)
            sb.Append($", \"min\": {Min}");
        if (Default != 0)
            sb.Append($", \"default\": {Default}");
        if (!Suggestions.IsEmpty)
        {
            sb.Append(", \"suggestions\": ");
            Suggestions.BuildJson(sb);
        }

        sb.Append('}');
    }
    public override JtNode CreateInstance(IJtNodeParent parent, JToken? @override) => new JtShortNode(parent, this, @override);
    public override JtNodeSource CreateOverride(IJtNodeSourceParent parent, JObject? @override) => new JtShortNodeSource(parent, this, @override);
    public override JToken CreateDefaultValue() => new JValue(Default);
    internal override IJtSuggestionCollectionSource? TryGetSuggestions() => suggestions;
}