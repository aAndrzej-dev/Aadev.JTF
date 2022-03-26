using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.Types
{
    public class JtFloat : JtToken
    {
        private const float minValue = float.MinValue;
        private const float maxValue = float.MaxValue;


        public override JTokenType JsonType => JTokenType.Float;
        public override JtTokenType Type => JtTokenType.Float;


        public float Min { get; set; }
        public float Max { get; set; }
        public float Default { get; set; }
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
                sb.Append("\"if\": [");

                for (int i = 0; i < Conditions.Count; i++)
                {
                    if (i != 0)
                        sb.Append(',');

                    sb.Append(Conditions[i].GetString());
                }

                sb.Append("],");
            }

            sb.Append($"\"id\": \"{Id}\"");
            sb.Append($"\"type\": \"{Type.Name}\"");
            sb.Append('}');
        }
    }
}
