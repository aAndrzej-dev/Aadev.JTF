using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtArray : JtContainer
    {
        /// <inheritdoc/>
        public override JtNodeType Type => JtNodeType.Array;

        [Browsable(false)] public JtNodeCollection Prefabs { get; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)] public override JtNodeCollection Children => Prefabs;
        public override bool HasExternalSources => !(Prefabs.CustomSourceId is null);
        [Browsable(false)] public bool MakeAsObject => ContainerJsonType is JtContainerType.Block;

        [DefaultValue(-1)] public int MaxSize { get; set; }
        [DefaultValue(false)] public bool SingleType { get; set; }

        public override JtContainerType ContainerDisplayType => JtContainerType.Array;

        public JtArray(JTemplate template, IIdentifiersManager identifiersManager) : base(template, identifiersManager)
        {
            MaxSize = -1;
            Prefabs = new JtNodeCollection(this);
        }

        internal JtArray(JObject obj, JTemplate template, IIdentifiersManager identifiersManager) : base(obj, template, identifiersManager)
        {
            SingleType = (bool?)obj["singleType"] ?? false;
            MaxSize = (int?)obj["maxSize"] ?? -1;

            if (obj["prefabs"] != null)
            {
                Prefabs = new JtNodeCollection(this, obj["prefabs"]);
            }
            else if (obj["prefab"] is JObject pref)
            {
                Prefabs = new JtNodeCollection(this)
                {
                    Create(pref, template, new BlankIdentifiersManager())
                };
            }
            else
                Prefabs = new JtNodeCollection(this);
            if (ContainerDisplayType == ContainerJsonType)
                ContainerJsonType = (bool)(obj["makeObject"] ?? false) ? JtContainerType.Block : JtContainerType.Array;
        }

        internal override void BulidJson(StringBuilder sb)
        {
            BuildCommonJson(sb);

            if (MaxSize >= 0)
                sb.Append($", \"maxSize\": {MaxSize}");
            if (SingleType)
                sb.Append($", \"singleType\": true");

            sb.Append(", \"prefabs\": ");
            Prefabs.BuildJson(sb);
            sb.Append('}');
        }
    }
}