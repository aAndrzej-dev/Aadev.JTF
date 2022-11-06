using Aadev.JTF.CustomSources;
using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.JtEnumerable
{
    internal sealed class JtNodeCollectionIterator : JtIterator<IJtNodeCollectionChild>
    {
        private readonly IJtNodeParent parent;
        private readonly JArray source;
        private readonly ICustomSourceProvider sourceProvider;
        private int index = -1;

        public JtNodeCollectionIterator(IJtNodeParent parent, JArray source, ICustomSourceProvider sourceProvider)
        {
            this.parent = parent;
            this.source = source;
            this.sourceProvider = sourceProvider;
        }

        public override JtIterator<IJtNodeCollectionChild> Clone() => new JtNodeCollectionIterator(parent, source, sourceProvider);
        public override bool MoveNext()
        {
            index++;
            if (index >= source.Count)
            {
                Current = null!;
                return false;
            }

            Current = CreateChildItem(source[index], parent, sourceProvider);
          
            return true;
        }

        internal static IJtNodeCollectionChild CreateChildItem(JToken source, IJtNodeParent parent, ICustomSourceProvider sourceProvider)
        {
            JtUnknown CreateUnknown() => new JtUnknown(parent);

            if (source is null)
                return CreateUnknown();
            if (source.Type is JTokenType.Array)
            {
                return JtNodeCollection.Create(parent, source, sourceProvider);
            }
            if (source.Type is JTokenType.String)
            {
                JtCustomResourceIdentifier id = (string?)source;
                if (id.Type is JtCustomResourceIdentifierType.None)
                    return CreateUnknown();
                if (id.Type is JtCustomResourceIdentifierType.Dynamic)
                    return CreateUnknown();
                if (id.Type is JtCustomResourceIdentifierType.External)
                {
                    return ((IJtNodeCollectionSourceChild)sourceProvider.GetCustomSource(id)!).CreateInstance(parent, null);
                }
                if (id.Type is JtCustomResourceIdentifierType.Direct)
                {
                    CustomSource? cs = sourceProvider.GetCustomSource(id);
                    if (cs is null)
                        return CreateUnknown();

                    if (cs is JtContainerNodeSource cns)
                    {
                        return cns.Children.CreateInstance(parent, (JToken?)null);
                    }

                    return CreateUnknown();
                }
            }
            if (source.Type is JTokenType.Object)
            {
                JToken? @base = source["base"];
                if (@base is null)
                {
                    return JtNode.Create(source, parent, sourceProvider);
                }
                IJtNodeCollectionSourceChild child = new CustomSourceBaseDeclaration(@base, sourceProvider).Value;
    


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