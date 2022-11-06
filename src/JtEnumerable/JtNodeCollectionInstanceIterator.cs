using Aadev.JTF.CustomSources;
using System.Collections.Generic;

namespace Aadev.JTF.JtEnumerable
{
    internal sealed class JtNodeCollectionInstanceIterator : JtIterator<IJtNodeCollectionChild>
    {
        private readonly IJtNodeParent parent;
        private readonly JtNodeCollectionSource source;
        private IEnumerator<IJtNodeCollectionSourceChild>? sourceEnumerator;


        public JtNodeCollectionInstanceIterator(IJtNodeParent parent, JtNodeCollectionSource source)
        {
            this.parent = parent;
            this.source = source;
        }



        public override JtIterator<IJtNodeCollectionChild> Clone() => new JtNodeCollectionInstanceIterator(parent, source);

        public override bool MoveNext()
        {
            sourceEnumerator ??= source.nodeEnumerable.GetEnumerator();

            if (!sourceEnumerator.MoveNext())
            {
                Current = null!;
                return false;
            }

            Current = sourceEnumerator.Current.CreateInstance(parent, null);
            return true;
        }

    }
}