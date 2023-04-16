﻿using System;

namespace Aadev.JTF
{
    public interface IJtSuggestion
    {
        Type SuggestionType { get; }
        string? DisplayName { get; set; }
        string? StringValue { get; }

        bool IsReadOnly { get; }

        T GetValue<T>();
        void SetValue<T>(T value);
        object? GetValue();
        void SetValue(object? value);
    }
    public interface IJtSuggestion<TSuggestion> : IJtSuggestion, IJtSuggestionCollectionChild<TSuggestion>
    {
        public TSuggestion Value { get; set; }
    }
}
