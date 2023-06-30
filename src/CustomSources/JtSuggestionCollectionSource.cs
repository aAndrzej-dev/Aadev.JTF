using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Aadev.JTF.CollectionBuilders;
using Aadev.JTF.Common;
using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.CustomSources;
[Editor("Aadev.JTF.Design.JtSuggestionCollectionEditor, Aadev.JTF.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4bb879fd89b07a65", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
public sealed class JtSuggestionCollectionSource<TSuggestion> : CustomSource, IJtSuggestionCollectionSource<TSuggestion>
{
    private JtSourceReference dynamicSourceId;
    private IJtSuggestionCollectionSourceBuilder<TSuggestion>? suggestionsBuilder;
    private List<IJtSuggestionCollectionSourceChild<TSuggestion>>? suggestions;
    private JtSuggestionCollectionSource<TSuggestion>? @base;




    [MemberNotNull(nameof(suggestions))]
    internal List<IJtSuggestionCollectionSourceChild<TSuggestion>> Suggestions
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
            if (suggestionsBuilder is not null)
            {
                suggestions = suggestionsBuilder.Build(this);
                suggestionsBuilder = null;
            }
            else if (Base is not null)
            {
                suggestions = Base.Suggestions;
            }
            else
                suggestions = new List<IJtSuggestionCollectionSourceChild<TSuggestion>>();
        }
    }

    public Type SuggestionType => typeof(TSuggestion);

    public override bool IsExternal => false;

    public int Count => Suggestions.Count;

    public bool IsReadOnly => false;

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


    IJtSuggestionCollectionSource? IJtCommonSuggestionCollection.Base { get => Base; set => Base = (JtSuggestionCollectionSource<TSuggestion>?)value; }

    List<IJtCommonSuggestionCollectionChild> IJtCommonSuggestionCollection.Suggestions
    {
        get
        {
            EnsureSuggestions();
            return Unsafe.As<List<IJtSuggestionCollectionSourceChild<TSuggestion>>, List<IJtCommonSuggestionCollectionChild>>(ref suggestions);
        }
    }

    public bool IsEmpty => DynamicSourceId.IsEmpty && Suggestions.Count == 0;

    public JtSuggestionCollectionSource<TSuggestion>? Base { get => @base; set { @base = value; suggestions = null; } }

    IJtCommonRoot IJtCommonSuggestionCollection.Root => Declaration;

    public IJtSuggestionCollectionSourceChild<TSuggestion> this[int index] { get => Suggestions[index]; set => Suggestions[index] = value; }

    private JtSuggestionCollectionSource(IJtCustomSourceParent parent) : base(parent) { }
    private JtSuggestionCollectionSource(IJtCustomSourceParent parent, JArray? source) : base(parent)
    {
        if (source is not null)
            suggestionsBuilder = JtCollectionBuilder.CreateJtSuggestionCollectionSource<TSuggestion>(source);
    }
    private JtSuggestionCollectionSource(IJtCustomSourceParent parent, JtSourceReference id) : base(parent)
    {
        DynamicSourceId = id;
    }
    private JtSuggestionCollectionSource(IJtCustomSourceParent parent, JtSuggestionCollection<TSuggestion> source) : base(parent)
    {
        if (source is not null)
        {
            if (!source.DynamicSourceId.IsEmpty)
                DynamicSourceId = source.DynamicSourceId;
            else
                suggestionsBuilder = JtCollectionBuilder.CreateJtSuggestionCollectionSource<TSuggestion>(source);
        }
    }

    internal override void BuildJsonDeclaration(StringBuilder sb)
    {
        if (!DynamicSourceId.IsEmpty)
        {
            sb.Append($"\"{DynamicSourceId}\"");
        }
        else
        {
            sb.Append('[');
#if NET5_0_OR_GREATER
            Span<IJtSuggestionCollectionSourceChild<TSuggestion>> listSpan = CollectionsMarshal.AsSpan(Suggestions);
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
    }

    public static JtSuggestionCollectionSource<TSuggestion> Create(IJtCustomSourceParent parent) => new JtSuggestionCollectionSource<TSuggestion>(parent);

    /// <summary>
    /// Creates new <see cref="JtSuggestionCollectionSource"/> from JSON value. Returns null if value is null or cannot create collection.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Exception is thrown when <paramref name="parent"/> is <see cref="null"/></exception>
    public static JtSuggestionCollectionSource<TSuggestion>? TryCreate(IJtCustomSourceParent parent, JToken? value)
    {
        if (parent is null)
            throw new ArgumentNullException(nameof(parent));

        if (value?.Type is JTokenType.String)
        {
            JtSourceReference id = (string?)value;
            return id.Type switch
            {
                JtSourceReferenceType.Local or JtSourceReferenceType.Direct or JtSourceReferenceType.Dynamic => new JtSuggestionCollectionSource<TSuggestion>(parent, id),
                JtSourceReferenceType.External => parent.SourceProvider.GetCustomSource<JtSuggestionCollectionSource<TSuggestion>>(id),
                _ => null,
            };
        }

        if (value?.Type is JTokenType.Array)
        {
            return new JtSuggestionCollectionSource<TSuggestion>(parent, (JArray)value);
        }

        return null;
    }

    public static JtSuggestionCollectionSource<TSuggestion> Create(IJtCustomSourceParent parent, JToken? value)
    {
        return TryCreate(parent, value) ?? new JtSuggestionCollectionSource<TSuggestion>(parent);
    }
    internal static JtSuggestionCollectionSource<TSuggestion> Create(IJtCustomSourceParent parent, JtSuggestionCollection<TSuggestion> source) => new JtSuggestionCollectionSource<TSuggestion>(parent, source);
    void IJtJsonBuildable.BuildJson(StringBuilder sb) => BuildJson(sb);
    internal JtSuggestionCollection<TSuggestion> CreateInstance(JtValueNode owner)
    {
        if (dynamicSourceId.IsEmpty)
            return JtSuggestionCollection<TSuggestion>.Create(owner, this);
        else
            return JtSuggestionCollection<TSuggestion>.Create(owner, dynamicSourceId); // For dynamic declaration only
    }
    IJtSuggestionCollection IJtSuggestionCollectionSource.CreateInstance(JtValueNode owner) => CreateInstance(owner);
    IJtSuggestionCollectionChild<TSuggestion> IJtSuggestionCollectionSourceChild<TSuggestion>.CreateInstance(JtValueNode owner) => CreateInstance(owner);
    public int IndexOf(IJtSuggestionCollectionSourceChild<TSuggestion> item) => Suggestions.IndexOf(item);
    public void Insert(int index, IJtSuggestionCollectionSourceChild<TSuggestion> item) => Suggestions.Insert(index, item);
    public void RemoveAt(int index) => Suggestions.RemoveAt(index);
    public void Add(IJtSuggestionCollectionSourceChild<TSuggestion> item) => Suggestions.Add(item);
    public void Clear() => Suggestions.Clear();
    public bool Contains(IJtSuggestionCollectionSourceChild<TSuggestion> item) => Suggestions.Contains(item);
    public void CopyTo(IJtSuggestionCollectionSourceChild<TSuggestion>[] array, int arrayIndex) => Suggestions.CopyTo(array, arrayIndex);
    public bool Remove(IJtSuggestionCollectionSourceChild<TSuggestion> item) => Suggestions.Remove(item);
    public IEnumerator<IJtSuggestionCollectionSourceChild<TSuggestion>> GetEnumerator() => Suggestions.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Suggestions.GetEnumerator();
    public IJtCommonSuggestion AddNewSuggestion(object? value, string? displayName = null)
    {
        if (value is TSuggestion tValue)
        {
            JtSuggestionSource<TSuggestion> suggestion = new JtSuggestionSource<TSuggestion>(tValue, displayName);
            Add(suggestion);
            return suggestion;
        }

        throw new InvalidCastException($"Cannot convert {value?.GetType()} to {typeof(TSuggestion)}");

    }
    public IJtCommonSuggestionCollection AddNewSuggestionCollection()
    {
        JtSuggestionCollectionSource<TSuggestion> collection = new JtSuggestionCollectionSource<TSuggestion>(this);
        Add(collection);
        return collection;
    }
}
