using System;

namespace Aadev.JTF.CustomSources
{
    public interface IJtSuggestionSource
    {
        Type SuggestionType { get; }
        string? DisplayName { get; set; }
        string? StringValue { get; }

        T GetValue<T>();
        void SetValue<T>(T value);
        object? GetValue();
        void SetValue(object? value);
    }
    public interface IJtSuggestionSource<TSuggestion> : IJtSuggestionSource, IJtSuggestionCollectionSourceChild<TSuggestion>
    {
        TSuggestion Value { get; set; }
    }
}
