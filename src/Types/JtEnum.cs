using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtEnum : JtNode
    {
        private string @default;
        private bool allowCustomValues;
        private string? customValueId;

        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.String;
        /// <inheritdoc/>
        public override JtNodeType Type => JtNodeType.Enum;

        public string Default
        {
            get => @default;
            set
            {
                if (!Values.Any(x => x.Name == value) && !AllowCustomValues)
                    @default = Values.Count > 0 ? Values[0].Name : string.Empty;
                else @default = value;
            }
        }
        public List<EnumValue> Values { get; private set; }
        [DefaultValue(false)] public bool AllowCustomValues { get => allowCustomValues; set => allowCustomValues = value; }

        public string? CustomValueId { get => customValueId; set { if (customValueId == value) return; customValueId = value; Values = new List<EnumValue>((EnumValue[])(Template.GetCustomValue(customValueId!))!.Value); } }

        public JtEnum(JTemplate template) : base(template)
        {
            Values = new List<EnumValue>();
            @default = string.Empty;
        }
        internal JtEnum(JObject obj, JTemplate template) : base(obj, template)
        {
            allowCustomValues = (bool)(obj["allowCustom"] ?? obj["canCustom"] ?? false);
            @default = (string?)obj["default"] ?? string.Empty;


            List<EnumValue> vallist = new List<EnumValue>();


            if (obj["values"] is JArray values)
            {
                foreach (JObject item in values)
                {
                    vallist.Add(new EnumValue((string?)item["name"], (string?)item["displayName"]));
                }
                Values = new List<EnumValue>(vallist);
            }
            else if (((JValue?)obj["values"])?.Value is string str)
            {
                if (!str.StartsWith("@"))
                    throw new System.Exception("Custom values name must starts with '@'");

                customValueId = str.AsSpan(1).ToString();

                Values = new List<EnumValue>((EnumValue[])(Template.GetCustomValue(customValueId!))!.Value);

            }
            else
            {
                Values = new List<EnumValue>();
            }
        }
        public override bool HasExternalSources => !(CustomValueId is null);
        internal override void BulidJson(StringBuilder sb)
        {
            BuildCommonJson(sb);

            if (!string.IsNullOrEmpty(Default))
                sb.Append($", \"default\": \"{Default}\"");
            if (AllowCustomValues)
                sb.Append($", \"allowCustom\": true");
            if (customValueId is null)
            {
                if (Values.Count > 0)
                {
                    sb.Append(", \"values\": [");

                    for (int i = 0; i < Values.Count; i++)
                    {
                        if (i != 0)
                            sb.Append(',');
                        sb.Append('{');
                        sb.Append($"\"name\": \"{Values[i].Name}\"");
                        if (!(Values[i].DisplayName is null))
                            sb.Append($", \"displayName\": \"{Values[i].DisplayName}\"");
                        sb.Append('}');
                    }

                    sb.Append(']');
                }

            }
            else
            {
                sb.Append($", \"values\": \"@{customValueId}\"");

            }
            sb.Append('}');
        }

        /// <inheritdoc/>
        public override JToken CreateDefaultValue() => new JValue(Default);

        public struct EnumValue
        {
            public string Name { get; set; }
            public string? DisplayName { get; set; }

            public EnumValue(string name, string? displayName = null)
            {
                Name = name;
                DisplayName = displayName;
            }
            public override string ToString() => Name;
        }
    }

}
