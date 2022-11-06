using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.JtEnumerable
{
    internal sealed class JtNodeSourceCollectionIterator : JtIterator<IJtNodeCollectionSourceChild>
    {
        private readonly ICustomSourceParent parent;
        private readonly JArray array;
        private readonly ICustomSourceProvider sourceProvider;
        private int index = -1;
        public JtNodeSourceCollectionIterator(ICustomSourceParent parent, JArray array, ICustomSourceProvider sourceProvider)
        {
            this.parent = parent;
            this.array = array;
            this.sourceProvider = sourceProvider;
        }

        internal static IJtNodeCollectionSourceChild CreateChildItem(ICustomSourceParent parent, JToken source, ICustomSourceProvider sourceProvider)
        {
            JtUnknownNodeSource CreateUnknown()
            {
#if DEBUG
                throw new JtfException();
#else
                return new JtUnknownNodeSource(parent, null, sourceProvider);
#endif
            }
            if (source is null)
                return CreateUnknown();
            if (source.Type is JTokenType.Array)
            {
                return JtNodeCollectionSource.Create(parent, source, sourceProvider);
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
                    CustomSource? cs = sourceProvider.GetCustomSource(id);

                    if (cs is IJtNodeCollectionSourceChild child)
                        return child;

                    return CreateUnknown();
                }
                if (id.Type is JtCustomResourceIdentifierType.Direct)
                {
                    CustomSource? cs = sourceProvider.GetCustomSource(id);

                    if (cs is JtContainerNodeSource cns)
                        return cns.Children;


                    return CreateUnknown();
                }
            }
            if (source.Type is JTokenType.Object)
            {
                JToken? @base = source["base"];
                if (@base is null)
                {
                    return JtNodeSource.Create(parent, source, sourceProvider);
                }
                IJtNodeCollectionSourceChild baseNode =  new CustomSourceBaseDeclaration(@base, sourceProvider).Value;
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
        public override JtIterator<IJtNodeCollectionSourceChild> Clone() => new JtNodeSourceCollectionIterator(parent, array, sourceProvider);
        public override bool MoveNext()
        {
            index++;
            if (index >= array.Count)
            {
                Current = null!;
                return false;
            }

            Current = CreateChildItem(parent, array[index], sourceProvider);
            return true;
        }
    }
}