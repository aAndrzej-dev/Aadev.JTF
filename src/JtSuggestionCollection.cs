using Aadev.JTF.CustomSources;
using Aadev.JTF.JtEnumerable;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aadev.JTF
{
    public sealed class JtSuggestionCollection<T> : IJtSuggestionCollectionChild<T>, IJtSuggestionCollection
    {
        internal JtCustomResourceIdentifier Id { get; }
        internal readonly IJtEnumerable<IJtSuggestionCollectionChild<T>> suggestionEnumerable;

        public Type SuggestionType => typeof(T);

        private JtSuggestionCollection()
        {
            suggestionEnumerable = JtEnumerable.JtEnumerable.CreateEmpty<IJtSuggestionCollectionChild<T>>();
        }
        private JtSuggestionCollection(JArray? source, ICustomSourceProvider sourceProvider)
        {
            suggestionEnumerable = JtEnumerable.JtEnumerable.CreateJtSuggestionCollection<T>(source, sourceProvider);
        }
        private JtSuggestionCollection(JtCustomResourceIdentifier id)
        {
            suggestionEnumerable = JtEnumerable.JtEnumerable.CreateEmpty<IJtSuggestionCollectionChild<T>>();
            Id = id;
        }
        private JtSuggestionCollection(JtSuggestionCollectionSource<T> source, JtCustomResourceIdentifier id)
        {
            suggestionEnumerable = JtEnumerable.JtEnumerable.CreateJtSuggestionCollection<T>(source);
            Id = id;
        }
        public bool IsStatic => Id.Type != JtCustomResourceIdentifierType.Dynamic && suggestionEnumerable.Enumerate().All(x => x.IsStatic);

        public static JtSuggestionCollection<T> Create() => new JtSuggestionCollection<T>();
        public static JtSuggestionCollection<T> Create(JToken? value, ICustomSourceProvider sourceProvider)
        {
            if (value?.Type is JTokenType.String)
            {
                JtCustomResourceIdentifier id = (string?)value;
                if (id.Type is JtCustomResourceIdentifierType.None)
                    return new JtSuggestionCollection<T>();
                if (id.Type is JtCustomResourceIdentifierType.Dynamic)
                    return new JtSuggestionCollection<T>(id);
                if (id.Type is JtCustomResourceIdentifierType.External)
                    return sourceProvider.GetCustomSource<JtSuggestionCollectionSource<T>>(id)?.CreateInstance(id) ?? new JtSuggestionCollection<T>(id);
                if (id.Type is JtCustomResourceIdentifierType.Direct)
                {
                    JtValueNodeSource? element = sourceProvider.GetCustomSource<JtValueNodeSource>(id);
                    if (element is null)
                        return new JtSuggestionCollection<T>(id);
                    IJtSuggestionCollection instance = element.Suggestions.CreateInstance();

                    if (instance.SuggestionType != typeof(T))
                        return new JtSuggestionCollection<T>(id);

                    return (JtSuggestionCollection<T>)instance;
                }

            }
            if (value?.Type is JTokenType.Array)
            {
                return new JtSuggestionCollection<T>((JArray)value, sourceProvider);
            }
            return new JtSuggestionCollection<T>();
        }
        internal static JtSuggestionCollection<T> Create(JtSuggestionCollectionSource<T> source, JtCustomResourceIdentifier id) => new JtSuggestionCollection<T>(source, id);


        public IEnumerable<IJtSuggestion> GetSuggestions(Func<JtIdentifier, IEnumerable<IJtSuggestion>> dynamicSuggestionsSource)
        {
            if (Id.Type is JtCustomResourceIdentifierType.Dynamic)
                return dynamicSuggestionsSource(Id.Identifier).Select(x => (JtSuggestion<T>)x);

            return suggestionEnumerable.Enumerate().SelectMany(x => x.GetSuggestions(dynamicSuggestionsSource)).Distinct();
        }
        public void BuildJson(StringBuilder sb)
        {
            if (Id.IsEmpty)
            {
                sb.Append('[');
                for (int i = 0; i < suggestionEnumerable.Enumerate().Count; i++)
                {
                    if (i > 0)
                        sb.Append(',');
                    suggestionEnumerable.Enumerate()[i].BuildJson(sb);
                }
                sb.Append(']');
            }
            else
            {
                sb.Append($"\"{Id}\"");
            }
        }



        internal JtSuggestionCollectionSource<T> CreateSource(ICustomSourceParent parent) => JtSuggestionCollectionSource<T>.Create(this, parent);
        IJtSuggestionCollectionSourceChild<T> IJtSuggestionCollectionChild<T>.CreateSource(ICustomSourceParent parent) => CreateSource(parent);
        IJtSuggestionCollectionSource IJtSuggestionCollection.CreateSource(ICustomSourceParent parent) => CreateSource(parent);

        public bool IsEmpty => suggestionEnumerable.Enumerate().Count == 0 && Id.Type != JtCustomResourceIdentifierType.Dynamic;
    }
}
