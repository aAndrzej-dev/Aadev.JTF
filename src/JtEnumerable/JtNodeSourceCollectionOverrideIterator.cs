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
        private IEnumerator<IJtNodeCollectionSourceChild>? sourceEnumerator;
        private int index = -1;

        public JtNodeSourceCollectionOverrideIterator(ICustomSourceParent parent, JtNodeCollectionSource @base, JArray @override)
        {
            this.@override = @override;
            this.@base = @base;
            this.parent = parent;
        }

        public override JtIterator<IJtNodeCollectionSourceChild> Clone() => new JtNodeSourceCollectionOverrideIterator(parent, @base, @override);
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
                    Current = new CustomSourceBaseDeclaration(@override[index], parent.SourceProvider).Value;
                    return true;
                }
            }
            else if (@override.Count <= index)
            {
                Current = sourceEnumerator.Current;
                return true;
            }
            else if (@override[index]?.Type is JTokenType.Null)
            {
                return MoveNext();
            }
            else
            {
                Current =  sourceEnumerator.Current.CreateOverride(parent, @override[index]);
                return true;
            }

        }
    }
}