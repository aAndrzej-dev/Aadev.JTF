using Aadev.JTF.CustomSources;
using Aadev.JTF.JtEnumerable;
using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Aadev.JTF
{
    public sealed class JtSuggestionCollection<T> : IJtSuggestionCollectionChild<T>, IJtSuggestionCollection
    {
        internal JtSourceReference Id { get; }
        internal readonly IJtEnumerable<IJtSuggestionCollectionChild<T>> suggestionEnumerable;

        public Type SuggestionType => typeof(T);

        private JtSuggestionCollection()
        {
            suggestionEnumerable = JtEnumerable.JtEnumerable.CreateEmpty<IJtSuggestionCollectionChild<T>>();
        }
        private JtSuggestionCollection(JtValue owner, JArray? source)
        {
            suggestionEnumerable = JtEnumerable.JtEnumerable.CreateJtSuggestionCollection<T>(owner, source);
        }
        private JtSuggestionCollection(JtSourceReference id)
        {
            suggestionEnumerable = JtEnumerable.JtEnumerable.CreateEmpty<IJtSuggestionCollectionChild<T>>();
            Id = id;
        }
        private JtSuggestionCollection(JtSuggestionCollectionSource<T> source, JtSourceReference id)
        {
            suggestionEnumerable = JtEnumerable.JtEnumerable.CreateJtSuggestionCollection<T>(source);
            Id = id;
        }
        public bool IsStatic => Id.Type != JtSourceReferenceType.Dynamic && suggestionEnumerable.Enumerate().All(x => x.IsStatic);

        public static JtSuggestionCollection<T> Create() => new JtSuggestionCollection<T>();
        public static JtSuggestionCollection<T> Create(JtValue owner, JToken? value)
        {
            if (owner is null)
                throw new ArgumentNullException(nameof(owner));
            if (value?.Type is JTokenType.String)
            {
                JtSourceReference id = (string?)value;
                switch (id.Type)
                {
                    case JtSourceReferenceType.None:
                    default:
                        return new JtSuggestionCollection<T>();
                    case JtSourceReferenceType.Local:
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
                }
            }
            if (value?.Type is JTokenType.Array)
            {
                return new JtSuggestionCollection<T>(owner, (JArray)value);
            }
            return new JtSuggestionCollection<T>();
        }
        internal static JtSuggestionCollection<T> Create(JtSuggestionCollectionSource<T> source, JtSourceReference id) => new JtSuggestionCollection<T>(source, id);


        public IEnumerable<IJtSuggestion> GetSuggestions(Func<JtIdentifier, IEnumerable<IJtSuggestion>> dynamicSuggestionsSource)
        {
            if (Id.Type is JtSourceReferenceType.Dynamic)
                return dynamicSuggestionsSource?.Invoke(Id.Identifier)?.Select(x => (JtSuggestion<T>)x) ?? Enumerable.Empty<IJtSuggestion>();

            return suggestionEnumerable.Enumerate().SelectMany(x => x.GetSuggestions(dynamicSuggestionsSource)).Distinct();
        }
        internal void BuildJson(StringBuilder sb)
        {
            if (Id.IsEmpty)
            {
                sb.Append('[');
                List<IJtSuggestionCollectionChild<T>> list = suggestionEnumerable.Enumerate();
#if NET5_0_OR_GREATER
                var listSpan = CollectionsMarshal.AsSpan(list);
                for (int i = 0; i < listSpan.Length; i++)
                {
                    if (i > 0)
                        sb.Append(',');
                    listSpan[i].BuildJson(sb);
                }
#else
                for (int i = 0; i < list.Count; i++)
                {
                    if (i > 0)
                        sb.Append(',');
                    list[i].BuildJson(sb);
                }
#endif
                sb.Append(']');
            }
            else
            {
                sb.Append($"\"{Id}\"");
            }
        }



        internal JtSuggestionCollectionSource<T> CreateSource(ICustomSourceParent parent) => JtSuggestionCollectionSource<T>.Create(parent, this);
        IJtSuggestionCollectionSourceChild<T> IJtSuggestionCollectionChild<T>.CreateSource(ICustomSourceParent parent) => CreateSource(parent);
        IJtSuggestionCollectionSource IJtSuggestionCollection.CreateSource(ICustomSourceParent parent) => CreateSource(parent);
        void IJtSuggestionCollectionChild<T>.BuildJson(StringBuilder sb) => BuildJson(sb);
        void IJtSuggestionCollection.BuildJson(StringBuilder sb) => BuildJson(sb);

        public bool IsEmpty => suggestionEnumerable.Enumerate().Count == 0 && Id.Type != JtSourceReferenceType.Dynamic;
    }
}
