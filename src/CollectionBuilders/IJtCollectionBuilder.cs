using System.Collections.Generic;

namespace Aadev.JTF.CollectionBuilders
{
    internal interface IJtCollectionBuilder<TResult>
    {
        List<TResult> Build();
    }
}
