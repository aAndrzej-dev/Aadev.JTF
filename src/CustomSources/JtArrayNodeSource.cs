using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtArrayNodeSource : JtContainerNodeSource
    {
        public JtNodeCollectionSource Prefabs { get; }

        internal JtArrayNodeSource(ICustomSourceParent parent, JObject source, ICustomSourceProvider sourceProvider) : base(parent, source, sourceProvider)
        {
            Prefabs = JtNodeCollectionSource.Create(this, source["prefabs"]!, sourceProvider);
            SingleType = (bool?)source["singleType"] ?? false;
            MaxSize = (int?)source["maxSize"] ?? -1;
        }


        internal JtArrayNodeSource(JtArray node, ICustomSourceProvider sourceProvider) : base(node, sourceProvider)
        {
            Prefabs = node.Prefabs.CreateSource(sourceProvider);
            MaxSize = node.MaxSize;
            SingleType = node.SingleType;
        }

        internal JtArrayNodeSource(ICustomSourceParent parent, JtArrayNodeSource @base, JObject? @override) : base(parent, @base, @override)
        {
            SingleType = (bool)(@override?["singleType"] ?? @base.SingleType);
            MaxSize = (int)(@override?["maxSize"] ?? @base.MaxSize);
            Prefabs = @base.Prefabs.CreateOverride(this, (JArray?)@override?["prefabs"]);
        }

        public override JtNodeType Type => JtNodeType.Array;

        public bool SingleType { get; set; }
        public int MaxSize { get; set; }

        public override JtContainerType ContainerDisplayType => JtContainerType.Array;

        public override JtNodeCollectionSource Children => Prefabs;
       public override JtNode CreateInstance(JToken? @override, IJtNodeParent parent) => new JtArray(this, @override, parent);
        internal override void BuildJsonDeclaration(StringBuilder sb)
        {
            BuildCommonJson(sb);
            sb.Append(", \"prefabs\": ");
            Children.BuildJson(sb);
            sb.Append('}');
        }
        internal override JtNodeSource CreateOverride(ICustomSourceParent parent, JObject? item) => new JtArrayNodeSource(parent, this, item);
    }
}
