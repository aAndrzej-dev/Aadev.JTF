using System.Collections.Generic;

namespace Aadev.JTF.CollectionBuilders
{
    internal interface IJtCollectionBuilder<T>
    {
        List<T> Build();
    }
}
