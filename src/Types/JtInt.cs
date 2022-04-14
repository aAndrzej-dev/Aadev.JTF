using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtInt : JtToken
    {
        private const int minValue = int.MinValue;
        private const int maxValue = int.MaxValue;
        private int @default;
        private int min;
        private int max;
        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.Integer;
        /// <inheritdoc/>
        public override JtTokenType Type => JtTokenType.Int;


        [DefaultValue(minValue)] public int Min { get => min; set { min = value.Min(Max); @default = value.Clamp(Min, Max); } }
        [DefaultValue(maxValue)] public int Max { get => max; set { max = value.Max(Min); @default = value.Clamp(Min, Max); } }
        [DefaultValue(0)] public int Default { get => @default; set => @default = value.Clamp(Min, Max); }
        public JtInt(JTemplate template) : base(template)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;
        }
        internal JtInt(JObject obj, JTemplate template) : base(obj, template)
        {
            Min = (int)(obj["min"] ?? minValue);
            Max = (int)(obj["max"] ?? maxValue);
            Default = (int)(obj["default"] ?? 0);
        }

        internal override void BulidJson(StringBuilder sb)
        {
            sb.Append('{');
            if (!IsArrayPrefab)
                sb.Append($"\"name\": \"{Name}\",");
            if (!string.IsNullOrWhiteSpace(Description))
                sb.Append($"\"description\": \"{Description}\",");
            if (DisplayName != Name)
                sb.Append($"\"displayName\": \"{DisplayName}\",");

            if (Min != minValue)
                sb.Append($"\"min\": {Min},");
            if (Max != maxValue)
                sb.Append($"\"max\": {Max},");
            if (Default != 0)
                sb.Append($"\"default\": {Default},");

            if (Conditions.Count > 0)
            {
                sb.Append("\"conditions\": [");

                for (int i = 0; i < Conditions.Count; i++)
                {
                    if (i != 0)
                        sb.Append(',');

                    sb.Append(Conditions[i].GetString());
                }

                sb.Append("],");
            }

            sb.Append($"\"id\": \"{Id}\",");
            sb.Append($"\"type\": \"{Type.Name}\"");
            sb.Append('}');
        }

        /// <inheritdoc/>
        public override JToken CreateDefaultToken() => new JValue(Default);
    }
}