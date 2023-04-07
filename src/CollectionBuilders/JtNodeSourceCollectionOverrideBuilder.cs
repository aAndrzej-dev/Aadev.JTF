using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Aadev.JTF.CollectionBuilders
{
    internal sealed class JtNodeSourceCollectionOverrideBuilder : IJtCollectionBuilder<IJtNodeCollectionSourceChild>
    {
        private readonly IJtNodeSourceParent parent;
        private readonly JArray @override;
        private readonly JtNodeCollectionSource @base;

        public JtNodeSourceCollectionOverrideBuilder(IJtNodeSourceParent parent, JtNodeCollectionSource @base, JArray @override)
        {
            this.@override = @override;
            this.@base = @base;
            this.parent = parent;
        }
        public List<IJtNodeCollectionSourceChild> Build()
        {
            List<IJtNodeCollectionSourceChild> list = new List<IJtNodeCollectionSourceChild>();

            int baseIndex = -1;
            int overrideIndex = -1;

            while(true)
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
                        list.Add(new CustomSourceBaseDeclaration(@override[overrideIndex], parent.SourceProvider).Value);
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
                    list.Add(@base.Children[baseIndex].CreateOverride(parent, @override[overrideIndex]));
                }
            }
            return list;
        }
    }
}