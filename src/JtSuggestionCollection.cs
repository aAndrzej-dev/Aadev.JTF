using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Aadev.JTF.Common;
using Aadev.JTF.CustomSources;
using Aadev.JTF.CustomSources.Nodes;
using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF;
[Editor("Aadev.JTF.Design.JtSuggestionCollectionEditor, Aadev.JTF.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4bb879fd89b07a65", $"System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
public sealed class JtSuggestionCollection<TSuggestion> : IJtSuggestionCollection<TSuggestion>
{
    private List<IJtSuggestionCollectionChild<TSuggestion>>? suggestions;
    private JtSourceReference dynamicSourceId;
    private JtSuggestionCollectionSource<TSuggestion>? @base;
    private JArray? jsonSource;

    public JtValueNode Owner { get; }
    public JtSuggestionCollectionSource<TSuggestion>? Base { get => @base; set { @base = value; suggestions = null; } }


    IJtSuggestionCollectionSource? IJtCommonSuggestionCollection.Base { get => Base; set => Base = (JtSuggestionCollectionSource<TSuggestion>?)value; }
    List<IJtCommonSuggestionCollectionChild> IJtCommonSuggestionCollection.Suggestions
    {
        get
        {
            EnsureSuggestions();
            return Unsafe.As<List<IJtSuggestionCollectionChild<TSuggestion>>, List<IJtCommonSuggestionCollectionChild>>(ref suggestions);
        }
    }
    public JtSourceReference DynamicSourceId
    {
        get => dynamicSourceId;
        set
        {
            if (value.IsEmpty)
            {
                dynamicSourceId = JtSourceReference.Empty;
                return;
            }

            if (value.Type is not JtSourceReferenceType.Dynamic)
                throw new JtfException("Cannot set non-dynamic id to a suggestion collection. Use custom sources instead.");
            dynamicSourceId = value;
        }
    }


    [MemberNotNull(nameof(suggestions))]
    private List<IJtSuggestionCollectionChild<TSuggestion>> Suggestions
    {
        get
        {
            EnsureSuggestions();
            return suggestions;
        }
    }
    [MemberNotNull(nameof(suggestions))]
    private void EnsureSuggestions()
    {
        if (suggestions is null)
        {
            if (Base is not null)
            {
                suggestions = new List<IJtSuggestionCollectionChild<TSuggestion>>(Base.Count);

                for (int i = 0; i < Base.Count; i++)
                {
                    suggestions.Add(Base[i].CreateInstance(Owner));
                }
            }
            else if (jsonSource is not null)
            {
                suggestions = new List<IJtSuggestionCollectionChild<TSuggestion>>(jsonSource.Count);

                for (int i = 0; i < jsonSource.Count; i++)
                {
                    suggestions.Add(CreateSuggestionItem(jsonSource[i]));
                }
            }
            else
                suggestions = new List<IJtSuggestionCollectionChild<TSuggestion>>();
        }
    }

    private IJtSuggestionCollectionChild<TSuggestion> CreateSuggestionItem(JToken source)
    {
        if (source?.Type is JTokenType.Array || source?.Type is JTokenType.String)
            return JtSuggestionCollection<TSuggestion>.Create(Owner, source);
        if (source?.Type is JTokenType.Object)
            return new JtSuggestion<TSuggestion>((JObject)source);
        return new JtSuggestion<TSuggestion>(default!, "Unknown");
    }




    public int GetCountWithoutEnumeration()
    {
        if (suggestions is not null)
            return suggestions.Count;
        if (Base is not null)
            return Base.Count;
        if (jsonSource is not null)
            return jsonSource.Count;
        return 0;
    }


    public Type SuggestionType => typeof(TSuggestion);

    private JtSuggestionCollection(JtValueNode owner)
    {
        Owner = owner;
    }
    private JtSuggestionCollection(JtValueNode owner, JArray? source)
    {
        Owner = owner;
        jsonSource = source;
    }
    private JtSuggestionCollection(JtValueNode owner, JtSourceReference id)
    {
        DynamicSourceId = id;
        Owner = owner;
    }
    private JtSuggestionCollection(JtValueNode owner, JtSuggestionCollectionSource<TSuggestion> source)
    {
        Base = source;
        Owner = owner;
    }
    public bool IsStatic => DynamicSourceId.IsEmpty && Suggestions.All(x => x.IsStatic);

    public static JtSuggestionCollection<TSuggestion> Create(JtValueNode owner) => new JtSuggestionCollection<TSuggestion>(owner);

    internal static JtSuggestionCollection<TSuggestion>? TryCreate(JtValueNode owner, JToken? value)
    {
        if (owner is null)
            throw new ArgumentNullException(nameof(owner));
        if (value?.Type is JTokenType.String)
        {
            JtSourceReference id = (string?)value;
            switch (id.Type)
            {

                case JtSourceReferenceType.Dynamic:
                    return new JtSuggestionCollection<TSuggestion>(owner, id);
                case JtSourceReferenceType.External:
                    return owner.GetCustomSource<JtSuggestionCollectionSource<TSuggestion>>(id)?.CreateInstance(owner) ?? new JtSuggestionCollection<TSuggestion>(owner);
                case JtSourceReferenceType.Direct:
                    {
                        JtValueNodeSource? element = owner.GetCustomSource<JtValueNodeSource>(id);
                        if (element is null || element.Suggestions.SuggestionType != typeof(TSuggestion))
                            return new JtSuggestionCollection<TSuggestion>(owner);

                        return (JtSuggestionCollection<TSuggestion>)element.Suggestions.CreateInstance(owner);
                    }
                default:
                    return null;
            }
        }

        if (value?.Type is JTokenType.Array)
        {
            return new JtSuggestionCollection<TSuggestion>(owner, (JArray)value);
        }

        return null;
    }
    public static JtSuggestionCollection<TSuggestion> Create(JtValueNode owner, JToken? value) => TryCreate(owner, value) ?? new JtSuggestionCollection<TSuggestion>(owner);
    internal static JtSuggestionCollection<TSuggestion> Create(JtValueNode owner, JtSuggestionCollectionSource<TSuggestion> source) => new JtSuggestionCollection<TSuggestion>(owner, source);
    internal static JtSuggestionCollection<TSuggestion> Create(JtValueNode owner, JtSourceReference id) => new JtSuggestionCollection<TSuggestion>(owner, id);


    public IEnumerable<IJtSuggestion> GetSuggestions(Func<JtIdentifier, IEnumerable<IJtSuggestion>> dynamicSuggestionsSource)
    {
        if (!DynamicSourceId.IsEmpty)
            return dynamicSuggestionsSource?.Invoke(DynamicSourceId.Identifier)?.Where(x => x.SuggestionType == SuggestionType) ?? Enumerable.Empty<IJtSuggestion>();

        return Suggestions.SelectMany(x => x.GetSuggestions(dynamicSuggestionsSource)).Distinct();
    }
    internal void BuildJson(StringBuilder sb)
    {
        if (Base?.IsDeclared is true)
        {
            sb.Append($"\"{Base.Declaration.Name}\"");
        }
        else if (DynamicSourceId.IsEmpty)
        {
            sb.Append('[');
#if NET5_0_OR_GREATER
            Span<IJtSuggestionCollectionChild<TSuggestion>> listSpan = CollectionsMarshal.AsSpan(Suggestions);
            for (int i = 0; i < listSpan.Length; i++)
            {
                if (i > 0)
                    sb.Append(',');
                listSpan[i].BuildJson(sb);
            }
#else
            for (int i = 0; i < Suggestions.Count; i++)
            {
                if (i > 0)
                    sb.Append(',');
                Suggestions[i].BuildJson(sb);
            }
#endif
            sb.Append(']');
        }
        else
        {
            sb.Append($"\"{DynamicSourceId}\"");
        }
    }



    internal JtSuggestionCollectionSource<TSuggestion> CreateSource(IJtCustomSourceParent parent) => Base is null ? JtSuggestionCollectionSource<TSuggestion>.Create(parent, this) : Base;
    IJtSuggestionCollectionSourceChild<TSuggestion> IJtSuggestionCollectionChild<TSuggestion>.CreateSource(IJtCustomSourceParent parent) => CreateSource(parent);
    IJtSuggestionCollectionSource IJtSuggestionCollection.CreateSource(IJtCustomSourceParent parent) => CreateSource(parent);
    void IJtJsonBuildable.BuildJson(StringBuilder sb) => BuildJson(sb);

    public bool IsEmpty => GetCountWithoutEnumeration() == 0 && DynamicSourceId.IsEmpty;

    public int IndexOf(IJtSuggestionCollectionChild<TSuggestion> item) => Suggestions.IndexOf(item);
    public void Insert(int index, IJtSuggestionCollectionChild<TSuggestion> item) { ThrowIfReadOnly(); Suggestions.Insert(index, item); }
    public void RemoveAt(int index) { ThrowIfReadOnly(); Suggestions.RemoveAt(index); }
    public void Add(IJtSuggestionCollectionChild<TSuggestion> item) { ThrowIfReadOnly(); Suggestions.Add(item); }
    public void Clear()
    {
        ThrowIfReadOnly();

        if (suggestions is null)
        {
            jsonSource = null;
            return;
        }

        Suggestions.Clear();
    }
    public bool Contains(IJtSuggestionCollectionChild<TSuggestion> item) => Suggestions.Contains(item);
    public void CopyTo(IJtSuggestionCollectionChild<TSuggestion>[] array, int arrayIndex) => Suggestions.CopyTo(array, arrayIndex);
    public bool Remove(IJtSuggestionCollectionChild<TSuggestion> item) { ThrowIfReadOnly(); return Suggestions.Remove(item); }
    public IEnumerator<IJtSuggestionCollectionChild<TSuggestion>> GetEnumerator() => Suggestions.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Suggestions.GetEnumerator();



    public int Count => Suggestions.Count;

    public bool IsReadOnly => Base is not null && !dynamicSourceId.IsEmpty;

    public ICustomSourceProvider SourceProvider => Owner;

    public IdentifiersManager IdentifiersManager => Owner.IdentifiersManager;

    public IJtCommonRoot Root => Owner.Template;

    private void ThrowIfReadOnly()
    {
        if (IsReadOnly)
            throw new ReadOnlyException("Suggestion collection based on suggestion collection source can not be edited.");
    }

    public IJtCommonSuggestion AddNewSuggestion(object? value, string? displayName = null)
    {
        ThrowIfReadOnly();
        if (value is TSuggestion tValue)
        {
            JtSuggestion<TSuggestion> suggestion = new JtSuggestion<TSuggestion>(tValue, displayName);
            Add(suggestion);
            return suggestion;
        }

        throw new InvalidCastException($"Cannot convert {value?.GetType()} to {typeof(TSuggestion)}");
    }
    public IJtCommonSuggestionCollection AddNewSuggestionCollection()
    {
        ThrowIfReadOnly();
        JtSuggestionCollection<TSuggestion> collection = new JtSuggestionCollection<TSuggestion>(Owner);
        Add(collection);
        return collection;
    }

    public IJtSuggestionCollectionChild<TSuggestion> this[int index] { get => Suggestions[index]; set { ThrowIfReadOnly(); Suggestions[index] = value; } }
}
