using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtBoolNodeSource : JtNodeSource
    {
        internal JtBoolNodeSource(JtBool node, ICustomSourceProvider sourceProvider) : base(node, sourceProvider)
        {
            Default = node.Default;
            Constant = node.Constant;
        }

        internal JtBoolNodeSource(ICustomSourceParent parent, JtNodeSource @base, JObject? @override) : base(parent, @base, @override)
        {
        }

        internal JtBoolNodeSource(ICustomSourceParent parent, JObject source, ICustomSourceProvider sourceProvider) : base(parent, source, sourceProvider)
        {
            Default = (bool?)source["default"] ?? false;
            Constant = (bool?)source["constant"] ?? false;
        }

        public bool Default { get; internal set; }
        public bool Constant { get; internal set; }

        public override JtNodeType Type => JtNodeType.Bool;

        public override JtNode CreateInstance(JToken? @override, IJtNodeParent parent) => new JtBool(this, @override, parent);

        internal override void BuildJsonDeclaration(StringBuilder sb)
        {
            BuildCommonJson(sb);
            if (Default)
                sb.Append(", \"default\": true");
            if (Constant)
                sb.Append(", \"constant\": true");
            sb.Append('}');
        }
        internal override JtNodeSource CreateOverride(ICustomSourceParent parent, JObject? item) => new JtBoolNodeSource(parent, this, item);
    }
}