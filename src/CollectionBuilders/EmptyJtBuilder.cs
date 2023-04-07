using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Aadev.JTF.CollectionBuilders
{
    internal sealed class EmptyJtBuilder<T> : IJtCollectionBuilder<T>
    {
        public static EmptyJtBuilder<T> Instance = new EmptyJtBuilder<T>();

        public List<T> Build() => new List<T>();

        private EmptyJtBuilder() { }
    }
}
