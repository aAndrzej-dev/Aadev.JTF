using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public class JtShort : JtToken
    {
        private const short minValue = short.MinValue;
        private const short maxValue = short.MaxValue;



        public override JTokenType JsonType => JTokenType.Integer;
        public override JtTokenType Type => JtTokenType.Short;


        [DefaultValue(minValue)] public short Min { get; set; }
        [DefaultValue(maxValue)] public short Max { get; set; }
        [DefaultValue(0)] public short Default { get; set; }
        public JtShort(JTemplate template) : base(template)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;
        }
        public JtShort(JObject obj, JTemplate template) : base(obj, template)
        {
            Min = (short)(obj["min"] ?? minValue);
            Max = (short)(obj["max"] ?? maxValue);
            Default = (short)(obj["default"] ?? 0);
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