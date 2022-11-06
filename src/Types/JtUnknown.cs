using Aadev.JTF.CustomSources;
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


        public JtUnknown(IJtNodeParent parent) : base(parent) { }
        internal JtUnknown(JObject obj, IJtNodeParent parent) : base(obj, parent) { }

        internal JtUnknown(JtUnknownNodeSource source, JToken? @override, IJtNodeParent parent) : base(source, @override, parent) { }

        internal override void BuildJson(StringBuilder sb)
        {
            BuildCommonJson(sb);
            sb.Append('}');
        }

        /// <inheritdoc/>
        public override JToken CreateDefaultValue() => JValue.CreateUndefined();
        public override JtNodeSource CreateSource() => currentSource ??= new JtUnknownNodeSource(this, this);
    }
}