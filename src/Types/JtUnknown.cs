using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.Types
{
    public class JtUnknown : JtToken
    {
        public override JTokenType JsonType => JTokenType.None;
        public override JtTokenType Type => JtTokenType.Unknown;

        public JtUnknown(JObject obj, JTemplate template) : base(obj, template)
        {
        }
        public JtUnknown(JTemplate template) : base(template)
        {

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
