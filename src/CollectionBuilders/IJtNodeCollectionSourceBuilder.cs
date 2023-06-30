using System.Collections.Generic;
using Aadev.JTF.CustomSources;

namespace Aadev.JTF.CollectionBuilders;

internal interface IJtNodeCollectionSourceBuilder
{
    List<IJtSourceStructureElement> Build(JtNodeCollectionSource @this);

}
