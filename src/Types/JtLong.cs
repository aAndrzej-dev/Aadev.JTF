using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public class JtLong : JtToken
    {
        private const long minValue = long.MinValue;
        private const long maxValue = long.MaxValue;

        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.Integer;
        /// <inheritdoc/>
        public override JtTokenType Type => JtTokenType.Long;


        [DefaultValue(minValue)] public long Min { get; set; }
        [DefaultValue(maxValue)] public long Max { get; set; }
        [DefaultValue(0)] public long Default { get; set; }
        public JtLong(JTemplate template) : base(template)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;
        }
        public JtLong(JObject obj, JTemplate template) : base(obj, template)
        {
            Min = (long)(obj["min"] ?? minValue);
            Max = (long)(obj["max"] ?? maxValue);
            Default = (long)(obj["default"] ?? 0);
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