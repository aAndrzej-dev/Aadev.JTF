using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Aadev.JTF.CollectionBuilders
{
    internal sealed class JtSuggestionCollectionBuilder<TSuggestion> : IJtCollectionBuilder<IJtSuggestionCollectionChild<TSuggestion>>
    {
        private readonly JtValueNode owner;
        private readonly JArray source;
        public JtSuggestionCollectionBuilder(JtValueNode owner, JArray source)
        {
            this.owner = owner;
            this.source = source;
        }


        public List<IJtSuggestionCollectionChild<TSuggestion>> Build()
        {
            List<IJtSuggestionCollectionChild<TSuggestion>> list = new List<IJtSuggestionCollectionChild<TSuggestion>>(source.Count);

            for (int i = 0; i < source.Count; i++)
            {
                list.Add(CreateSuggestionItem(source[i]));
            }
            return list;
        }

        private IJtSuggestionCollectionChild<TSuggestion> CreateSuggestionItem(JToken source)
        {
            if (source?.Type is JTokenType.Array || source?.Type is JTokenType.String)
                return JtSuggestionCollection<TSuggestion>.Create(owner, source);
            if (source?.Type is JTokenType.Object)
                return new JtSuggestion<TSuggestion>((JObject)source);
            return new JtSuggestion<TSuggestion>(default!, "Unknown");
        }
    }
}
