using Aadev.JTF.CustomSources;
using System.Collections.Generic;

namespace Aadev.JTF.CollectionBuilders
{
    internal sealed class JtNodeSourceCollectionInstanceBuilder : IJtCollectionBuilder<IJtNodeCollectionSourceChild>
    {
        private readonly JtNodeCollection instance;

        public JtNodeSourceCollectionInstanceBuilder(JtNodeCollection instance)
        {
            this.instance = instance;
        }

        public List<IJtNodeCollectionSourceChild> Build()
        {
            List<IJtNodeCollectionSourceChild> list = new List<IJtNodeCollectionSourceChild>(instance.Children.Count);

            for (int i = 0; i < instance.Children.Count; i++)
            {
                list.Add(instance.Children[i].CreateSource());
            }
            return list;
        }
    }
}