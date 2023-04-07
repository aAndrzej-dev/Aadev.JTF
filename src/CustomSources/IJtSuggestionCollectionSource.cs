﻿using System;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public interface IJtSuggestionCollectionSource
    {
        bool IsSavable { get; }
        Type SuggestionType { get; }

        IJtSuggestionCollection CreateInstance();
        internal void BuildJson(StringBuilder sb);
    }
}
