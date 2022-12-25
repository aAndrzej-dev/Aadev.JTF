using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtBlock : JtContainer
    {
        public override JtNodeCollection Children { get; }
        public override JtContainerType ContainerDisplayType => JtContainerType.Block;
        public override JtNodeType Type => JtNodeType.Block;


        public JtBlock(IJtNodeParent parent) : base(parent)
        {
            Children = JtNodeCollection.Create(this);
        }
        internal JtBlock(IJtNodeParent parent, JObject source) : base(parent, source)
        {
            Children = JtNodeCollection.Create(this, source["children"]);
        }
        internal JtBlock(IJtNodeParent parent, JtBlockNodeSource source, JToken? @override) : base(parent, source, @override)
        {
            Children = source.Children.CreateInstance(this, (JArray?)@override?["children"]);
        }

        internal override void BuildJson(StringBuilder sb)
        {
            BuildCommonJson(sb);
            if (Base != null)
            {
                if (Children.IsOverriden())
                {
                    sb.Append(", \"children\": ");
                    Children.BuildJson(sb);
                }
                sb.Append('}');
                return;
            }

            sb.Append(", \"children\": ");
            Children.BuildJson(sb);
            sb.Append('}');
        }
        public override JtNodeSource CreateSource() => currentSource ??= new JtBlockNodeSource(this);
    }
}