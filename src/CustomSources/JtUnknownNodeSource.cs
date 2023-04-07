using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtUnknownNodeSource : JtNodeSource
    {
        public override JtNodeType Type => JtNodeType.Unknown;

        public override JTokenType JsonType => JTokenType.Undefined;

        public JtUnknownNodeSource(IJtNodeSourceParent parent) : base(parent) { }
        internal JtUnknownNodeSource(JtUnknownNode node) : base(node)
        {
#if DEBUG
            throw new JtfException();
#endif
        }
        internal JtUnknownNodeSource(IJtNodeSourceParent parent, JObject? source) : base(parent, source)
        {
#if DEBUG
            throw new JtfException();
#endif
        }
        internal JtUnknownNodeSource(IJtNodeSourceParent parent, JtUnknownNodeSource @base, JObject? @override) : base(parent, @base, @override)
        {
#if DEBUG
            throw new JtfException();
#endif
        }

        internal override void BuildJsonDeclaration(StringBuilder sb)
        {
            BuildCommonJson(sb);
            sb.Append('}');

        }
        public override JtNode CreateInstance(IJtNodeParent parent, JToken? @override) => new JtUnknownNode(parent, this, @override);
        public override JtNodeSource CreateOverride(IJtNodeSourceParent parent, JObject? @override) => new JtUnknownNodeSource(parent, this, @override);
        public override JToken CreateDefaultValue() => JValue.CreateUndefined();
    }
}