using System.Collections.Generic;
using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.CollectionBuilders;

internal sealed class JtSuggestionCollectionSourceBuilder<TSuggestion> : IJtSuggestionCollectionSourceBuilder<TSuggestion>
{
    private readonly JArray source;

    public JtSuggestionCollectionSourceBuilder(JArray source)
    {
        this.source = source;
    }
    public List<IJtSuggestionCollectionSourceChild<TSuggestion>> Build(JtSuggestionCollectionSource<TSuggestion> owner)
    {
        List<IJtSuggestionCollectionSourceChild<TSuggestion>> list = new List<IJtSuggestionCollectionSourceChild<TSuggestion>>(source.Count);

        for (int i = 0; i < source.Count; i++)
        {
            list.Add(CreateSuggestionItem(owner, source[i]));
        }

        return list;
    }

    private static IJtSuggestionCollectionSourceChild<TSuggestion> CreateSuggestionItem(JtSuggestionCollectionSource<TSuggestion> owner, JToken source)
    {
        if (source?.Type is JTokenType.Array || source?.Type is JTokenType.String)
            return JtSuggestionCollectionSource<TSuggestion>.Create(owner, source);
        if (source?.Type is JTokenType.Object)
            return new JtSuggestionSource<TSuggestion>((JObject)source);
        return new JtSuggestionSource<TSuggestion>(default!, "Unknown");
    }
}
