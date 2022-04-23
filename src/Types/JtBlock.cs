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
                    children.ReadOnly = true;
                    children.AddRange((JtToken[])(Template.GetCustomValue(CustomValueId!))!.Value);

                }

                return children;
            }
        }

        public string? CustomValueId
        {
            get => customValueId; set
            {

                if (customValueId == value) return;
                customValueId = value;
                children ??= new TokensCollection(this);

                if (string.IsNullOrWhiteSpace(value))
                {
                    customValueId = null;
                }
                if (customValueId is null)
                {
                    children.ReadOnly = false;
                    return;
                }
                children.ReadOnly = true;
                children.Clear();
                children.AddRange((JtToken[])(Template.GetCustomValue(CustomValueId!))!.Value);
            }
        }
        public override bool HasExternalSources => !(CustomValueId is null);
        public JtBlock(JTemplate template) : base(template)
        {
            children = new TokensCollection(this);
        }
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
                    throw new System.Exception("Custom values name must starts with '@'");

                customValueId = str.AsSpan(1).ToString();



            }


        }

        internal override void BulidJson(StringBuilder sb)
        {
            BuildCommonJson(sb);
            sb.Append(',');

            if (customValueId is null)
            {
                sb.Append("\"children\": [");

                for (int i = 0; i < Children.Count; i++)
                {
                    if (i != 0)
                        sb.Append(',');

                    Children[i].BulidJson(sb);
                }

                sb.Append(']');
            }
            else
            {
                sb.Append($"\"children\": \"@{customValueId}\"");
            }
            sb.Append('}');
        }
        /// <inheritdoc/>
        public override JToken CreateDefaultValue() => new JObject();
    }
}
