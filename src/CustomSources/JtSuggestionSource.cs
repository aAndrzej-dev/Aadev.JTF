using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtSuggestionSource<TSuggestion> : CustomSource, IJtSuggestionSource<TSuggestion>
    {
        private JtSuggestionSourceInstance<TSuggestion>? instance;

        public TSuggestion Value { get; set; }
        public string? DisplayName { get; set; }

        public Type SuggestionType => typeof(TSuggestion);

        public override bool IsExternal => false;

        public string? StringValue => Value?.ToString();


        internal JtSuggestionSource(JtSuggestionCollectionSource<TSuggestion> parent, JObject source) : base(parent)
        {
            if (source["name"]?.Type is JTokenType.String && typeof(TSuggestion) == typeof(string))
                Value = (TSuggestion)((JValue?)source["name"])?.Value!;
            else
                Value = (TSuggestion)Convert.ChangeType(((JValue?)source["value"])?.Value, typeof(TSuggestion), CultureInfo.InvariantCulture)!;
            DisplayName = (string?)source["displayName"] ?? Value?.ToString();
        }

        internal JtSuggestionSource(JtSuggestion<TSuggestion> jtSuggestion) : base(null!)
        {
            Value = jtSuggestion.Value;
            DisplayName = jtSuggestion.DisplayName;
        }

        internal JtSuggestionSource(JtSuggestionCollectionSource<TSuggestion> parent, TSuggestion value, string displayName) : base(parent)
        {
            Value = value;
            DisplayName = displayName;
        }

        public IJtSuggestion<TSuggestion> CreateInstance() => instance ??= new JtSuggestionSourceInstance<TSuggestion>(this);
        internal override void BuildJsonDeclaration(StringBuilder sb)
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

        IJtSuggestionCollectionChild<TSuggestion> IJtSuggestionCollectionSourceChild<TSuggestion>.CreateInstance() => CreateInstance();
        void IJtSuggestionCollectionSourceChild<TSuggestion>.BuildJson(StringBuilder sb) => BuildJson(sb);
        T IJtSuggestionSource.GetValue<T>()
        {
            if (Value is T v)
                return v;
            throw new InvalidCastException($"Cannot convert {typeof(T)} to {typeof(TSuggestion)}");
        }
        void IJtSuggestionSource.SetValue<T>(T value)
        {
            if (value is TSuggestion v)
                Value = v;
            throw new InvalidCastException($"Cannot convert {typeof(T)} to {typeof(TSuggestion)}");
        }

        object? IJtSuggestionSource.GetValue() => Value;
        void IJtSuggestionSource.SetValue(object? value)
        {
            if (value is TSuggestion v)
                Value = v;
            throw new InvalidCastException($"Cannot convert {value?.GetType()} to {typeof(TSuggestion)}");
        }
    }
}
