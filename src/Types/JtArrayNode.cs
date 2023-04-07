using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtArrayNode : JtContainerNode
    {
        private int? maxSize;
        private bool? singleType;

        public new JtArrayNodeSource? Base => (JtArrayNodeSource?)base.Base;
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)] public override JtNodeCollection Children => Prefabs;
        public override JtContainerType ContainerDisplayType => JtContainerType.Array;
        public override JtNodeType Type => JtNodeType.Array;

        [Browsable(false)] public JtNodeCollection Prefabs { get; }
        [DefaultValue(-1)] public int MaxSize { get => maxSize ?? Base?.MaxSize ?? -1; set => maxSize = value; }
        [DefaultValue(false)] public bool SingleType { get => singleType ?? Base?.SingleType ?? false; set => singleType = value; }


        [Browsable(false)] public bool MakeAsObject => ContainerJsonType is JtContainerType.Block;



        public JtArrayNode(IJtNodeParent parent) : base(parent)
        {
            MaxSize = -1;
            Prefabs = JtNodeCollection.Create(this);
        }
        internal JtArrayNode(IJtNodeParent parent, JObject source) : base(parent, source)
        {
            SingleType = (bool?)source["singleType"] ?? false;
            MaxSize = (int?)source["maxSize"] ?? -1;


            Prefabs = JtNodeCollection.Create(this, source["prefabs"]);
            if (ContainerDisplayType == ContainerJsonType)
                ContainerJsonType = (bool)(source["makeObject"] ?? false) ? JtContainerType.Block : JtContainerType.Array;
        }
        internal JtArrayNode(IJtNodeParent parent, JtArrayNodeSource source, JToken? @override) : base(parent, source, @override)
        {
            Prefabs = source.Prefabs.CreateInstance(this, @override?["prefabs"]);
            if (@override is null)
                return;
            singleType = (bool?)@override["singleType"];
            maxSize = (int?)@override["maxSize"];
        }

        internal override void BuildJson(StringBuilder sb)
        {
            BuildCommonJson(sb);

            if (Base is not null)
            {
                if (Children.IsOverridden())
                {
                    sb.Append(", \"prefabs\": ");
                    Prefabs.BuildJson(sb);
                }
                if (MaxSize != Base.MaxSize)
                    sb.Append($", \"maxSize\": {MaxSize}");
                if (SingleType != Base.SingleType)
                    sb.Append($", \"singleType\": {SingleType}");
                sb.Append('}');
                return;
            }



            if (MaxSize >= 0)
                sb.Append($", \"maxSize\": {MaxSize}");
            if (SingleType)
                sb.Append($", \"singleType\": true");

            sb.Append(", \"prefabs\": ");
            Prefabs.BuildJson(sb);

            sb.Append('}');
        }
        public override JtNodeSource CreateSource() => currentSource ??= new JtArrayNodeSource(this);
    }
}