using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Aadev.JTF.CollectionBuilders
{
    internal sealed class JtSuggestionCollectionBuilder<T> : IJtCollectionBuilder<IJtSuggestionCollectionChild<T>>
    {
        private readonly JtValueNode owner;
        private readonly JArray source;
        public JtSuggestionCollectionBuilder(JtValueNode owner, JArray source)
        {
            this.owner = owner;
            this.source = source;
        }


        public List<IJtSuggestionCollectionChild<T>> Build()
        {
            List<IJtSuggestionCollectionChild<T>> list = new List<IJtSuggestionCollectionChild<T>>(source.Count);

            for (int i = 0; i < source.Count; i++)
            {
                list.Add(CreateSuggestionItem(source[i]));
            }
            return list;
        }

        private IJtSuggestionCollectionChild<T> CreateSuggestionItem(JToken source)
        {
            if (source?.Type is JTokenType.Array || source?.Type is JTokenType.String)
                return JtSuggestionCollection<T>.Create(owner, source);
            if (source?.Type is JTokenType.Object)
                return new JtSuggestion<T>((JObject)source);
            return new JtSuggestion<T>(default!, "Unknown");
        }
    }
}
