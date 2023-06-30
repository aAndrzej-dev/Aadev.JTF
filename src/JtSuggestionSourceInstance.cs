using System;
using System.Collections.Generic;
using System.Text;
using Aadev.JTF.Common;
using Aadev.JTF.CustomSources;

namespace Aadev.JTF;

public sealed class JtSuggestionSourceInstance<TSuggestion> : IJtSuggestion<TSuggestion>, IEquatable<JtSuggestionSourceInstance<TSuggestion>?>
{
    private readonly JtSuggestionSource<TSuggestion> source;

    internal JtSuggestionSourceInstance(JtSuggestionSource<TSuggestion> source)
    {
        this.source = source;
    }

    public Type SuggestionType => source.SuggestionType;

    public string? DisplayName { get => source.DisplayName; set => throw new NotSupportedException(); }

    public string? StringValue => source.StringValue;

    public TSuggestion Value { get => source.Value; set => throw new NotSupportedException(); }


    bool IJtSuggestionCollectionChild<TSuggestion>.IsStatic => true;

    public bool IsReadOnly => true;

    public override string? ToString() => DisplayName ?? StringValue;
    public override bool Equals(object? obj) => Equals(obj as JtSuggestionSourceInstance<TSuggestion>);
    public bool Equals(JtSuggestionSourceInstance<TSuggestion>? other) => other is not null && EqualityComparer<JtSuggestionSource<TSuggestion>>.Default.Equals(source, other.source);
    public override int GetHashCode() => HashCode.Combine(source);
    T IJtCommonSuggestion.GetValue<T>() => ((IJtSuggestionSource)source).GetValue<T>();
    object? IJtCommonSuggestion.GetValue() => ((IJtSuggestionSource)source).GetValue();
    void IJtCommonSuggestion.SetValue<T>(T value) => throw new NotSupportedException();
    void IJtCommonSuggestion.SetValue(object? value) => throw new NotSupportedException();
    void IJtJsonBuildable.BuildJson(StringBuilder sb) => throw new NotSupportedException();
    IJtSuggestionCollectionSourceChild<TSuggestion> IJtSuggestionCollectionChild<TSuggestion>.CreateSource(IJtCustomSourceParent parent) => source;
    IEnumerable<IJtSuggestion> IJtSuggestionCollectionChild<TSuggestion>.GetSuggestions(Func<JtIdentifier, IEnumerable<IJtSuggestion>> dynamicSuggestionsSource)
    {
        yield return this;
    }


    public static bool operator ==(JtSuggestionSourceInstance<TSuggestion>? left, JtSuggestionSourceInstance<TSuggestion>? right) => EqualityComparer<JtSuggestionSourceInstance<TSuggestion>>.Default.Equals(left, right);
    public static bool operator !=(JtSuggestionSourceInstance<TSuggestion>? left, JtSuggestionSourceInstance<TSuggestion>? right) => !(left == right);
}
