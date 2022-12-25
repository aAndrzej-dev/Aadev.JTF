using Aadev.JTF.CustomSources;
using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Aadev.JTF
{
    [Serializable]
    public sealed class JtSuggestion<T> : IJtSuggestion, IJtSuggestionCollectionChild<T>, IEquatable<JtSuggestion<T>?>
    {
        private T value;

        public T Value
        {
            get => value;
            set
            {
                if (DisplayName?.Equals(Value?.ToString(),StringComparison.Ordinal) is true)
                    DisplayName = value?.ToString();
                this.value = value;
            }
        }
        public string? DisplayName { get; set; }

        public Type SuggestionType => typeof(T);

        public bool IsEmpty => true;

        public string? StringValue => Value?.ToString();

        public bool IsStatic => true;

        public JtSuggestion(T value, string? displayName = null)
        {
            this.value = value;
            DisplayName = displayName ?? value?.ToString();
        }
        public JtSuggestion(JObject source)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            if (source["name"]?.Type is JTokenType.String && typeof(T) == typeof(string))
                value = (T)((JValue?)source["name"])?.Value!;
            else
                value = (T)Convert.ChangeType(((JValue?)source["value"])?.Value, typeof(T), CultureInfo.InvariantCulture)!;
            DisplayName = (string?)source["displayName"] ?? value?.ToString();
        }
        public override string? ToString() => DisplayName ?? Value?.ToString();

        void IJtSuggestionCollectionChild<T>.BuildJson(StringBuilder sb)
        {
            sb.Append('{');
            if (Value is string str)
                sb.Append($"\"value\": \"{str}\"");
            else if (Value is byte || Value is short || Value is int || Value is long || Value is float || Value is double)
                sb.Append($"\"value\": {Value}");
            else if (Value is bool b)
                sb.Append($"\"value\": {(b ? "true" : "false")}");
            if (DisplayName?.Equals(Value?.ToString(), StringComparison.Ordinal) is false)
                sb.Append($", \"displayName\": \"{DisplayName}\"");
            sb.Append('}');
        }
        T1 IJtSuggestion.GetValue<T1>()
        {
            if (Value is T1 v)
                return v;
            throw new InvalidCastException($"Cannot convert {typeof(T1)} to {typeof(T)}");
        }
        void IJtSuggestion.SetValue<T1>(T1 value)
        {
            if (value is T v)
                Value = v;
            throw new InvalidCastException($"Cannot convert {typeof(T1)} to {typeof(T)}");
        }

        object? IJtSuggestion.GetValue() => Value;
        void IJtSuggestion.SetValue(object? value)
        {
            if (value is T v)
                Value = v;
            throw new InvalidCastException($"Cannot convert {value?.GetType()} to {typeof(T)}");
        }

        IEnumerable<IJtSuggestion> IJtSuggestionCollectionChild<T>.GetSuggestions(Func<JtIdentifier, IEnumerable<IJtSuggestion>> dynamicSuggestionsSource) 
        {
            yield return this;
        }

        public override bool Equals(object? obj) => Equals(obj as JtSuggestion<T>);
        public bool Equals(JtSuggestion<T>? other) => !(other is null) && EqualityComparer<T>.Default.Equals(Value, other.Value) && DisplayName == other.DisplayName;
        public override int GetHashCode() => HashCode.Combine(Value, DisplayName);

        public JtSuggestionSource<T> CreateSource() => new JtSuggestionSource<T>(this);
        IJtSuggestionCollectionSourceChild<T> IJtSuggestionCollectionChild<T>.CreateSource(ICustomSourceParent parent) => CreateSource();

        public static bool operator ==(JtSuggestion<T>? left, JtSuggestion<T>? right) => !(left is null || right is null) && EqualityComparer<JtSuggestion<T>>.Default.Equals(left, right);
        public static bool operator !=(JtSuggestion<T>? left, JtSuggestion<T>? right) => !(left == right);
    }
}
