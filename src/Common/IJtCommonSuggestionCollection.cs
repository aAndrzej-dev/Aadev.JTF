using System;
using System.Collections.Generic;
using System.ComponentModel;
using Aadev.JTF.CustomSources;

namespace Aadev.JTF.Common;

[Editor("Aadev.JTF.Design.JtSuggestionCollectionEditor, Aadev.JTF.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4bb879fd89b07a65", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
public interface IJtCommonSuggestionCollection : IJtJsonBuildable, IHaveCustomSourceProvider
{
    JtSourceReference DynamicSourceId { get; set; }
    IJtSuggestionCollectionSource? Base { get; set; }
    List<IJtCommonSuggestionCollectionChild> Suggestions { get; }
    Type SuggestionType { get; }
    bool IsEmpty { get; }
    IdentifiersManager IdentifiersManager { get; }
    IJtCommonRoot Root { get; }

    IJtCommonSuggestion AddNewSuggestion(object? value, string? displayName = null);
    IJtCommonSuggestionCollection AddNewSuggestionCollection();
}
