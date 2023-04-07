using Aadev.JTF.CustomSources;
using System.Collections.Generic;

namespace Aadev.JTF.CollectionBuilders
{
    internal sealed class JtSuggestionCollectionSourceInstanceBuilder<T> : IJtCollectionBuilder<IJtSuggestionCollectionSourceChild<T>>
    {
        private readonly JtSuggestionCollection<T> source;
        private readonly IJtCustomSourceParent parent;
        public JtSuggestionCollectionSourceInstanceBuilder(IJtCustomSourceParent parent, JtSuggestionCollection<T> source)
        {
            this.source = source;
            this.parent = parent;
        }



        public List<IJtSuggestionCollectionSourceChild<T>> Build()
        {
            List<IJtSuggestionCollectionSourceChild<T>> list = new List<IJtSuggestionCollectionSourceChild<T>>(source.Count);

            for (int i = 0; i < source.Count; i++)
            {
                list.Add(source[i].CreateSource(parent));
            }
            return list;
        }
    }
}
