﻿using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Aadev.JTF
{
    [Serializable]
    public sealed class JtSuggestion<TSuggestion> : IJtSuggestion<TSuggestion>, IEquatable<JtSuggestion<TSuggestion>?>
    {
        private TSuggestion value;

        public TSuggestion Value
        {
            get => value;
            set
            {
                if (DisplayName?.Equals(Value?.ToString(), StringComparison.Ordinal) is true)
                    DisplayName = value?.ToString();
                this.value = value;
            }
        }
        public string? DisplayName { get; set; }

        public Type SuggestionType => typeof(TSuggestion);


        public string? StringValue => Value?.ToString();

        bool IJtSuggestionCollectionChild<TSuggestion>.IsStatic => true;

        public bool IsReadOnly => false;

        public JtSuggestion(TSuggestion value, string? displayName = null)
        {
            this.value = value;
            DisplayName = displayName ?? value?.ToString();
        }
        public JtSuggestion(JObject source)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            if (source["name"]?.Type is JTokenType.String && typeof(TSuggestion) == typeof(string))
                value = (TSuggestion)((JValue?)source["name"])?.Value!;
            else
                value = (TSuggestion)Convert.ChangeType(((JValue?)source["value"])?.Value, typeof(TSuggestion), CultureInfo.InvariantCulture)!;
            DisplayName = (string?)source["displayName"] ?? value?.ToString();

        }
        public override string? ToString() => DisplayName ?? StringValue;

        void IJtSuggestionCollectionChild<TSuggestion>.BuildJson(StringBuilder sb)
        {
            sb.Append('{');
            if (Value is string str)
                sb.Append($"\"value\": \"{str}\"");
            else if (Value is byte or short or int or long or float or double)
                sb.Append($"\"value\": {Value}");
            else if (Value is bool b)
                sb.Append($"\"value\": {(b ? "true" : "false")}");
            if (DisplayName?.Equals(Value?.ToString(), StringComparison.Ordinal) is false)
                sb.Append($", \"displayName\": \"{DisplayName}\"");
            sb.Append('}');
        }
        T IJtSuggestion.GetValue<T>()
        {
            if (Value is T v)
                return v;
            throw new InvalidCastException($"Cannot convert {typeof(T)} to {typeof(TSuggestion)}");
        }
        void IJtSuggestion.SetValue<T>(T value)
        {
            if (value is TSuggestion v)
                Value = v;
            throw new InvalidCastException($"Cannot convert {typeof(T)} to {typeof(TSuggestion)}");
        }

        object? IJtSuggestion.GetValue() => Value;
        void IJtSuggestion.SetValue(object? value)
        {
            if (value is TSuggestion v)
                Value = v;
            throw new InvalidCastException($"Cannot convert {value?.GetType()} to {typeof(TSuggestion)}");
        }

        IEnumerable<IJtSuggestion> IJtSuggestionCollectionChild<TSuggestion>.GetSuggestions(Func<JtIdentifier, IEnumerable<IJtSuggestion>> dynamicSuggestionsSource)
        {
            yield return this;
        }

        public override bool Equals(object? obj) => Equals(obj as JtSuggestion<TSuggestion>);
        public bool Equals(JtSuggestion<TSuggestion>? other) => other is not null && EqualityComparer<TSuggestion>.Default.Equals(Value, other.Value) && DisplayName == other.DisplayName;
        public override int GetHashCode() => HashCode.Combine(Value, DisplayName);

        public JtSuggestionSource<TSuggestion> CreateSource() => new JtSuggestionSource<TSuggestion>(this);
        IJtSuggestionCollectionSourceChild<TSuggestion> IJtSuggestionCollectionChild<TSuggestion>.CreateSource(IJtCustomSourceParent parent) => CreateSource();


        public static bool operator ==(JtSuggestion<TSuggestion>? left, JtSuggestion<TSuggestion>? right) => !(left is null || right is null) && EqualityComparer<JtSuggestion<TSuggestion>>.Default.Equals(left, right);
        public static bool operator !=(JtSuggestion<TSuggestion>? left, JtSuggestion<TSuggestion>? right) => !(left == right);
    }
}
