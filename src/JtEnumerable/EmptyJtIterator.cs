using System.Collections.Generic;

namespace Aadev.JTF.JtEnumerable
{
    internal sealed class EmptyJtIterator<T> : JtIterator<T>
    {
        public override JtIterator<T> Clone() => new EmptyJtIterator<T>();
        public override bool MoveNext() => false;
        public override List<T> Enumerate() => new List<T>();
        public override IEnumerator<T> GetEnumerator() => this;
    }
}
