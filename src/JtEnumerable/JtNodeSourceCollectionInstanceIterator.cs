using Aadev.JTF.CustomSources;
using System.Collections.Generic;

namespace Aadev.JTF.JtEnumerable
{
    internal sealed class JtNodeSourceCollectionInstanceIterator : JtIterator<IJtNodeCollectionSourceChild>
    {
        private readonly JtNodeCollection instance;
        private IEnumerator<IJtNodeCollectionChild>? sourceEnumerator;

        public JtNodeSourceCollectionInstanceIterator(JtNodeCollection instance)
        {
            this.instance = instance;
        }

        public override JtIterator<IJtNodeCollectionSourceChild> Clone() => new JtNodeSourceCollectionInstanceIterator(instance);
        public override bool MoveNext()
        {
            sourceEnumerator ??= instance.nodeEnumerable.Enumerate().GetEnumerator();

            if (!sourceEnumerator.MoveNext())
            {
                Current = null!;
                return false;
            }
            Current = sourceEnumerator.Current.CreateSource();
            return true;
        }
    }
}