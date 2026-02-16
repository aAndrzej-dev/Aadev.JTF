using System.Collections.Generic;
using Aadev.JTF.CustomSources;

namespace Aadev.JTF.CollectionBuilders;

internal sealed class JtNodeSourceCollectionInstanceBuilder : IJtNodeCollectionSourceBuilder
{
    private readonly JtNodeCollection instance;

    public JtNodeSourceCollectionInstanceBuilder(JtNodeCollection instance)
    {
        this.instance = instance;
    }

    public List<IJtSourceStructureElement> Build(JtNodeCollectionSource @this)
    {
        List<IJtSourceStructureElement> list = new List<IJtSourceStructureElement>(instance.Children.Count);

        for (int i = 0; i < instance.Children.Count; i++)
        {
            list.Add(instance.Children[i].CreateSource());
        }

        return list;
    }
}