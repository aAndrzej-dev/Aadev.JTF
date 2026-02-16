using System;
using System.Globalization;
using Aadev.JTF.Common;
using Aadev.JTF.Nodes;
using Aadev.JTF.Tools;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.CustomSources;

public sealed class JtSuggestionSource<TSuggestion> : IJtSuggestionSource<TSuggestion>
{
    private JtSuggestionSourceInstance<TSuggestion>? instance;

    public TSuggestion Value { get; set; }
    public string? DisplayName { get; set; }

    public Type SuggestionType => typeof(TSuggestion);

    public string? StringValue => Value?.ToString();


    internal JtSuggestionSource(JObject source)
    {
        if (source["name"]?.Type is JTokenType.String && typeof(TSuggestion) == typeof(string))
            Value = (TSuggestion)((JValue?)source["name"])?.Value!;
        else
            Value = (TSuggestion)Convert.ChangeType(((JValue?)source["value"])?.Value, typeof(TSuggestion), CultureInfo.InvariantCulture)!;
        DisplayName = (string?)source["displayName"] ?? Value?.ToString();
    }

    internal JtSuggestionSource(JtSuggestion<TSuggestion> jtSuggestion)
    {
        Value = jtSuggestion.Value;
        DisplayName = jtSuggestion.DisplayName;
    }

    internal JtSuggestionSource(TSuggestion value, string displayName)
    {
        Value = value;
        DisplayName = displayName;
    }

    public override string? ToString() => DisplayName ?? StringValue;
    public IJtSuggestion<TSuggestion> CreateInstance() => instance ??= new JtSuggestionSourceInstance<TSuggestion>(this);

    IJtSuggestionCollectionChild<TSuggestion> IJtSuggestionCollectionSourceChild<TSuggestion>.CreateInstance(JtValueNode owenr) => CreateInstance();
    T IJtCommonSuggestion.GetValue<T>()
    {
        if (Value is T v)
            return v;

        throw new InvalidCastException($"Cannot cast from '{typeof(T)}' to {typeof(TSuggestion)}");
    }
    void IJtCommonSuggestion.SetValue<T>(T value)
    {
        if (value is TSuggestion v)
            Value = v;
        throw new InvalidCastException($"Cannot cast '{nameof(value)}' from '{typeof(T)}' to {typeof(TSuggestion)}");
    }

    object? IJtCommonSuggestion.GetValue() => Value;
    void IJtCommonSuggestion.SetValue(object? value)
    {
        if (value is TSuggestion v)
            Value = v;
        throw new InvalidCastException($"Cannot cast '{nameof(value)}' from '{value?.GetType()?.ToString() ?? "null"}' to {typeof(TSuggestion)}");
    }

    void IJsonBuildable.BuildJson(JsonBuilder jb)
    {
        jb.StartBlock();
        jb.AddTProperty("value", Value);
        if (DisplayName?.Equals(Value?.ToString(), StringComparison.Ordinal) is false)
            jb.AddProperty("displayName", DisplayName);
        jb.EndBlock();
    }
}
