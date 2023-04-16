using Aadev.JTF.CustomSources;
using System.Collections.Generic;

namespace Aadev.JTF.CollectionBuilders
{
    internal interface IJtSuggestionCollectionSourceBuilder<TSuggestion>
    {
        List<IJtSuggestionCollectionSourceChild<TSuggestion>> Build(JtSuggestionCollectionSource<TSuggestion> owner);
    }
}
