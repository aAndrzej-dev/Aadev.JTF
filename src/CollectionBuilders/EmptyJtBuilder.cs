using System.Collections.Generic;

namespace Aadev.JTF.CollectionBuilders
{
    internal sealed class EmptyJtBuilder<TResult> : IJtCollectionBuilder<TResult>
    {
        public static EmptyJtBuilder<TResult> Instance = new EmptyJtBuilder<TResult>();

        public List<TResult> Build() => new List<TResult>();

        private EmptyJtBuilder() { }
    }
}
