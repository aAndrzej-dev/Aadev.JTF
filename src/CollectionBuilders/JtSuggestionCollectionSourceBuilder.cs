using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Aadev.JTF.CollectionBuilders
{
    internal sealed class JtSuggestionCollectionSourceBuilder<T> : IJtCollectionBuilder<IJtSuggestionCollectionSourceChild<T>>
    {
        private readonly JtSuggestionCollectionSource<T> parent;
        private readonly JArray source;

        public JtSuggestionCollectionSourceBuilder(JtSuggestionCollectionSource<T> parent, JArray source)
        {
            this.parent = parent;
            this.source = source;
        }
        public List<IJtSuggestionCollectionSourceChild<T>> Build()
        {
            List<IJtSuggestionCollectionSourceChild<T>> list = new List<IJtSuggestionCollectionSourceChild<T>>(source.Count);

            for (int i = 0; i < source.Count; i++)
            {
                list.Add(CreateSuggestionItem(source[i]));
            }
            return list;
        }

        private IJtSuggestionCollectionSourceChild<T> CreateSuggestionItem(JToken source)
        {
            if (source?.Type is JTokenType.Array || source?.Type is JTokenType.String)
                return JtSuggestionCollectionSource<T>.Create(parent, source);
            if (source?.Type is JTokenType.Object)
                return new JtSuggestionSource<T>(parent, (JObject)source);
            return new JtSuggestionSource<T>(parent, default!, "Unknown");
        }
    }
}
