using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Aadev.JTF.JtEnumerable
{
    internal sealed class JtNodeCollectionOverrideIterator : JtIterator<IJtNodeCollectionChild>
    {
        private readonly IJtNodeParent parent;
        private readonly JtNodeCollectionSource @base;
        private readonly JArray @override;
        private readonly ICustomSourceProvider sourceProvider;
        private IEnumerator<IJtNodeCollectionSourceChild>? sourceEnumerator;
        private int index = -1;

        public JtNodeCollectionOverrideIterator(IJtNodeParent parent, JtNodeCollectionSource @base, JArray @override, ICustomSourceProvider sourceProvider)
        {
            this.parent = parent;
            this.@base = @base;
            this.@override = @override;
            this.sourceProvider = sourceProvider;
        }

        public override JtIterator<IJtNodeCollectionChild> Clone() => new JtNodeCollectionOverrideIterator(parent, @base, @override, sourceProvider);
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
                    Current = JtNodeCollectionIterator.CreateChildItem(@override[index], parent, sourceProvider);
                    return true;
                }
            }
            else if (@override.Count <= index)
            {
                Current = sourceEnumerator.Current.CreateInstance(parent, null);
                return true;
            }
            else if (@override[index]?.Type is JTokenType.Null)
            {
                return MoveNext();
            }
            else
            {
                Current = sourceEnumerator.Current.CreateInstance(parent, @override[index]);
                return true;
            }
        }
    }
}