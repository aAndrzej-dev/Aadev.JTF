using System;

namespace Aadev.JTF.Common;
public interface IJtCommonSuggestion
{
    Type SuggestionType { get; }
    string? DisplayName { get; set; }
    string? StringValue { get; }

    T GetValue<T>();
    object? GetValue();
    void SetValue<T>(T value);
    void SetValue(object? value);
}
