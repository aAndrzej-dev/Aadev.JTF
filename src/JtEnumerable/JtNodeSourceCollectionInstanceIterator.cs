using Aadev.JTF.CustomSources;
using System.Collections.Generic;

namespace Aadev.JTF.JtEnumerable
{
    internal sealed class JtNodeSourceCollectionInstanceIterator : JtIterator<IJtNodeCollectionSourceChild>
    {
        private readonly JtNodeCollection collection;
        private IEnumerator<IJtNodeCollectionChild>? sourceEnumerator;
        private readonly ICustomSourceProvider sourceProvider;

        public JtNodeSourceCollectionInstanceIterator(JtNodeCollection collection, ICustomSourceProvider sourceProvider)
        {
            this.collection = collection;
            this.sourceProvider = sourceProvider;
        }

        public override JtIterator<IJtNodeCollectionSourceChild> Clone() => new JtNodeSourceCollectionInstanceIterator(collection, sourceProvider);
        public override bool MoveNext()
        {
            sourceEnumerator ??= collection.nodeEnumerable.GetEnumerator();

            if (!sourceEnumerator.MoveNext())
            {
                Current = null!;
                return false;
            }

            if (sourceEnumerator.Current is JtNode n)
            {
                Current = n.CreateSource();
                return true;
            }
            if (sourceEnumerator.Current is JtNodeCollection nc)
            {
                Current = nc.CreateSource(sourceProvider);
                return true;
            }
            throw new InternalException();
        }
    }
}