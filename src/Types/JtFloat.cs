using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public class JtFloat : JtToken
    {
        private const float minValue = float.MinValue;
        private const float maxValue = float.MaxValue;
        private float @default;
        private float min;
        private float max;

        public override JTokenType JsonType => JTokenType.Float;
        public override JtTokenType Type => JtTokenType.Float;


        [DefaultValue(minValue)] public float Min { get => min; set { min = value.Min(Max); @default = value.Clamp(Min, Max); } }
        [DefaultValue(maxValue)] public float Max { get => max; set { max = value.Max(Min); @default = value.Clamp(Min, Max); } }
        [DefaultValue(0)] public float Default { get => @default; set => @default = value.Clamp(Min, Max); }
        public JtFloat(JTemplate template) : base(template)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;
        }
        public JtFloat(JObject obj, JTemplate template) : base(obj, template)
        {
            Min = (float)(obj["min"] ?? minValue);
            Max = (float)(obj["max"] ?? maxValue);
            Default = (float)(obj["default"] ?? 0);
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
            if (IsUsingCustomType)
                sb.Append($"\"type\": \"{CustomType}\"");
            else
                sb.Append($"\"type\": \"{Type.Name}\"");
            sb.Append('}');
        }
    }
}
