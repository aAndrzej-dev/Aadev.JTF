using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.JtEnumerable
{
    internal sealed class JtNodeSourceCollectionIterator : JtIterator<IJtNodeCollectionSourceChild>
    {
        private readonly ICustomSourceParent parent;
        private readonly JArray source;
        private int index = -1;
        public JtNodeSourceCollectionIterator(ICustomSourceParent parent, JArray source)
        {
            this.parent = parent;
            this.source = source;
        }

        internal static IJtNodeCollectionSourceChild CreateChildItem(ICustomSourceParent parent, JToken source)
        {
            JtUnknownNodeSource CreateUnknown() => new JtUnknownNodeSource(parent, null);
            if (source is null)
                return CreateUnknown();
            if (source.Type is JTokenType.Array)
            {
                return JtNodeCollectionSource.Create(parent, source);
            }
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

                        if (cs is IJtNodeCollectionSourceChild child)
                            return child;

                        return CreateUnknown();
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
                {
                    return JtNodeSource.Create(parent, source);
                }
                IJtNodeCollectionSourceChild baseNode = new CustomSourceBaseDeclaration(@base, parent.SourceProvider).Value;
                if (baseNode is JtNodeCollectionSource ncs)
                {
                    return ncs.CreateOverride(parent, (JArray?)source["_"]);
                }
                if (baseNode is JtNodeSource n)
                {
                    return n.CreateOverride(parent, (JObject)source);
                }

                return CreateUnknown();
            }
            return CreateUnknown();
        }
        public override JtIterator<IJtNodeCollectionSourceChild> Clone() => new JtNodeSourceCollectionIterator(parent, source);
        public override bool MoveNext()
        {
            index++;
            if (index >= source.Count)
            {
                Current = null!;
                return false;
            }

            Current = CreateChildItem(parent, source[index]);
            return true;
        }
    }
}