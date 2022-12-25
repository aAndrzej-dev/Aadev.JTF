using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtSuggestionSource<T> : CustomSource, ISuggestionSource, IJtSuggestionCollectionSourceChild<T>
    {
        public T Value { get; set; }
        public string? DisplayName { get; set; }

        public Type SuggestionType => typeof(T);

        internal JtSuggestionSource(ICustomSourceParent parent, JObject source) : base(parent)
        {
            if (source["name"]?.Type is JTokenType.String && typeof(T) == typeof(string))
                Value = (T)((JValue?)source["name"])?.Value!;
            else
                Value = (T)Convert.ChangeType(((JValue?)source["value"])?.Value, typeof(T), CultureInfo.InvariantCulture)!;
            DisplayName = (string?)source["displayName"] ?? Value?.ToString();
        }

        internal JtSuggestionSource(JtSuggestion<T> jtSuggestion) : base(null!)
        {
            Value = jtSuggestion.Value;
            DisplayName = jtSuggestion.DisplayName;
        }

        internal JtSuggestionSource(JtSuggestionCollectionSource<T> parent, T value, string displayName) : base(parent)
        {
            Value = value;
            DisplayName = displayName;
        }

        public JtSuggestion<T> CreateInstance() => new JtSuggestion<T>(Value, DisplayName);
        internal override void BuildJsonDeclaration(StringBuilder sb)
        {
            sb.Append('{');
            if (Value is string str)
                sb.Append( $"\"value\": \"{str}\"");
            else if (Value is byte || Value is short || Value is int || Value is long || Value is float || Value is double)
                sb.Append( $"\"value\": {Value}");
            else if (Value is bool b)
                sb.Append( $"\"value\": {(b ? "true" : "false")}");
            if (DisplayName?.Equals(Value?.ToString(), StringComparison.Ordinal) is false)
                sb.Append( $", \"displayName\": \"{DisplayName}\"");
            sb.Append('}');
        }

        IJtSuggestionCollectionChild<T> IJtSuggestionCollectionSourceChild<T>.CreateInstance() => CreateInstance();
        void IJtSuggestionCollectionSourceChild<T>.BuildJson(StringBuilder sb) => BuildJson(sb);
    }
    public interface ISuggestionSource
    {
        Type SuggestionType { get; }
    }
}
