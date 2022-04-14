using Newtonsoft.Json.Linq;
using System;

namespace Aadev.JTF
{
    [Serializable]
    public class JtCondition : IEquatable<JtCondition>
    {

        public string? VariableId { get; set; }
        public ConditionType Type { get; set; }
        public string? Value { get; set; }
        public bool IgnoreCase { get; set; }

        public JtCondition(JObject obj)
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));
            VariableId = (string?)obj["id"];
            Value = (string?)obj["value"];

            if (!Enum.TryParse((string?)obj["type"], true, out ConditionType type))
            {
                throw new Exception($"Invalid condition type `{obj["type"]}`");
            }

            Type = type;

        }

        public JtCondition()
        {
            Type = ConditionType.Equal;
        }


        public override string ToString() => VariableId + (Type == ConditionType.Equal ? " = " : (Type == ConditionType.NotEqual ? " != " : (Type == ConditionType.Less ? " < " : " > "))) + Value;
        public string GetString() => $"{{ \"id\": \"{VariableId}\", \"type\": \"{Type.ToString().ToLower()}\", \"value\": \"{Value}\" }}";


        public JObject ToJObject() => new JObject()
        {
            ["id"] = VariableId,
            ["value"] = Value,
            ["type"] = Type.ToString().ToLower()
        };

        public bool Check(string? value) => Type switch
        {
            ConditionType.Equal => value.Compare(Value, IgnoreCase),
            ConditionType.NotEqual => !value.Compare(Value, IgnoreCase),
            ConditionType.Less => double.TryParse(value, out double tmp1) && double.TryParse(Value, out double tmp2) && tmp1 < tmp2,
            ConditionType.Bigger => double.TryParse(value, out double tmp1) && double.TryParse(Value, out double tmp2) && tmp1 > tmp2,
            _ => false
        };

        public override bool Equals(object? obj) => obj is JtCondition condition && Equals(condition);
        public bool Equals(JtCondition? other) => VariableId == other?.VariableId && Type == other?.Type && Value == other?.Value;

        public override int GetHashCode() => HashCode.Combine(VariableId, Type, Value);

        public static bool operator ==(JtCondition left, JtCondition right) => left.Equals(right);
        public static bool operator !=(JtCondition left, JtCondition right) => !(left == right);
    }
    public enum ConditionType
    {
        Equal,
        NotEqual,
        Less,
        Bigger
    }
}