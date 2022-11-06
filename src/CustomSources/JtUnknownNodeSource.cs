using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtUnknownNodeSource : JtNodeSource
    {
        internal JtUnknownNodeSource(JtNode node, ICustomSourceProvider sourceProvider) : base(node, sourceProvider)
        {
        }

        internal JtUnknownNodeSource(ICustomSourceParent parent, JtUnknownNodeSource @base, JObject? @override) : base(parent, @base, @override)
        {
        }

        internal JtUnknownNodeSource(ICustomSourceParent parent, JObject? source, ICustomSourceProvider sourceProvider) : base(parent, source, sourceProvider)
        {
        }

        public override JtNodeType Type => JtNodeType.Unknown;
        public override JtNode CreateInstance(JToken? @override, IJtNodeParent parent) => new JtUnknown(this, @override, parent);
        internal override void BuildJsonDeclaration(StringBuilder sb)
        {
            BuildCommonJson(sb);
            sb.Append('}');

        }

        internal override JtNodeSource CreateOverride(ICustomSourceParent parent, JObject? item) => new JtUnknownNodeSource(parent, this, item);
    }
}