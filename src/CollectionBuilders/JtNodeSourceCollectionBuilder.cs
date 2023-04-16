using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Aadev.JTF.CollectionBuilders
{
    internal sealed class JtNodeSourceCollectionBuilder : IJtCollectionBuilder<IJtNodeCollectionSourceChild>
    {
        private readonly JtNodeCollectionSource owner;
        private readonly JArray source;
        public JtNodeSourceCollectionBuilder(JtNodeCollectionSource owner, JArray source)
        {
            this.owner = owner;
            this.source = source;
        }

        public List<IJtNodeCollectionSourceChild> Build()
        {
            List<IJtNodeCollectionSourceChild> list = new List<IJtNodeCollectionSourceChild>(source.Count);

            for (int i = 0; i < source.Count; i++)
            {
                list.Add(CreateChildItem(owner, source[i]));
            }
            return list;
        }

        internal static IJtNodeCollectionSourceChild CreateChildItem(IJtNodeSourceParent parent, JToken source)
        {
            JtUnknownNodeSource CreateUnknown() => new JtUnknownNodeSource(parent, null);
            if (source is null)
                return CreateUnknown();
            if (source.Type is JTokenType.Array)
                return JtNodeCollectionSource.Create(parent, source);
            if (source.Type is JTokenType.String)
            {
                JtSourceReference id = (string?)source;
                switch (id.Type)
                {
                    case JtSourceReferenceType.None:
                    case JtSourceReferenceType.Local:
                    case JtSourceReferenceType.Dynamic:
                    default:
                        return CreateUnknown();
                    case JtSourceReferenceType.External:
                    {
                        CustomSource? cs = parent.SourceProvider.GetCustomSource(id);

                        return cs is IJtNodeCollectionSourceChild child ? child : CreateUnknown();
                    }
                    case JtSourceReferenceType.Direct:
                    {
                        return parent.SourceProvider.GetCustomSource<JtNodeSource>(id) ?? CreateUnknown();
                    }
                }
            }
            if (source.Type is JTokenType.Object)
            {
                JToken? @base = source["base"];
                if (@base is null)
                    return JtNodeSource.Create(parent, source);
                IJtNodeCollectionSourceChild baseNode = new CustomSourceBaseDeclaration(@base, parent.SourceProvider).Value;
                if (baseNode is JtNodeCollectionSource ncs)
                    return ncs.CreateOverride(parent, (JArray?)source["_"]);
                if (baseNode is JtNodeSource n)
                    return n.CreateOverride(parent, (JObject)source);

                return CreateUnknown();
            }
            return CreateUnknown();
        }
    }
}