using Aadev.JTF.CustomSources;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aadev.JTF
{
    internal interface IJtSuggestionCollectionChild<T>
    {
        bool IsEmpty { get; }
        bool IsStatic { get; }

        internal void BuildJson(StringBuilder sb);
        IEnumerable<IJtSuggestion> GetSuggestions(Func<JtIdentifier, IEnumerable<IJtSuggestion>> dynamicSuggestionsSource);
        IJtSuggestionCollectionSourceChild<T> CreateSource(ICustomSourceParent parent);
    }
}
