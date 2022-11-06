using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System;

namespace Aadev.JTF.JtEnumerable
{
    internal sealed class JtSuggestionCollectionIterator<T> : JtIterator<IJtSuggestionCollectionChild<T>>
    {
        private readonly JArray source;
        private readonly ICustomSourceProvider sourceProvider;
        private int index = -1;
        public JtSuggestionCollectionIterator(JArray source, ICustomSourceProvider sourceProvider)
        {
            this.source = source;
            this.sourceProvider = sourceProvider;
        }


        public override JtIterator<IJtSuggestionCollectionChild<T>> Clone() => new JtSuggestionCollectionIterator<T>(source, sourceProvider);
        public override bool MoveNext()
        {
            index++;
            if (index >= source.Count)
            {
                Current = null!;
                return false;
            }
            Current = CreateSuggestionItem(source[index]);
            return true;
        }

        private IJtSuggestionCollectionChild<T> CreateSuggestionItem(JToken source)
        {
            if (source is null)
                return new JtSuggestion<T>(default!, "Unknown");
            if (source.Type is JTokenType.Array || source.Type is JTokenType.String)
            {
                return JtSuggestionCollection<T>.Create(source, sourceProvider);
            }
            if (source.Type is JTokenType.Object)
            {
                return new JtSuggestion<T>((JObject)source);
            }
            return new JtSuggestion<T>(default!, "Unknown");
        }
    }
}
