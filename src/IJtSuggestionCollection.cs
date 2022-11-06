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


        void BuildJson(StringBuilder sb);
        IJtSuggestionCollectionSource CreateSource(ICustomSourceParent parent);
        IEnumerable<IJtSuggestion> GetSuggestions(Func<JtIdentifier, IEnumerable<IJtSuggestion>> dynamicSuggestionsSource);
    }
}
