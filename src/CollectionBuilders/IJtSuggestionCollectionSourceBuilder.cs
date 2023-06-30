using System.Collections.Generic;
using Aadev.JTF.CustomSources;

namespace Aadev.JTF.CollectionBuilders;

internal interface IJtSuggestionCollectionSourceBuilder<TSuggestion>
{
    List<IJtSuggestionCollectionSourceChild<TSuggestion>> Build(JtSuggestionCollectionSource<TSuggestion> owner);
}
