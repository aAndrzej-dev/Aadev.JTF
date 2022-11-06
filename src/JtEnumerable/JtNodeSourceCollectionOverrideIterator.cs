using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Aadev.JTF.JtEnumerable
{
    internal sealed class JtNodeSourceCollectionOverrideIterator : JtIterator<IJtNodeCollectionSourceChild>
    {
        private readonly ICustomSourceParent parent;
        private readonly JArray @override;
        private readonly JtNodeCollectionSource @base;
        private readonly ICustomSourceProvider sourceProvider;
        private IEnumerator<IJtNodeCollectionSourceChild>? sourceEnumerator;
        private int index = -1;

        public JtNodeSourceCollectionOverrideIterator(ICustomSourceParent parent, JtNodeCollectionSource @base, JArray @override, ICustomSourceProvider sourceProvider)
        {
            this.@override = @override;
            this.@base = @base;
            this.sourceProvider = sourceProvider;
            this.parent = parent;
        }

        public override JtIterator<IJtNodeCollectionSourceChild> Clone() => new JtNodeSourceCollectionOverrideIterator(parent, @base, @override, sourceProvider);
        public override bool MoveNext()
        {
            sourceEnumerator ??= @base.nodeEnumerable.Enumerate().GetEnumerator();
            index++;
            if (!sourceEnumerator.MoveNext())
            {
                if (@override.Count <= index)
                {
                    Current = null!;
                    return false;
                }
                else
                {
                    Current = new CustomSourceBaseDeclaration(@override[index], sourceProvider).Value;
                    return true;
                }
            }
            else if (@override.Count <= index)
            {
                if (sourceEnumerator.Current is IJtNodeCollectionSourceChild child)
                {
                    Current = child;
                    return true;
                }
                throw new InternalException();
            }
            else if (@override[index]?.Type is JTokenType.Null)
            {
                return MoveNext();
            }
            else
            {
                if (sourceEnumerator.Current is JtNodeSource ns)
                {
                    Current = ns.CreateOverride(parent, (JObject?)@override[index]);
                    return true;
                }
                if (sourceEnumerator.Current is JtNodeCollectionSource ncs)
                {
                    Current = ncs.CreateOverride(parent, (JArray?)@override[index]);
                    return true;
                }
                throw new InternalException();
            }

        }
    }
}