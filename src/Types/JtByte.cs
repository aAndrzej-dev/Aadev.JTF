using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public class JtByte : JtToken
    {
        private const byte minValue = byte.MinValue;
        private const byte maxValue = byte.MaxValue;



        public override JTokenType JsonType => JTokenType.Integer;
        public override JtTokenType Type => JtTokenType.Byte;


        [DefaultValue(minValue)] public byte Min { get; set; }
        [DefaultValue(maxValue)] public byte Max { get; set; }
        [DefaultValue(0)] public byte Default { get; set; }

        public JtByte(JTemplate template) : base(template)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;
        }
        public JtByte(JObject obj, JTemplate template) : base(obj, template)
        {
            Min = (byte)(obj["min"] ?? minValue);
            Max = (byte)(obj["max"] ?? maxValue);
            Default = (byte)(obj["default"] ?? 0);
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
