using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtBoolNodeSource : JtNodeSource
    {
        public override JtNodeType Type => JtNodeType.Bool;

        public bool Default { get; set; }
        public bool Constant { get; set; }

        public override JTokenType JsonType => JTokenType.Boolean;

        public JtBoolNodeSource(IJtNodeSourceParent parent) : base(parent) { }
        internal JtBoolNodeSource(JtBoolNode node) : base(node)
        {
            Default = node.Default;
            Constant = node.Constant;
        }
        internal JtBoolNodeSource(IJtNodeSourceParent parent, JObject source) : base(parent, source)
        {
            Default = (bool?)source["default"] ?? false;
            Constant = (bool?)source["constant"] ?? false;
        }
        internal JtBoolNodeSource(IJtNodeSourceParent parent, JtBoolNodeSource @base, JObject? @override) : base(parent, @base, @override)
        {
            Default = (bool)(@override?["default"] ?? @base.Default);
            Constant = (bool)(@override?["constant"] ?? @base.Constant);
        }





        internal override void BuildJsonDeclaration(StringBuilder sb)
        {
            BuildCommonJson(sb);
            if (Default)
                sb.Append(", \"default\": true");
            if (Constant)
                sb.Append(", \"constant\": true");
            sb.Append('}');
        }
        public override JtNode CreateInstance(IJtNodeParent parent, JToken? @override) => new JtBoolNode(parent, this, @override);
        public override JtNodeSource CreateOverride(IJtNodeSourceParent parent, JObject? @override) => new JtBoolNodeSource(parent, this, @override);
        public override JToken CreateDefaultValue() => new JValue(Default);
    }
}