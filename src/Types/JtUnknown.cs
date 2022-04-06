﻿using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.Types
{
    public class JtUnknown : JtToken
    {
        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.None;
        /// <inheritdoc/>
        public override JtTokenType Type => JtTokenType.Unknown;

        /// <inheritdoc/>
        public JtUnknown(JObject obj, JTemplate template) : base(obj, template)
        {
        }
        /// <inheritdoc/>
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
