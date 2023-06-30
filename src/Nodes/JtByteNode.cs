using System.ComponentModel;
using System.Text;
using Aadev.JTF.CustomSources;
using Aadev.JTF.CustomSources.Nodes;
using Newtonsoft.Json.Linq;
using ValueType = System.Byte;

namespace Aadev.JTF.Types;

public sealed class JtByteNode : JtValueNode
{
    private const ValueType minValue = ValueType.MinValue;
    private const ValueType maxValue = ValueType.MaxValue;
    private IJtSuggestionCollection? suggestions;
    private ValueType? @default;
    private ValueType? min;
    private ValueType? max;


    public new JtByteNodeSource? Base => (JtByteNodeSource?)base.Base;
    public override JTokenType JsonType => JTokenType.Integer;
    public override JtNodeType Type => JtNodeType.Byte;



    [DefaultValue(minValue), RefreshProperties(RefreshProperties.All)] public ValueType Min { get => min ?? Base?.Min ?? minValue; set { min = value; max = max.Max(min); @default = @default.Clamp(Min, Max); } }
    [DefaultValue(maxValue), RefreshProperties(RefreshProperties.All)] public ValueType Max { get => max ?? Base?.Max ?? maxValue; set { max = value; min = min.Min(max); @default = @default.Clamp(Min, Max); } }
    [DefaultValue(0)] public ValueType Default { get => @default ?? Base?.Default ?? 0; set => @default = value.Clamp(Min, Max); }

    public override IJtSuggestionCollection Suggestions => suggestions ??= JtSuggestionCollection<ValueType>.Create(this);

    internal static JtByteNode CreateSelf(IJtNodeParent parent) => new JtByteNode(parent);
    internal static JtByteNode CreateSelf(IJtNodeParent parent, JObject source) => new JtByteNode(parent, source);
    public JtByteNode(IJtNodeParent parent) : base(parent)
    {
        Min = minValue;
        Max = maxValue;
        Default = 0;
    }
    internal JtByteNode(IJtNodeParent parent, JObject source) : base(parent, source)
    {
        Min = (ValueType)(source["min"] ?? minValue);
        Max = (ValueType)(source["max"] ?? maxValue);
        Default = (ValueType)(source["default"] ?? 0);

        suggestions = JtSuggestionCollection<ValueType>.TryCreate(this, source["suggestions"]);
    }
    internal JtByteNode(IJtNodeParent parent, JtByteNodeSource source, JToken? @override) : base(parent, source, @override)
    {
        suggestions = source.TryGetSuggestions()?.CreateInstance(this);
        if (@override is null)
            return;
        min = (ValueType?)@override["min"];
        max = (ValueType?)@override["max"];
        @default = (ValueType?)@override["default"];

    }

    internal override void BuildJson(StringBuilder sb)
    {
        BuildCommonJson(sb);

        if (Min != minValue)
            sb.Append($", \"min\": {Min}");
        if (Max != maxValue)
            sb.Append($", \"max\": {Max}");
        if (Default != 0)
            sb.Append($", \"default\": {Default}");
        sb.Append('}');
    }
    public override JToken CreateDefaultValue() => new JValue(Default);
    public override object GetDefaultValue() => Default;
    public override JtNodeSource CreateSource() => currentSource ??= new JtByteNodeSource(this);
    public override string? GetDisplayString(JToken? value)
    {
        if (value is null or not JValue)
            return null;
        ValueType? val = (ValueType?)value;
        if (val is null)
            return null;
        if (val == Default)
        {
            return $"Default ({val})";
        }

        if (val == Max)
        {
            return $"Max ({val})";
        }

        if (val == Min)
        {
            return $"Min ({val})";
        }

        return val.ToString();
    }

    public override IJtSuggestionCollection? TryGetSuggestions() => suggestions;
}