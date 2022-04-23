using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtBool : JtToken
    {
        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.Boolean;
        /// <inheritdoc/>
        public override JtTokenType Type => JtTokenType.Bool;

        [DefaultValue(false)] public bool Default { get; set; }

        public JtBool(JTemplate template) : base(template)
        {
            Default = false;
        }
        internal JtBool(JObject obj, JTemplate template) : base(obj, template)
        {
            Default = (bool)(obj["default"] ?? false);
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
