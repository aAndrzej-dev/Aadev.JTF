using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtArray : JtNode, IJtParentNode
    {
        private int fixedSize;
        private bool isFixedSize;
        private int defaultPrefabIndex;

        /// <inheritdoc/>
        public override JTokenType JsonType => MakeAsObject ? JTokenType.Object : JTokenType.Array;
        /// <inheritdoc/>
        public override JtNodeType Type => JtNodeType.Array;

        [Browsable(false)] public JtNodeCollection Prefabs { get; }


        [DefaultValue(false)] public bool MakeAsObject { get; set; }
        [DefaultValue(-1)] public int FixedSize { get => fixedSize; set { fixedSize = value; isFixedSize = fixedSize >= 0; if (fixedSize < 0) fixedSize = -1; } }
        [DefaultValue(0)] public int DefaultPrefabIndex { get => defaultPrefabIndex; set { if (Prefabs.Count <= value) return; defaultPrefabIndex = value; } }

        [DefaultValue(false)] public bool IsFixedSize { get => isFixedSize; set { isFixedSize = value; FixedSize = isFixedSize ? 0 : -1; } }

        JtNodeCollection IJtParentNode.Children => Prefabs;

        public override bool HasExternalSources => !(Prefabs.CustomSourceId is null);



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


            MakeAsObject = (bool)(obj["makeObject"] ?? false);
            FixedSize = (int)(obj["fixedSize"] ?? -1);
            DefaultPrefabIndex = (int)(obj["defaultPrefabIndex"] ?? 0);
        }

        internal override void BulidJson(StringBuilder sb)
        {
            BuildCommonJson(sb);
            sb.Append(',');


            if (MakeAsObject)
                sb.Append($"\"makeObject\": true,");
            if (DefaultPrefabIndex != 0)
                sb.Append($"\"defaultPrefabIndex\": {DefaultPrefabIndex},");
            sb.Append("\"prefabs\": ");
            Prefabs.BuildJson(sb);
            sb.Append('}');
        }
        /// <inheritdoc/>
        public override JToken CreateDefaultValue()
        {
            if (MakeAsObject)
                return new JObject();
            return new JArray();
        }
    }
}