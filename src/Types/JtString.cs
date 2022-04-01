using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public class JtString : JtToken
    {
        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.String;
        /// <inheritdoc/>
        public override JtTokenType Type => JtTokenType.String;

        [DefaultValue(0)] public uint MinLength { get; set; }
        [DefaultValue(int.MaxValue)] public int MaxLength { get; set; }
        public string Default { get; set; }
        public JtString(JTemplate template) : base(template)
        {
            MinLength = 0;
            MaxLength = int.MaxValue;
            Default = string.Empty;
        }
        public JtString(JObject obj, JTemplate template) : base(obj, template)
        {
            MinLength = (uint)(obj["minLength"] ?? 0);
            MaxLength = (int)(obj["maxLength"] ?? int.MaxValue);
            Default = (string?)obj["default"] ?? string.Empty;
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

            if (MinLength != 0)
                sb.Append($"\"minLength\": {MinLength},");
            if (MaxLength != int.MaxValue)
                sb.Append($"\"maxLength\": {MaxLength},");
            if (!string.IsNullOrEmpty(Default))
                sb.Append($"\"default\": \"{Default}\",");

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
