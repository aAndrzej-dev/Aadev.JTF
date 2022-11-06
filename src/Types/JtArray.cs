using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtArray : JtContainer
    {
        private int? maxSize;
        private bool? singleType;

        public override JtNodeType Type => JtNodeType.Array;

        public new JtArrayNodeSource? Base => (JtArrayNodeSource?)base.Base;

        [Browsable(false)] public JtNodeCollection Prefabs { get; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)] public override JtNodeCollection Children => Prefabs;
        [Browsable(false)] public bool MakeAsObject => ContainerJsonType is JtContainerType.Block;

        [DefaultValue(-1)] public int MaxSize { get => maxSize ?? Base?.MaxSize ?? -1; set => maxSize = value; }
        [DefaultValue(false)] public bool SingleType { get => singleType ?? Base?.SingleType ?? false; set => singleType = value; }

        public override JtContainerType ContainerDisplayType => JtContainerType.Array;

        public JtArray(IJtNodeParent parent) : base(parent)
        {
            MaxSize = -1;
            Prefabs = JtNodeCollection.Create(this);
        }

        internal JtArray(JObject obj, IJtNodeParent parent) : base(obj, parent)
        {
            SingleType = (bool?)obj["singleType"] ?? false;
            MaxSize = (int?)obj["maxSize"] ?? -1;


            Prefabs = JtNodeCollection.Create(this, obj["prefabs"], this);
            if (ContainerDisplayType == ContainerJsonType)
                ContainerJsonType = (bool)(obj["makeObject"] ?? false) ? JtContainerType.Block : JtContainerType.Array;
        }

        internal JtArray(JtArrayNodeSource source, JToken? @override, IJtNodeParent parent) : base(source, @override, parent)
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
            if (Base != null)
            {
                if (Children.IsOverriden())
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

        public override JtNodeSource CreateSource() => currentSource ??= new JtArrayNodeSource(this, this);
    }
}