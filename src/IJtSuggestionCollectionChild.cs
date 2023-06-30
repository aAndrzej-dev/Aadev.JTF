using System;
using System.Collections.Generic;
using Aadev.JTF.Common;
using Aadev.JTF.CustomSources;

namespace Aadev.JTF;

public interface IJtSuggestionCollectionChild<TSuggestion> : IJtSuggestionCollectionChild
{
    bool IsStatic { get; }
    bool IsReadOnly { get; }

    IEnumerable<IJtSuggestion> GetSuggestions(Func<JtIdentifier, IEnumerable<IJtSuggestion>> dynamicSuggestionsSource);
    IJtSuggestionCollectionSourceChild<TSuggestion> CreateSource(IJtCustomSourceParent parent);
}
public interface IJtSuggestionCollectionChild : IJtCommonSuggestionCollectionChild
{

}