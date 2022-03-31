using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public class JtEnum : JtToken
    {
        public override JTokenType JsonType => JTokenType.String;
        public override JtTokenType Type => JtTokenType.Enum;

        public string Default { get; set; }
        public List<string?> Values { get; }
        [DefaultValue(false)] public bool CanUseCustomValue { get; set; }

        public JtEnum(JTemplate template) : base(template)
        {
            Values = new List<string?>();
            Default = string.Empty;
        }
        public JtEnum(JObject obj, JTemplate template) : base(obj, template)
        {
            Values = new List<string?>();

            if (IsUsingCustomType) obj = CustomType?.Object!;


            CanUseCustomValue = (bool)(obj["canCustom"] ?? false);
            Default = (string?)obj["default"] ?? string.Empty;

            JArray? values = obj["values"] as JArray;

            if (values is null)
            {
                return;
            }
            foreach (JObject item in values)
            {
                Values.Add((string?)item["name"]);
            }


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

            if (!string.IsNullOrEmpty(Default))
                sb.Append($"\"default\": \"{Default}\",");
            if (CanUseCustomValue)
                sb.Append($"\"canCustom\": true,");

            sb.Append("\"values\": [");

            for (int i = 0; i < Values.Count; i++)
            {
                if (i != 0)
                    sb.Append(',');

                sb.Append($"\"name\": \"{Values}\"");
            }

            sb.Append("],");



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
