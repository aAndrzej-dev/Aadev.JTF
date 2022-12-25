using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtUnknownNodeSource : JtNodeSource
    {
        public override JtNodeType Type => JtNodeType.Unknown;



        public JtUnknownNodeSource(ICustomSourceParent parent) : base(parent) { }
        internal JtUnknownNodeSource(JtNode node) : base(node)
        {
#if DEBUG
            throw new JtfException();
#endif
        }
        internal JtUnknownNodeSource(ICustomSourceParent parent, JObject? source) : base(parent, source)
        {
#if DEBUG
            throw new JtfException();
#endif
        }
        internal JtUnknownNodeSource(ICustomSourceParent parent, JtUnknownNodeSource @base, JObject? @override) : base(parent, @base, @override)
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
        public override JtNode CreateInstance(IJtNodeParent parent, JToken? @override) => new JtUnknown(parent, this, @override);
        public override JtNodeSource CreateOverride(ICustomSourceParent parent, JObject? @override) => new JtUnknownNodeSource(parent, this, @override);
    }
}