using System;
using System.Collections.Generic;
using System.Globalization;
using Aadev.JTF.Common;
using Aadev.JTF.CustomSources;
using Aadev.JTF.Tools;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF;

[Serializable]
public sealed class JtSuggestion<TSuggestion> : IJtSuggestion<TSuggestion>, IEquatable<JtSuggestion<TSuggestion>?>
{
    private TSuggestion value;

    public TSuggestion Value
    {
        get => value;
        set
        {
            if (DisplayName?.Equals(Value?.ToString(), StringComparison.Ordinal) is true)
                DisplayName = value?.ToString();
            this.value = value;
        }
    }
    public string? DisplayName { get; set; }

    public Type SuggestionType => typeof(TSuggestion);


    public string? StringValue => Value?.ToString();

    bool IJtSuggestionCollectionChild<TSuggestion>.IsStatic => true;
    bool IJtSuggestionCollectionChild<TSuggestion>.IsReadOnly => false;
    public JtSuggestion(TSuggestion value, string? displayName = null)
    {
        this.value = value;
        DisplayName = displayName ?? value?.ToString();
    }
    public JtSuggestion(JObject source)
    {

        if (source is null)
            throw new ArgumentNullException(nameof(source));
        if (source["name"]?.Type is JTokenType.String && typeof(TSuggestion) == typeof(string))
            value = (TSuggestion)((JValue?)source["name"])?.Value!;
        else
            value = (TSuggestion)Convert.ChangeType(((JValue?)source["value"])?.Value, typeof(TSuggestion), CultureInfo.InvariantCulture)!;
        DisplayName = (string?)source["displayName"] ?? value?.ToString();

    }
    public override string? ToString() => DisplayName ?? StringValue;

    public override bool Equals(object? obj) => Equals(obj as JtSuggestion<TSuggestion>);
    public bool Equals(JtSuggestion<TSuggestion>? other) => other is not null && EqualityComparer<TSuggestion>.Default.Equals(Value, other.Value) && DisplayName == other.DisplayName;
    public override int GetHashCode() => HashCode.Combine(Value, DisplayName);

    public JtSuggestionSource<TSuggestion> CreateSource() => new JtSuggestionSource<TSuggestion>(this);

    T IJtCommonSuggestion.GetValue<T>()
    {
        if (Value is not T v)
            throw new InvalidCastException($"Cannot cast '{nameof(value)}' from '{(value?.GetType()?.ToString() ?? "null")}' to {typeof(TSuggestion)}");
        return v;
    }
    void IJtCommonSuggestion.SetValue<T>(T value)
    {
        if (value is not TSuggestion v)
            throw new InvalidCastException($"Cannot cast '{nameof(value)}' from '{(value?.GetType()?.ToString() ?? "null")}' to {typeof(TSuggestion)}");
        Value = v;
    }
    object? IJtCommonSuggestion.GetValue() => Value;
    void IJtCommonSuggestion.SetValue(object? value)
    {
        if (value is not TSuggestion v)
            throw new InvalidCastException($"Cannot cast '{nameof(value)}' from '{(value?.GetType()?.ToString() ?? "null")}' to {typeof(TSuggestion)}");
        Value = v;
    }
    IEnumerable<IJtSuggestion> IJtSuggestionCollectionChild<TSuggestion>.GetSuggestions(Func<JtIdentifier, IEnumerable<IJtSuggestion>> dynamicSuggestionsSource)
    {
        yield return this;
    }
    IJtSuggestionCollectionSourceChild<TSuggestion> IJtSuggestionCollectionChild<TSuggestion>.CreateSource(IJtCustomSourceParent parent) => CreateSource();
    void IJsonBuildable.BuildJson(JsonBuilder jb)
    {
        jb.StartBlock();

        jb.AddTProperty("value", Value);
        if (DisplayName?.Equals(Value?.ToString(), StringComparison.Ordinal) is false)
            jb.AddProperty("displayName", DisplayName);
        jb.EndBlock();
    }


    public static bool operator ==(JtSuggestion<TSuggestion>? left, JtSuggestion<TSuggestion>? right) => !(left is null || right is null) && EqualityComparer<JtSuggestion<TSuggestion>>.Default.Equals(left, right);
    public static bool operator !=(JtSuggestion<TSuggestion>? left, JtSuggestion<TSuggestion>? right) => !(left == right);
}
