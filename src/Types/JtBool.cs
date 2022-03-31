using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public class JtBool : JtToken
    {
        public override JTokenType JsonType => JTokenType.Boolean;
        public override JtTokenType Type => JtTokenType.Bool;

        [DefaultValue(false)] public bool Default { get; set; }

        public JtBool(JTemplate template) : base(template)
        {
            Default = false;
        }
        public JtBool(JObject obj, JTemplate template) : base(obj, template)
        {
            Default = (bool)(obj["default"] ?? false);
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

            if (Default)
                sb.Append($"\"default\": true,");

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
