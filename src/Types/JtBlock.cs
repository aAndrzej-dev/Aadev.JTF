using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtBlock : JtContainer
    {
        /// <inheritdoc/>
        public override JtNodeType Type => JtNodeType.Block;


        public override JtContainerType ContainerDisplayType => JtContainerType.Block;

        public override bool HasExternalSources => !(Children.CustomSourceId is null);

        public override JtNodeCollection Children { get; }

        public JtBlock(JTemplate template, IIdentifiersManager identifiersManager) : base(template, identifiersManager)
        {
            Children = new JtNodeCollection(this);
        }
        internal JtBlock(JObject obj, JTemplate template, IIdentifiersManager identifiersManager) : base(obj, template, identifiersManager)
        {
            Children = new JtNodeCollection(this, obj["children"]);
        }

        internal override void BulidJson(StringBuilder sb)
        {
            BuildCommonJson(sb);

            sb.Append(",\"children\": ");
            Children.BuildJson(sb);
            sb.Append('}');
        }
    }
}