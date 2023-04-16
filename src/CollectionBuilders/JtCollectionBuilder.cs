using Aadev.JTF.CustomSources;
using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.CollectionBuilders
{
    internal static class JtCollectionBuilder
    {
        internal static IJtCollectionBuilder<T> CreateEmpty<T>() => EmptyJtBuilder<T>.Instance;
        public static IJtCollectionBuilder<IJtNodeCollectionSourceChild> CreateJtNodeSourceCollection(JtNodeCollectionSource owner, JArray? source)
        {
            if (source is null)
                return CreateEmpty<IJtNodeCollectionSourceChild>();
            return new JtNodeSourceCollectionBuilder(owner, source);
        }
        public static IJtCollectionBuilder<IJtNodeCollectionSourceChild> CreateJtNodeSourceCollection(JtNodeCollectionSource owner, JtNodeCollectionSource @base, JArray? @override)
        {
            if (@override is null)
                return new JtNodeSourceCopyBuilder(@base);
            return new JtNodeSourceCollectionOverrideBuilder(owner, @base, @override);
        }
        public static IJtCollectionBuilder<IJtNodeCollectionSourceChild> CreateJtNodeSourceCollection(JtNodeCollection source)
        {
            if (source is null)
                return CreateEmpty<IJtNodeCollectionSourceChild>();
            return new JtNodeSourceCollectionInstanceBuilder(source);
        }
        internal static IJtCollectionBuilder<IJtSuggestionCollectionChild<TSuggestion>> CreateJtSuggestionCollection<TSuggestion>(JtValueNode owner, JArray? source)
        {
            if (source is null)
                return CreateEmpty<IJtSuggestionCollectionChild<TSuggestion>>();
            return new JtSuggestionCollectionBuilder<TSuggestion>(owner, source);
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
}
