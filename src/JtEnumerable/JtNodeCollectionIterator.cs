using Aadev.JTF.CustomSources;
using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.JtEnumerable
{
    internal sealed class JtNodeCollectionIterator : JtIterator<IJtNodeCollectionChild>
    {
        private readonly IJtNodeParent parent;
        private readonly JArray source;
        private int index = -1;

        public JtNodeCollectionIterator(IJtNodeParent parent, JArray source)
        {
            this.parent = parent;
            this.source = source;
        }

        public override JtIterator<IJtNodeCollectionChild> Clone() => new JtNodeCollectionIterator(parent, source);
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

        internal static IJtNodeCollectionChild CreateChildItem(IJtNodeParent parent, JToken source)
        {
            JtUnknown CreateUnknown() => new JtUnknown(parent);

            if (source is null)
                return CreateUnknown();
            if (source.Type is JTokenType.Array)
            {
                return JtNodeCollection.Create(parent, source);
            }
            if (source.Type is JTokenType.String)
            {
                JtSourceReference id = (string?)source;
                switch (id.Type)
                {
                    case JtSourceReferenceType.None:
                    case JtSourceReferenceType.Dynamic:
                    case JtSourceReferenceType.Local:
                    default:
                        return CreateUnknown();
                    case JtSourceReferenceType.External:
                        return ((IJtNodeCollectionSourceChild)parent.SourceProvider.GetCustomSource(id)!).CreateInstance(parent, null);
                    case JtSourceReferenceType.Direct:
                    {
                        return parent.SourceProvider.GetCustomSource<JtNodeSource>(id)?.CreateInstance(parent, null) ?? CreateUnknown();
                    }
                }
            }
            if (source.Type is JTokenType.Object)
            {
                JToken? @base = source["base"];
                if (@base is null)
                {
                    return JtNode.Create(parent, source);
                }
                IJtNodeCollectionSourceChild child = new CustomSourceBaseDeclaration(@base, parent.SourceProvider).Value;
    


                if (child is JtNodeCollectionSource)
                {
                    return child.CreateInstance(parent, source["_"]);
                }
                if (child is JtNodeSource)
                {
                    return child.CreateInstance(parent, source);
                }

                return CreateUnknown();
            }
            return CreateUnknown();
        }
    }
}