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
        public bool MakeAsObject => ContainerJsonType is JtContainerType.Block;

        public override JtContainerType ContainerDisplayType => JtContainerType.Array;

        public JtArray(JTemplate template, IIdentifiersManager identifiersManager) : base(template, identifiersManager)
        {
            Prefabs = new JtNodeCollection(this);
        }

        internal JtArray(JObject obj, JTemplate template, IIdentifiersManager identifiersManager) : base(obj, template, identifiersManager)
        {
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
            sb.Append(',');

            sb.Append("\"prefabs\": ");
            Prefabs.BuildJson(sb);
            sb.Append('}');
        }
    }
}