using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtBool : JtNode
    {
        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.Boolean;
        /// <inheritdoc/>
        public override JtNodeType Type => JtNodeType.Bool;

        [DefaultValue(false)] public bool Default { get; set; }


        public JtBool(JTemplate template, IIdentifiersManager identifiersManager) : base(template, identifiersManager)
        {
        }
        internal JtBool(JObject obj, JTemplate template, IIdentifiersManager identifiersManager) : base(obj, template, identifiersManager)
        {
            Default = (bool?)obj["default"] ?? false;

        }

        internal override void BulidJson(StringBuilder sb)
        {
            BuildCommonJson(sb);

            if (Default)
                sb.Append($", \"default\": true");
            sb.Append('}');
        }

        /// <inheritdoc/>
        public override JToken CreateDefaultValue() => new JValue(Default);

    }
}