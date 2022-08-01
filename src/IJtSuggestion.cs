using System;
using System.Text;

namespace Aadev.JTF
{
    public interface IJtSuggestion
    {
        Type ValueType { get; }
        string? DisplayName { get; set; }
        void BulidJson(StringBuilder sb);
        T GetValue<T>();
        void SetValue<T>(T value);
        object? GetValue();
        void SetValue(object? value);
    }
}
