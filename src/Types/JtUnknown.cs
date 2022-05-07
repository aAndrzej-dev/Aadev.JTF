using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtUnknown : JtNode
    {
        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.None;
        /// <inheritdoc/>
        public override JtNodeType Type => JtNodeType.Unknown;


        public JtUnknown(JTemplate template) : base(template) { }
        internal JtUnknown(JObject obj, JTemplate template) : base(obj, template) { }
        internal override void BulidJson(StringBuilder sb)
        {
            BuildCommonJson(sb);
            sb.Append('}');
        }

        /// <inheritdoc/>
        public override JToken CreateDefaultValue() => JValue.CreateUndefined();
    }
}
