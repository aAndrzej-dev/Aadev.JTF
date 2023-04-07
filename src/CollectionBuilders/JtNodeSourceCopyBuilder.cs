using Aadev.JTF.CustomSources;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Aadev.JTF.CollectionBuilders
{
    internal sealed class JtNodeSourceCopyBuilder : IJtCollectionBuilder<IJtNodeCollectionSourceChild>
    {
        private readonly JtNodeCollectionSource source;

        public JtNodeSourceCopyBuilder(JtNodeCollectionSource source)
        {
            this.source = source;
        }

        public List<IJtNodeCollectionSourceChild> Build()
        {
            List<IJtNodeCollectionSourceChild> list = new List<IJtNodeCollectionSourceChild>(source.Children.Count);

            for (int i = 0; i < source.Children.Count; i++)
            {
                list.Add(source.Children[i]);
            }
            return list;
        }
    }
}
