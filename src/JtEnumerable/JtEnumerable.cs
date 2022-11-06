using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.JtEnumerable
{
    internal static class JtEnumerable
    {
        internal static IJtEnumerable<T> CreateEmpty<T>() => new EmptyJtIterator<T>();
        public static IJtEnumerable<IJtNodeCollectionSourceChild> CreateJtNodeSourceCollection(ICustomSourceParent parent, JArray? source, ICustomSourceProvider sourceProvider)
        {
            if (source is null)
                return CreateEmpty<IJtNodeCollectionSourceChild>();
            return new JtNodeSourceCollectionIterator(parent, source, sourceProvider);
        }
        public static IJtEnumerable<IJtNodeCollectionSourceChild> CreateJtNodeSourceCollection(ICustomSourceParent parent, JtNodeCollectionSource @base, JArray? @override, ICustomSourceProvider sourceProvider)
        {
            if (@override is null)
                return @base.nodeEnumerable;
            return new JtNodeSourceCollectionOverrideIterator(parent, @base, @override, sourceProvider);
        }
        public static IJtEnumerable<IJtNodeCollectionSourceChild> CreateJtNodeSourceCollection(JtNodeCollection sourceCollection, ICustomSourceProvider sourceProvider)
        {
            if (sourceCollection is null)
                return CreateEmpty<IJtNodeCollectionSourceChild>();
            return new JtNodeSourceCollectionInstanceIterator(sourceCollection, sourceProvider);
        }
        internal static IJtEnumerable<IJtNodeCollectionChild> CreatJtNodeCollection(IJtNodeParent parent, JArray? source, ICustomSourceProvider sourceProvider)
        {
            if (source is null)
                return CreateEmpty<IJtNodeCollectionChild>();
            return new JtNodeCollectionIterator(parent, source, sourceProvider);
        }
        internal static IJtEnumerable<IJtNodeCollectionChild> CreatJtNodeCollection(IJtNodeParent parent, JtNodeCollectionSource? @base, JArray? @override, ICustomSourceProvider sourceProvider)
        {
            if (@base is null)
                return CreateEmpty<IJtNodeCollectionChild>();
            if (@override is null)
                return new JtNodeCollectionInstanceIterator(parent, @base);
            else
                return new JtNodeCollectionOverrideIterator(parent, @base, @override, sourceProvider);
        }
        internal static IJtEnumerable<IJtSuggestionCollectionChild<T>> CreateJtSuggestionCollection<T>(JArray? source, ICustomSourceProvider sourceProvider)
        {
            if (source is null)
                return CreateEmpty<IJtSuggestionCollectionChild<T>>();
            return new JtSuggestionCollectionIterator<T>(source, sourceProvider);
        }
        internal static IJtEnumerable<IJtSuggestionCollectionChild<T>> CreateJtSuggestionCollection<T>(JtSuggestionCollectionSource<T>? source)
        {
            if (source is null)
                return CreateEmpty<IJtSuggestionCollectionChild<T>>();
            return new JtSuggestionCollectionInstanceIterator<T>(source);
        }
        internal static IJtEnumerable<IJtSuggestionCollectionSourceChild<T>> CreateJtSuggestionCollectionSource<T>(JtSuggestionCollectionSource<T> parent, JArray? source, ICustomSourceProvider sourceProvider)
        {
            if (source is null)
                return CreateEmpty<IJtSuggestionCollectionSourceChild<T>>();

            return new JtSuggestionCollectionSourceIterator<T>(parent, source, sourceProvider);
        }
        internal static IJtEnumerable<IJtSuggestionCollectionSourceChild<T>> CreateJtSuggestionCollectionSource<T>(JtSuggestionCollection<T>? source, ICustomSourceParent parent)
        {
            if (source is null)
                return CreateEmpty<IJtSuggestionCollectionSourceChild<T>>();
            return new JtSuggestionCollectionSourceInstanceIterator<T>(source, parent);
        }
    }
}
