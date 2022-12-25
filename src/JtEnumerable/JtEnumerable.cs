using Aadev.JTF.CustomSources;
using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.JtEnumerable
{
    internal static class JtEnumerable
    {
        internal static IJtEnumerable<T> CreateEmpty<T>() => new EmptyJtIterator<T>();
        public static IJtEnumerable<IJtNodeCollectionSourceChild> CreateJtNodeSourceCollection(ICustomSourceParent parent, JArray? source)
        {
            if (source is null)
                return CreateEmpty<IJtNodeCollectionSourceChild>();
            return new JtNodeSourceCollectionIterator(parent, source);
        }
        public static IJtEnumerable<IJtNodeCollectionSourceChild> CreateJtNodeSourceCollection(ICustomSourceParent parent, JtNodeCollectionSource @base, JArray? @override)
        {
            if (@override is null)
                return @base.nodeEnumerable;
            return new JtNodeSourceCollectionOverrideIterator(parent, @base, @override);
        }
        public static IJtEnumerable<IJtNodeCollectionSourceChild> CreateJtNodeSourceCollection(JtNodeCollection sourceCollection)
        {
            if (sourceCollection is null)
                return CreateEmpty<IJtNodeCollectionSourceChild>();
            return new JtNodeSourceCollectionInstanceIterator(sourceCollection);
        }
        internal static IJtEnumerable<IJtNodeCollectionChild> CreatJtNodeCollection(IJtNodeParent parent, JArray? source)
        {
            if (source is null)
                return CreateEmpty<IJtNodeCollectionChild>();
            return new JtNodeCollectionIterator(parent, source);
        }
        internal static IJtEnumerable<IJtNodeCollectionChild> CreatJtNodeCollection(IJtNodeParent parent, JtNodeCollectionSource? @base, JArray? @override)
        {
            if (@base is null)
                return CreateEmpty<IJtNodeCollectionChild>();
            if (@override is null)
                return new JtNodeCollectionInstanceIterator(parent, @base);
            else
                return new JtNodeCollectionOverrideIterator(parent, @base, @override);
        }
        internal static IJtEnumerable<IJtSuggestionCollectionChild<T>> CreateJtSuggestionCollection<T>(JtValue owner, JArray? source)
        {
            if (source is null)
                return CreateEmpty<IJtSuggestionCollectionChild<T>>();
            return new JtSuggestionCollectionIterator<T>(owner, source);
        }
        internal static IJtEnumerable<IJtSuggestionCollectionChild<T>> CreateJtSuggestionCollection<T>(JtSuggestionCollectionSource<T>? source)
        {
            if (source is null)
                return CreateEmpty<IJtSuggestionCollectionChild<T>>();
            return new JtSuggestionCollectionInstanceIterator<T>(source);
        }
        internal static IJtEnumerable<IJtSuggestionCollectionSourceChild<T>> CreateJtSuggestionCollectionSource<T>(JtSuggestionCollectionSource<T> parent, JArray? source)
        {
            if (source is null)
                return CreateEmpty<IJtSuggestionCollectionSourceChild<T>>();

            return new JtSuggestionCollectionSourceIterator<T>(parent, source);
        }
        internal static IJtEnumerable<IJtSuggestionCollectionSourceChild<T>> CreateJtSuggestionCollectionSource<T>(ICustomSourceParent parent, JtSuggestionCollection<T>? source)
        {
            if (source is null)
                return CreateEmpty<IJtSuggestionCollectionSourceChild<T>>();
            return new JtSuggestionCollectionSourceInstanceIterator<T>(parent, source);
        }
    }
}
