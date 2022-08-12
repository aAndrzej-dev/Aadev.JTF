using System;
using System.Collections.Generic;

namespace Aadev.JTF
{
    public interface IJtSuggestionCollection : IJtCollection, IList<IJtSuggestion>
    {
        Type ValueType { get; }
    }
}
