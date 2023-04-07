using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtUnknownNode : JtNode
    {
        public override JTokenType JsonType => JTokenType.Undefined;
        public override JtNodeType Type => JtNodeType.Unknown;

        public JtUnknownNode(IJtNodeParent parent) : base(parent)
        {

        }
        internal JtUnknownNode(IJtNodeParent parent, JObject source) : base(parent, source)
        {

        }

        internal JtUnknownNode(IJtNodeParent parent, JtUnknownNodeSource source, JToken? @override) : base(parent, source, @override)
        {

        }

        internal override void BuildJson(StringBuilder sb)
        {
            BuildCommonJson(sb);
            sb.Append('}');
        }
        public override JToken CreateDefaultValue() => JValue.CreateUndefined();
        public override JtNodeSource CreateSource() => currentSource ??= new JtUnknownNodeSource(this);
    }
}