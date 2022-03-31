using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public class JtDouble : JtToken
    {
        private const double minValue = double.MinValue;
        private const double maxValue = double.MaxValue;



        public override JTokenType JsonType => JTokenType.Float;
        public override JtTokenType Type => JtTokenType.Double;


        [DefaultValue(minValue)] public double Min { get; set; }
        [DefaultValue(maxValue)] public double Max { get; set; }
        [DefaultValue(0)] public double Default { get; set; }
        public JtDouble(JTemplate template) : base(template)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;
        }
        public JtDouble(JObject obj, JTemplate template) : base(obj, template)
        {
            Min = (double)(obj["min"] ?? minValue);
            Max = (double)(obj["max"] ?? maxValue);
            Default = (double)(obj["default"] ?? 0);
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