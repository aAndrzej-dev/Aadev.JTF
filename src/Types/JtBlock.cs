using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtBlock : JtToken, IJtParentType
    {
        private TokensCollection? children;
        private string? customValueId;

        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.Object;
        /// <inheritdoc/>
        public override JtTokenType Type => JtTokenType.Block;
        [Browsable(false)]
        public TokensCollection Children
        {
            get
            {
                if (children is null)
                {
                    children = new TokensCollection(this);
                    children.AddRange((JtToken[])(Template.GetCustomValue(CustomValueId!))!.Value);

                }

                return children;
            }
        }

        public string? CustomValueId { get => customValueId; set { if (customValueId == value) return; customValueId = value; children ??= new TokensCollection(this); children.Clear(); children.AddRange((JtToken[])(Template.GetCustomValue(CustomValueId!))!.Value); } }

        public JtBlock(JTemplate template) : base(template) { }
        internal JtBlock(JObject obj, JTemplate template) : base(obj, template)
        {
            if (obj["children"] is JArray arr)
            {
                children = new TokensCollection(this);
                foreach (JObject item in arr)
                {
                    children.Add(Create(item, Template));
                }
            }
            else if (((JValue?)obj["children"])?.Value is string str)
            {
                if (!str.StartsWith("@"))
                    throw new System.Exception();

                customValueId = str.AsSpan(1).ToString();



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

            if (customValueId is null)
            {
                sb.Append("\"children\": [");

                for (int i = 0; i < Children.Count; i++)
                {
                    if (i != 0)
                        sb.Append(',');

                    Children[i].BulidJson(sb);
                }

                sb.Append("],");
            }
            else
            {
                sb.Append($"\"children\": \"{customValueId}\"");
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
        public override JToken CreateDefaultToken() => new JObject();
    }
}
