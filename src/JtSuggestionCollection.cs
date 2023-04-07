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
    public sealed class JtSuggestionCollection<T> : IJtSuggestionCollectionChild<T>, IJtSuggestionCollection, IList<IJtSuggestionCollectionChild<T>>
    {
        private IJtCollectionBuilder<IJtSuggestionCollectionChild<T>>? suggestionsBuilder;
        private List<IJtSuggestionCollectionChild<T>>? suggestions;
        private JtSourceReference id;

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
        internal List<IJtSuggestionCollectionChild<T>> Suggestions
        {
            get
            {
                if (suggestions is null)
                {
                    suggestions = suggestionsBuilder!.Build();
                    suggestionsBuilder = null;
                }
                return suggestions;
            }
        }


        public Type SuggestionType => typeof(T);

        private JtSuggestionCollection()
        {
            suggestionsBuilder = JtCollectionBuilder.CreateEmpty<IJtSuggestionCollectionChild<T>>();
        }
        private JtSuggestionCollection(JtValueNode owner, JArray? source)
        {
            suggestionsBuilder = JtCollectionBuilder.CreateJtSuggestionCollection<T>(owner, source);
        }
        private JtSuggestionCollection(JtSourceReference id)
        {
            suggestionsBuilder = JtCollectionBuilder.CreateEmpty<IJtSuggestionCollectionChild<T>>();
            Id = id;
        }
        private JtSuggestionCollection(JtSuggestionCollectionSource<T> source)
        {
            suggestionsBuilder = JtCollectionBuilder.CreateJtSuggestionCollection<T>(source);
            if(source.IsDeclared && source.Declaration is CustomSourceDeclaration csd)
            {
                id = new JtSourceReference(csd.Id, JtSourceReferenceType.External);
            }
        }
        public bool IsStatic => Id.IsEmpty && Suggestions.All(x => x.IsStatic);

        public static JtSuggestionCollection<T> Create() => new JtSuggestionCollection<T>();
        public static JtSuggestionCollection<T> Create(JtValueNode owner, JToken? value)
        {
            if (owner is null)
                throw new ArgumentNullException(nameof(owner));
            if (value?.Type is JTokenType.String)
            {
                JtSourceReference id = (string?)value;
                switch (id.Type)
                {

                    case JtSourceReferenceType.Dynamic:
                        return new JtSuggestionCollection<T>(id);
                    case JtSourceReferenceType.External:
                        return owner.GetCustomSource<JtSuggestionCollectionSource<T>>(id)?.CreateInstance() ?? new JtSuggestionCollection<T>(id);
                    case JtSourceReferenceType.Direct:
                    {
                        JtValueNodeSource? element = owner.GetCustomSource<JtValueNodeSource>(id);
                        if (element is null || element.Suggestions.SuggestionType != typeof(T))
                            return new JtSuggestionCollection<T>(id);

                        return (JtSuggestionCollection<T>)element.Suggestions.CreateInstance();
                    }
                    default:
                        return new JtSuggestionCollection<T>();
                }
            }
            if (value?.Type is JTokenType.Array)
            {
                return new JtSuggestionCollection<T>(owner, (JArray)value);
            }
            return new JtSuggestionCollection<T>();
        }
        internal static JtSuggestionCollection<T> Create(JtSuggestionCollectionSource<T> source) => new JtSuggestionCollection<T>(source);
        internal static JtSuggestionCollection<T> Create(JtSourceReference id) => new JtSuggestionCollection<T>(id);


        public IEnumerable<IJtSuggestion> GetSuggestions(Func<JtIdentifier, IEnumerable<IJtSuggestion>> dynamicSuggestionsSource)
        {
            if (!Id.IsEmpty)
                return dynamicSuggestionsSource?.Invoke(Id.Identifier)?.Select(x => (JtSuggestion<T>)x) ?? Enumerable.Empty<IJtSuggestion>();

            return Suggestions.SelectMany(x => x.GetSuggestions(dynamicSuggestionsSource)).Distinct();
        }
        internal void BuildJson(StringBuilder sb)
        {
            if (Id.IsEmpty)
            {
                sb.Append('[');
#if NET5_0_OR_GREATER
                Span<IJtSuggestionCollectionChild<T>> listSpan = CollectionsMarshal.AsSpan(Suggestions);
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



        internal JtSuggestionCollectionSource<T> CreateSource(IJtCustomSourceParent parent) => JtSuggestionCollectionSource<T>.Create(parent, this);
        IJtSuggestionCollectionSourceChild<T> IJtSuggestionCollectionChild<T>.CreateSource(IJtCustomSourceParent parent) => CreateSource(parent);
        IJtSuggestionCollectionSource IJtSuggestionCollection.CreateSource(IJtCustomSourceParent parent) => CreateSource(parent);
        void IJtSuggestionCollectionChild<T>.BuildJson(StringBuilder sb) => BuildJson(sb);
        void IJtSuggestionCollection.BuildJson(StringBuilder sb) => BuildJson(sb);
        public int IndexOf(IJtSuggestionCollectionChild<T> item) => ((IList<IJtSuggestionCollectionChild<T>>)Suggestions).IndexOf(item);
        public void Insert(int index, IJtSuggestionCollectionChild<T> item) => ((IList<IJtSuggestionCollectionChild<T>>)Suggestions).Insert(index, item);
        public void RemoveAt(int index) => ((IList<IJtSuggestionCollectionChild<T>>)Suggestions).RemoveAt(index);
        public void Add(IJtSuggestionCollectionChild<T> item) => ((ICollection<IJtSuggestionCollectionChild<T>>)Suggestions).Add(item);
        public void Clear() => ((ICollection<IJtSuggestionCollectionChild<T>>)Suggestions).Clear();
        public bool Contains(IJtSuggestionCollectionChild<T> item) => ((ICollection<IJtSuggestionCollectionChild<T>>)Suggestions).Contains(item);
        public void CopyTo(IJtSuggestionCollectionChild<T>[] array, int arrayIndex) => ((ICollection<IJtSuggestionCollectionChild<T>>)Suggestions).CopyTo(array, arrayIndex);
        public bool Remove(IJtSuggestionCollectionChild<T> item) => ((ICollection<IJtSuggestionCollectionChild<T>>)Suggestions).Remove(item);
        public IEnumerator<IJtSuggestionCollectionChild<T>> GetEnumerator() => ((IEnumerable<IJtSuggestionCollectionChild<T>>)Suggestions).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Suggestions).GetEnumerator();

        public bool IsEmpty => Suggestions.Count == 0 && Id.IsEmpty;

        public int Count => ((ICollection<IJtSuggestionCollectionChild<T>>)Suggestions).Count;

        public bool IsReadOnly => ((ICollection<IJtSuggestionCollectionChild<T>>)Suggestions).IsReadOnly;

        public IJtSuggestionCollectionChild<T> this[int index] { get => ((IList<IJtSuggestionCollectionChild<T>>)Suggestions)[index]; set => ((IList<IJtSuggestionCollectionChild<T>>)Suggestions)[index] = value; }
    }
}
