using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Text;

namespace Aadev.JTF
{
    [Serializable]
    public class JtSuggestion<T> : IJtSuggestion
    {
        private T value;

        public T Value
        {
            get => value;
            set
            {
                if (DisplayName?.Equals(Value?.ToString()) is true)
                    DisplayName = value?.ToString();
                this.value = value;
            }
        }
        public string? DisplayName { get; set; }

        public Type ValueType => typeof(T);

        public JtSuggestion() { }
        public JtSuggestion(T value)
        {
            Value = value;
            DisplayName = value?.ToString();
        }
        public JtSuggestion(JObject obj)
        {

            if (((JValue?)obj["name"])?.Value is string && typeof(T) == typeof(string))
                Value = (T)((JValue?)obj["name"])?.Value!;
            else
                Value = (T)Convert.ChangeType(((JValue?)obj["value"])?.Value, typeof(T), CultureInfo.InvariantCulture)!;
            DisplayName = (string?)obj["displayName"] ?? Value?.ToString();
        }
        public override string? ToString() => DisplayName ?? Value?.ToString();

        public void BulidJson(StringBuilder sb)
        {
            sb.Append('{');
            if (Value is string str)
                sb.Append($"\"value\": \"{str}\"");
            else if (Value is byte || Value is short || Value is int || Value is long || Value is float || Value is double)
                sb.Append($"\"value\": {Value}");
            else if (Value is bool b)
                sb.Append($"\"value\": {(b ? "true" : "false")}");
            if (DisplayName?.Equals(Value?.ToString()) is false)
                sb.Append($", \"displayName\": \"{DisplayName}\"");
            sb.Append('}');
        }
        public T1 GetValue<T1>()
        {
            if (Value is T1 v)
                return v;
            throw new Exception();
        }
        public void SetValue<T1>(T1 value)
        {
            if (value is T v)
                Value = v;
            throw new Exception();
        }

        public object? GetValue() => Value;
        public void SetValue(object? value)
        {
            if (value is T v)
                Value = v;
            throw new Exception();
        }
    }
}
