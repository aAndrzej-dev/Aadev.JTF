using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Aadev.JTF.CollectionBuilders
{
    internal sealed class JtNodeSourceCollectionOverrideBuilder : IJtCollectionBuilder<IJtNodeCollectionSourceChild>
    {
        private readonly JtNodeCollectionSource owner;
        private readonly JArray @override;
        private readonly JtNodeCollectionSource @base;

        public JtNodeSourceCollectionOverrideBuilder(JtNodeCollectionSource owner, JtNodeCollectionSource @base, JArray @override)
        {
            this.@override = @override;
            this.@base = @base;
            this.owner = owner;
        }
        public List<IJtNodeCollectionSourceChild> Build()
        {
            List<IJtNodeCollectionSourceChild> list = new List<IJtNodeCollectionSourceChild>();

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
                        list.Add(new CustomSourceBaseDeclaration(@override[overrideIndex], owner.SourceProvider).Value);
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
                    list.Add(@base.Children[baseIndex].CreateOverride(owner, @override[overrideIndex]));
                }
            }
            return list;
        }
    }
}