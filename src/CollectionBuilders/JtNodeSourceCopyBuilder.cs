using System.Collections.Generic;
using Aadev.JTF.CustomSources;

namespace Aadev.JTF.CollectionBuilders;

internal sealed class JtNodeSourceCopyBuilder : IJtNodeCollectionSourceBuilder
{
    private readonly JtNodeCollectionSource source;

    public JtNodeSourceCopyBuilder(JtNodeCollectionSource source)
    {
        this.source = source;
    }

    public List<IJtSourceStructureElement> Build(JtNodeCollectionSource @this)
    {
        List<IJtSourceStructureElement> list = new List<IJtSourceStructureElement>(source.Children.Count);

        for (int i = 0; i < source.Children.Count; i++)
        {
            list.Add(source.Children[i]);
        }

        return list;
    }
}
