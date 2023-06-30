using System;
using System.Collections.Generic;
using Aadev.JTF.Common;
using Aadev.JTF.CustomSources;
using Aadev.JTF.Types;

namespace Aadev.JTF;


public interface IJtSuggestionCollection : IJtCommonSuggestionCollection
{
    JtValueNode Owner { get; }

    IJtSuggestionCollectionSource CreateSource(IJtCustomSourceParent parent);
    IEnumerable<IJtSuggestion> GetSuggestions(Func<JtIdentifier, IEnumerable<IJtSuggestion>> dynamicSuggestionsSource);
}
public interface IJtSuggestionCollection<TSuggestion> : IJtSuggestionCollection, IJtSuggestionCollectionChild<TSuggestion>, IList<IJtSuggestionCollectionChild<TSuggestion>>
{
}
