using Aadev.JTF.CustomSources;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aadev.JTF
{
    public interface IJtSuggestionCollection
    {
        Type SuggestionType { get; }
        bool IsEmpty { get; }


        internal void BuildJson(StringBuilder sb);
        IJtSuggestionCollectionSource CreateSource(IJtCustomSourceParent parent);
        IEnumerable<IJtSuggestion> GetSuggestions(Func<JtIdentifier, IEnumerable<IJtSuggestion>> dynamicSuggestionsSource);
    }
    public interface IJtSuggestionCollection<TSuggestion> : IJtSuggestionCollection, IJtSuggestionCollectionChild<TSuggestion>, IList<IJtSuggestionCollectionChild<TSuggestion>>
    {

    }
}
