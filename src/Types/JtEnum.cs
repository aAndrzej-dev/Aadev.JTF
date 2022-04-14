using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtEnum : JtToken
    {
        private string @default;
        private bool allowCustomValues;
        private string? customValueId;

        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.String;
        /// <inheritdoc/>
        public override JtTokenType Type => JtTokenType.Enum;

        public string Default
        {
            get => @default;
            set
            {
                if (!Values.Contains(value) && !AllowCustomValues)
                    @default = Values.Count > 0 ? (Values[0] ?? string.Empty) : string.Empty;
                else @default = value;
            }
        }
        public IList<string?> Values { get; private set; }
        [DefaultValue(false)] public bool AllowCustomValues { get => allowCustomValues; set => allowCustomValues = value; }

        public string? CustomValueId { get => customValueId; set { if (customValueId == value) return; customValueId = value; Values = new List<string?>((string[])(Template.GetCustomValue(customValueId!))!.Value); } }

        public JtEnum(JTemplate template) : base(template)
        {
            Values = new List<string?>();
            @default = string.Empty;
        }
        internal JtEnum(JObject obj, JTemplate template) : base(obj, template)
        {
            allowCustomValues = (bool)(obj["allowCustom"] ?? obj["canCustom"] ?? false);
            @default = (string?)obj["default"] ?? string.Empty;


            List<string?> vallist = new List<string?>();


            if (obj["values"] is JArray values)
            {
                foreach (JObject item in values)
                {
                    vallist.Add((string?)item["name"]);
                }
                Values = new List<string?>(vallist);
            }
            else if (((JValue?)obj["values"])?.Value is string str)
            {
                if (!str.StartsWith("@"))
                    throw new System.Exception();

                customValueId = str.AsSpan(1).ToString();

                Values = new List<string?>((string[])(Template.GetCustomValue(customValueId!))!.Value);
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
            if (AllowCustomValues)
                sb.Append($"\"allowCustom\": true,");
            if (customValueId is null)
            {
                sb.Append("\"values\": [");

                for (int i = 0; i < Values.Count; i++)
                {
                    if (i != 0)
                        sb.Append(',');

                    sb.Append($"{{\"name\": \"{Values[i]}\"}}");
                }

                sb.Append("],");
            }
            else
            {
                sb.Append($"\"values\": \"{customValueId}\"");

            }




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
            sb.Append($"\"type\": \"{Type.Name}\"");
            sb.Append('}');
        }

        /// <inheritdoc/>
        public override JToken CreateDefaultToken() => new JValue(Default);
    }

}
