using Aadev.JTF.CollectionBuilders;
using Aadev.JTF.CustomSources;
using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Aadev.JTF
{
    public sealed class JtSuggestionCollection<TSuggestion> : IJtSuggestionCollection<TSuggestion>
    {
        private IJtCollectionBuilder<IJtSuggestionCollectionChild<TSuggestion>>? suggestionsBuilder;
        private List<IJtSuggestionCollectionChild<TSuggestion>>? suggestions;
        private JtSourceReference id;
        private readonly JtSuggestionCollectionSource<TSuggestion>? @base;

        internal JtSourceReference Id
        {
            get => id;
            set
            {
                if (value.IsEmpty)
                {
                    id = JtSourceReference.Empty;
                    return;
                }
                if (value.Type is not JtSourceReferenceType.Dynamic)
                    throw new JtfException("Cannot set non-dynamic id to a suggestion collection. Use custom sources instead.");
                id = value;
            }
        }


        [MemberNotNull(nameof(suggestions))]
        internal List<IJtSuggestionCollectionChild<TSuggestion>> Suggestions
        {
            get
            {
                if (suggestions is null)
                {
                    if (@base is not null)
                    {
                        suggestions = new List<IJtSuggestionCollectionChild<TSuggestion>>(@base.Count);

                        for (int i = 0; i < @base.Count; i++)
                        {
                            suggestions.Add(@base[i].CreateInstance());
                        }
                    }
                    else if (suggestionsBuilder is not null)
                    {
                        suggestions = suggestionsBuilder.Build();
                        suggestionsBuilder = null;
                    }
                    else
                        suggestions = new List<IJtSuggestionCollectionChild<TSuggestion>>();
                }
                return suggestions;
            }
        }


        public Type SuggestionType => typeof(TSuggestion);

        private JtSuggestionCollection() { }
        private JtSuggestionCollection(JtValueNode owner, JArray? source)
        {
            suggestionsBuilder = JtCollectionBuilder.CreateJtSuggestionCollection<TSuggestion>(owner, source);
        }
        private JtSuggestionCollection(JtSourceReference id)
        {
            Id = id;
        }
        private JtSuggestionCollection(JtSuggestionCollectionSource<TSuggestion> source)
        {
            @base = source;
        }
        public bool IsStatic => Id.IsEmpty && Suggestions.All(x => x.IsStatic);

        public static JtSuggestionCollection<TSuggestion> Create() => new JtSuggestionCollection<TSuggestion>();

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
                        return new JtSuggestionCollection<TSuggestion>(id);
                    case JtSourceReferenceType.External:
                        return owner.GetCustomSource<JtSuggestionCollectionSource<TSuggestion>>(id)?.CreateInstance() ?? new JtSuggestionCollection<TSuggestion>(id);
                    case JtSourceReferenceType.Direct:
                    {
                        JtValueNodeSource? element = owner.GetCustomSource<JtValueNodeSource>(id);
                        if (element is null || element.Suggestions.SuggestionType != typeof(TSuggestion))
                            return new JtSuggestionCollection<TSuggestion>(id);

                        return (JtSuggestionCollection<TSuggestion>)element.Suggestions.CreateInstance();
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
        public static JtSuggestionCollection<TSuggestion> Create(JtValueNode owner, JToken? value)
        {
            return TryCreate(owner, value) ?? new JtSuggestionCollection<TSuggestion>();
        }
        internal static JtSuggestionCollection<TSuggestion> Create(JtSuggestionCollectionSource<TSuggestion> source) => new JtSuggestionCollection<TSuggestion>(source);
        internal static JtSuggestionCollection<TSuggestion> Create(JtSourceReference id) => new JtSuggestionCollection<TSuggestion>(id);


        public IEnumerable<IJtSuggestion> GetSuggestions(Func<JtIdentifier, IEnumerable<IJtSuggestion>> dynamicSuggestionsSource)
        {
            if (!Id.IsEmpty)
                return dynamicSuggestionsSource?.Invoke(Id.Identifier)?.Select(x => (JtSuggestion<TSuggestion>)x) ?? Enumerable.Empty<IJtSuggestion>();

            return Suggestions.SelectMany(x => x.GetSuggestions(dynamicSuggestionsSource)).Distinct();
        }
        internal void BuildJson(StringBuilder sb)
        {
            if (@base?.IsDeclared is true)
            {
                sb.Append($"\"{@base.Declaration.Name}\"");
            }
            else if (Id.IsEmpty)
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
                sb.Append($"\"{Id}\"");
            }
        }



        internal JtSuggestionCollectionSource<TSuggestion> CreateSource(IJtCustomSourceParent parent) => @base is null ? JtSuggestionCollectionSource<TSuggestion>.Create(parent, this) : @base;
        IJtSuggestionCollectionSourceChild<TSuggestion> IJtSuggestionCollectionChild<TSuggestion>.CreateSource(IJtCustomSourceParent parent) => CreateSource(parent);
        IJtSuggestionCollectionSource IJtSuggestionCollection.CreateSource(IJtCustomSourceParent parent) => CreateSource(parent);
        void IJtSuggestionCollectionChild<TSuggestion>.BuildJson(StringBuilder sb) => BuildJson(sb);
        void IJtSuggestionCollection.BuildJson(StringBuilder sb) => BuildJson(sb);

        public bool IsEmpty => Suggestions.Count == 0 && Id.IsEmpty;


        public int IndexOf(IJtSuggestionCollectionChild<TSuggestion> item) => Suggestions.IndexOf(item);
        public void Insert(int index, IJtSuggestionCollectionChild<TSuggestion> item) { ThrowIfReadOnly(); Suggestions.Insert(index, item); }
        public void RemoveAt(int index) { ThrowIfReadOnly(); Suggestions.RemoveAt(index); }
        public void Add(IJtSuggestionCollectionChild<TSuggestion> item) { ThrowIfReadOnly(); Suggestions.Add(item); }
        public void Clear()
        {
            ThrowIfReadOnly();

            if (suggestions is null)
            {
                suggestionsBuilder = null;
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

        public bool IsReadOnly => @base is not null && !id.IsEmpty;


        private void ThrowIfReadOnly()
        {
            if (IsReadOnly)
                throw new ReadOnlyException("Suggestion collection based on suggestion collection source can not be edited.");
        }

        public IJtSuggestionCollectionChild<TSuggestion> this[int index] { get => Suggestions[index]; set { ThrowIfReadOnly(); Suggestions[index] = value; } }

    }
}
