using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtArrayNodeSource : JtContainerNodeSource
    {
        public override JtContainerType ContainerDisplayType => JtContainerType.Array;
        public override JtNodeCollectionSource Children => Prefabs;
        public override JtNodeType Type => JtNodeType.Array;


        public bool SingleType { get; set; }
        public int MaxSize { get; set; }
        public JtNodeCollectionSource Prefabs { get; }


        public JtArrayNodeSource(ICustomSourceParent parent) : base(parent)
        {
            Prefabs = JtNodeCollectionSource.Create(this);
            MaxSize = -1;
        }
        internal JtArrayNodeSource(JtArray node) : base(node)
        {
            Prefabs = node.Prefabs.CreateSource();
            MaxSize = node.MaxSize;
            SingleType = node.SingleType;
        }
        internal JtArrayNodeSource(ICustomSourceParent parent, JObject source) : base(parent, source)
        {
            Prefabs = JtNodeCollectionSource.Create(this, source["prefabs"]);
            SingleType = (bool?)source["singleType"] ?? false;
            MaxSize = (int?)source["maxSize"] ?? -1;
        }
        internal JtArrayNodeSource(ICustomSourceParent parent, JtArrayNodeSource @base, JObject? @override) : base(parent, @base, @override)
        {
            SingleType = (bool)(@override?["singleType"] ?? @base.SingleType);
            MaxSize = (int)(@override?["maxSize"] ?? @base.MaxSize);
            Prefabs = @base.Prefabs.CreateOverride(this, (JArray?)@override?["prefabs"]);
        }




        internal override void BuildJsonDeclaration(StringBuilder sb)
        {
            BuildCommonJson(sb);
            sb.Append(", \"prefabs\": ");
            Children.BuildJson(sb);
            sb.Append('}');
        }
        public override JtNode CreateInstance(IJtNodeParent parent, JToken? @override) => new JtArray(parent, this, @override);
        public override JtNodeSource CreateOverride(ICustomSourceParent parent, JObject? @override) => new JtArrayNodeSource(parent, this, @override);
    }
}
