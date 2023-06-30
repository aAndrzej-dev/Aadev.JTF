using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.CollectionBuilders;

internal static class JtCollectionBuilder
{
    public static IJtNodeCollectionSourceBuilder? CreateJtNodeSourceCollection(JArray? source)
    {
        if (source is null)
            return null;
        return new JtNodeSourceCollectionBuilder(source);
    }
    public static IJtNodeCollectionSourceBuilder CreateJtNodeSourceCollection(JtNodeCollectionSource @base, JArray? @override)
    {
        if (@override is null)
            return new JtNodeSourceCopyBuilder(@base);
        return new JtNodeSourceCollectionOverrideBuilder(@base, @override);
    }
    public static IJtNodeCollectionSourceBuilder? CreateJtNodeSourceCollection(JtNodeCollection source)
    {
        if (source is null)
            return null;
        return new JtNodeSourceCollectionInstanceBuilder(source);
    }
    internal static IJtSuggestionCollectionSourceBuilder<TSuggestion> CreateJtSuggestionCollectionSource<TSuggestion>(JArray source)
    {
        return new JtSuggestionCollectionSourceBuilder<TSuggestion>(source);
    }
    internal static IJtSuggestionCollectionSourceBuilder<TSuggestion> CreateJtSuggestionCollectionSource<TSuggestion>(JtSuggestionCollection<TSuggestion> source)
    {
        return new JtSuggestionCollectionSourceInstanceBuilder<TSuggestion>(source);
    }
}
