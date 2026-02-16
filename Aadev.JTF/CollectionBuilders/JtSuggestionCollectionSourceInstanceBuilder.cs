using System.Collections.Generic;
using Aadev.JTF.CustomSources;

namespace Aadev.JTF.CollectionBuilders;

internal sealed class JtSuggestionCollectionSourceInstanceBuilder<TSuggestion> : IJtSuggestionCollectionSourceBuilder<TSuggestion>
{
    private readonly JtSuggestionCollection<TSuggestion> source;
    public JtSuggestionCollectionSourceInstanceBuilder(JtSuggestionCollection<TSuggestion> source)
    {
        this.source = source;
    }

    public List<IJtSuggestionCollectionSourceChild<TSuggestion>> Build(JtSuggestionCollectionSource<TSuggestion> owner)
    {
        List<IJtSuggestionCollectionSourceChild<TSuggestion>> list = new List<IJtSuggestionCollectionSourceChild<TSuggestion>>(source.Count);

        for (int i = 0; i < source.Count; i++)
        {
            list.Add(source[i].CreateSource(owner));
        }

        return list;
    }
}
