using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public class JtString : JtToken
    {
        private int maxLength;
        private int minLength;

        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.String;
        /// <inheritdoc/>
        public override JtTokenType Type => JtTokenType.String;

        [DefaultValue(0)] public int MinLength { get => minLength; set => minLength = value.Max(0); }
        [DefaultValue(-1)] public int MaxLength { get => maxLength; set => maxLength = value.Max(-1); }
        public string Default { get; set; }
        public JtString(JTemplate template) : base(template)
        {
            MinLength = 0;
            MaxLength = int.MaxValue;
            Default = string.Empty;
        }
        public JtString(JObject obj, JTemplate template) : base(obj, template)
        {
            MinLength = (int)(obj["minLength"] ?? 0);
            MaxLength = (int)(obj["maxLength"] ?? -1);
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
