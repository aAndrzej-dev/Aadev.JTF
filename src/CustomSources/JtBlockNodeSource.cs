using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtBlockNodeSource : JtContainerNodeSource
    {
        public override JtNodeCollectionSource Children { get; }

        internal JtBlockNodeSource(ICustomSourceParent parent, JObject source, ICustomSourceProvider sourceProvider) : base(parent, source, sourceProvider)
        {
            Children = JtNodeCollectionSource.Create(this, source["children"]!, sourceProvider);
        }

        internal JtBlockNodeSource(JtBlock node, ICustomSourceProvider sourceProvider) : base(node, sourceProvider)
        {
            Children = node.Children.CreateSource(sourceProvider);
        }

        internal JtBlockNodeSource(ICustomSourceParent parent, JtBlockNodeSource @base, JObject? @override) : base(parent, @base, @override)
        {
            Children = @base.Children.CreateOverride(this, (JArray?)@override?["children"]);
        }

        public override JtNodeType Type => JtNodeType.Block;
        public override JtContainerType ContainerDisplayType => JtContainerType.Block;
        public override JtNode CreateInstance(JToken? @override, IJtNodeParent parent) => new JtBlock(this, @override, parent);
        internal override void BuildJsonDeclaration(StringBuilder sb)
        {
            BuildCommonJson(sb);
            sb.Append(", \"children\": ");
            Children.BuildJson(sb);
            sb.Append('}');
        }
        internal override JtNodeSource CreateOverride(ICustomSourceParent parent, JObject? item) => new JtBlockNodeSource(parent, this, item);
    }
}