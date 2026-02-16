using System.Collections.Generic;
using Aadev.JTF.CustomSources;
using Aadev.JTF.CustomSources.Declarations;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.CollectionBuilders;

internal sealed class JtNodeSourceCollectionOverrideBuilder : IJtNodeCollectionSourceBuilder
{
    private readonly JArray @override;
    private readonly JtNodeCollectionSource @base;

    public JtNodeSourceCollectionOverrideBuilder(JtNodeCollectionSource @base, JArray @override)
    {
        this.@override = @override;
        this.@base = @base;
    }
    public List<IJtSourceStructureElement> Build(JtNodeCollectionSource @this)
    {
        List<IJtSourceStructureElement> list = new List<IJtSourceStructureElement>();

        int baseIndex = -1;
        int overrideIndex = -1;

        while (true)
        {
            baseIndex++;
            overrideIndex++;
            if (@base.Children.Count <= baseIndex)
            {
                if (@override.Count <= overrideIndex)
                {
                    break;
                }
                else
                {
                    list.Add(new CustomSourceBaseDeclaration(@override[overrideIndex], @this.SourceProvider).Value);
                }
            }
            else if (@override.Count <= overrideIndex)
            {
                list.Add(@base.Children[baseIndex]);
            }
            else if (@override[overrideIndex]?.Type is JTokenType.Null)
            {
                continue;
            }
            else
            {
                list.Add(@base.Children[baseIndex].CreateOverride(@this, @override[overrideIndex]));
            }
        }

        return list;
    }
}