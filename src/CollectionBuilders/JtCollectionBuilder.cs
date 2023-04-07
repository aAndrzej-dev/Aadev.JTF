using Aadev.JTF.CustomSources;
using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.CollectionBuilders
{
    internal static class JtCollectionBuilder
    {
        internal static IJtCollectionBuilder<T> CreateEmpty<T>() => EmptyJtBuilder<T>.Instance;
        public static IJtCollectionBuilder<IJtNodeCollectionSourceChild> CreateJtNodeSourceCollection(IJtNodeSourceParent parent, JArray? source)
        {
            if (source is null)
                return CreateEmpty<IJtNodeCollectionSourceChild>();
            return new JtNodeSourceCollectionBuilder(parent, source);
        }
        public static IJtCollectionBuilder<IJtNodeCollectionSourceChild> CreateJtNodeSourceCollection(IJtNodeSourceParent parent, JtNodeCollectionSource @base, JArray? @override)
        {
            if (@override is null)
                return new JtNodeSourceCopyBuilder(@base);
            return new JtNodeSourceCollectionOverrideBuilder(parent, @base, @override);
        }
        public static IJtCollectionBuilder<IJtNodeCollectionSourceChild> CreateJtNodeSourceCollection(JtNodeCollection sourceCollection)
        {
            if (sourceCollection is null)
                return CreateEmpty<IJtNodeCollectionSourceChild>();
            return new JtNodeSourceCollectionInstanceBuilder(sourceCollection);
        }
        internal static IJtCollectionBuilder<IJtSuggestionCollectionChild<T>> CreateJtSuggestionCollection<T>(JtValueNode owner, JArray? source)
        {
            if (source is null)
                return CreateEmpty<IJtSuggestionCollectionChild<T>>();
            return new JtSuggestionCollectionBuilder<T>(owner, source);
        }
        internal static IJtCollectionBuilder<IJtSuggestionCollectionChild<T>> CreateJtSuggestionCollection<T>(JtSuggestionCollectionSource<T>? source)
        {
            if (source is null)
                return CreateEmpty<IJtSuggestionCollectionChild<T>>();
            return new JtSuggestionCollectionInstanceBuilder<T>(source);
        }
        internal static IJtCollectionBuilder<IJtSuggestionCollectionSourceChild<T>> CreateJtSuggestionCollectionSource<T>(JtSuggestionCollectionSource<T> parent, JArray? source)
        {
            if (source is null)
                return CreateEmpty<IJtSuggestionCollectionSourceChild<T>>();

            return new JtSuggestionCollectionSourceBuilder<T>(parent, source);
        }
        internal static IJtCollectionBuilder<IJtSuggestionCollectionSourceChild<T>> CreateJtSuggestionCollectionSource<T>(IJtCustomSourceParent parent, JtSuggestionCollection<T>? source)
        {
            if (source is null)
                return CreateEmpty<IJtSuggestionCollectionSourceChild<T>>();
            return new JtSuggestionCollectionSourceInstanceBuilder<T>(parent, source);
        }
    }
}
