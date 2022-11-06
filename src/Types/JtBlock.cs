using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtBlock : JtContainer
    {
        public override JtNodeType Type => JtNodeType.Block;


        public override JtContainerType ContainerDisplayType => JtContainerType.Block;


        public override JtNodeCollection Children { get; }

        public JtBlock(IJtNodeParent parent) : base(parent)
        {
            Children = JtNodeCollection.Create(this);
        }
        internal JtBlock(JObject obj, IJtNodeParent parent) : base(obj, parent)
        {
            Children = JtNodeCollection.Create(this, obj["children"], this);
        }
        internal JtBlock(JtBlockNodeSource source, JToken? @override, IJtNodeParent parent) : base(source, @override, parent)
        {
            Children = source.Children.CreateInstance(this, (JArray?)@override?["children"]);
        }

        internal override void BuildJson(StringBuilder sb)
        {
            BuildCommonJson(sb);
            if(Base != null)
            {
                if(Children.IsOverriden())
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
        public override JtNodeSource CreateSource() => currentSource ??= new JtBlockNodeSource(this, this);
    }
}