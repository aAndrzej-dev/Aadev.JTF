using System;
using System.Globalization;
using System.Text;
using Aadev.JTF.Common;
using Aadev.JTF.Types;
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
    internal void BuildJson(StringBuilder sb)
    {
        sb.Append('{');
        if (Value is string str)
            sb.Append($"\"value\": \"{str}\"");
        else if (Value is byte or short or int or long or float or double)
#if NET6_0_OR_GREATER
            sb.Append(CultureInfo.InvariantCulture, $"\"value\": {Value}");
#else
            sb.Append($"\"value\": {Value}");
#endif
        else if (Value is bool b)
            sb.Append($"\"value\": {(b ? "true" : "false")}");
        if (DisplayName?.Equals(Value?.ToString(), StringComparison.Ordinal) is false)
            sb.Append($", \"displayName\": \"{DisplayName}\"");
        sb.Append('}');
    }

    IJtSuggestionCollectionChild<TSuggestion> IJtSuggestionCollectionSourceChild<TSuggestion>.CreateInstance(JtValueNode owenr) => CreateInstance();
    T IJtCommonSuggestion.GetValue<T>()
    {
        if (Value is T v)
            return v;
        throw new InvalidCastException($"Cannot convert {typeof(T)} to {typeof(TSuggestion)}");
    }
    void IJtCommonSuggestion.SetValue<T>(T value)
    {
        if (value is TSuggestion v)
            Value = v;
        throw new InvalidCastException($"Cannot convert {typeof(T)} to {typeof(TSuggestion)}");
    }

    object? IJtCommonSuggestion.GetValue() => Value;
    void IJtCommonSuggestion.SetValue(object? value)
    {
        if (value is TSuggestion v)
            Value = v;
        throw new InvalidCastException($"Cannot convert {value?.GetType()} to {typeof(TSuggestion)}");
    }

    void IJtJsonBuildable.BuildJson(StringBuilder sb) => BuildJson(sb);
}
