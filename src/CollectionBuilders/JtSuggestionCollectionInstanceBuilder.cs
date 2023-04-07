using Aadev.JTF.CustomSources;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Aadev.JTF.CollectionBuilders
{
    internal sealed class JtSuggestionCollectionInstanceBuilder<T> : IJtCollectionBuilder<IJtSuggestionCollectionChild<T>>
    {
        private readonly JtSuggestionCollectionSource<T> source;
        public JtSuggestionCollectionInstanceBuilder(JtSuggestionCollectionSource<T> source)
        {
            this.source = source;
        }

        public List<IJtSuggestionCollectionChild<T>> Build()
        {
            List<IJtSuggestionCollectionChild<T>> list = new List<IJtSuggestionCollectionChild<T>>(source.Count);

            for (int i = 0; i < source.Count; i++)
            {
                list.Add(source[i].CreateInstance());
            }
            return list;
        }
    }
}
