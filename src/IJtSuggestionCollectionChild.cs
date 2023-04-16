using Aadev.JTF.CustomSources;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aadev.JTF
{
    public interface IJtSuggestionCollectionChild<TSuggestion>
    {
        bool IsStatic { get; }
        bool IsReadOnly { get; }

        internal void BuildJson(StringBuilder sb);
        IEnumerable<IJtSuggestion> GetSuggestions(Func<JtIdentifier, IEnumerable<IJtSuggestion>> dynamicSuggestionsSource);
        IJtSuggestionCollectionSourceChild<TSuggestion> CreateSource(IJtCustomSourceParent parent);

    }
}
