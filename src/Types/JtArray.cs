using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public class JtArray : JtToken, IJtParentType
    {
        private int fixedSize;
        private bool isFixedSize;
        private int defaultPrefabIndex;

        public override JTokenType JsonType => MakeAsObject ? JTokenType.Object : JTokenType.Array;
        public override JtTokenType Type => JtTokenType.Array;

        [Browsable(false)]
        public TokensCollection Prefabs { get; }
        [DefaultValue(false)] public bool MakeAsObject { get; set; }

        [DefaultValue(false)] public bool IsFixedSize { get => isFixedSize; set { isFixedSize = value; fixedSize = isFixedSize ? 0 : -1; } }
        [DefaultValue(-1)] public int FixedSize { get => fixedSize; set { fixedSize = value; isFixedSize = fixedSize >= 0; if (fixedSize < 0) fixedSize = -1; } }
        [DefaultValue(0)] public int DefaultPrefabIndex { get => defaultPrefabIndex; set { if (Prefabs.Count <= value) return; defaultPrefabIndex = value; } }


        TokensCollection IJtParentType.Children => Prefabs;

        public JtArray(JTemplate template) : base(template)
        {
            Prefabs = new TokensCollection(this);
        }

        public JtArray(JObject obj, JTemplate template) : base(obj, template)
        {
            Prefabs = new TokensCollection(this);

            MakeAsObject = (bool)(obj["makeObject"] ?? false);
            FixedSize = (int)(obj["fixedSize"] ?? -1);

            if (obj["prefabs"] is JArray arr)
            {
                foreach (JObject? item in arr)
                {
                    if (item is null)
                        continue;
                    Prefabs.Add(Create(item, template));
                }
            }
            else if (obj["prefab"] is JObject pref)
            {
                Prefabs.Add(Create(pref, template));
            }
        }

        internal override void BulidJson(StringBuilder sb)
        {
            sb.Append('{');
            if (!IsArrayPrefab)
                sb.Append($"\"name\": \"{Name}\",");
            if (!string.IsNullOrWhiteSpace(Description))
                sb.Append($"\"description\": \"{Description}\",");
            if (DisplayName != Name)
                sb.Append($"\"displayName\": \"{DisplayName}\",");

            if (MakeAsObject)
                sb.Append($"\"makeObject\": true,");


            sb.Append("\"prefabs\": [");

            for (int i = 0; i < Prefabs.Count; i++)
            {
                if (i != 0)
                    sb.Append(',');

                Prefabs[i].BulidJson(sb);
            }

            sb.Append("],");

            if (Conditions.Count > 0)
            {
                sb.Append("\"conditions\": [");

                for (int i = 0; i < Conditions.Count; i++)
                {
                    if (i != 0)
                        sb.Append(',');

                    sb.Append(Conditions[i].GetString());
                }

                sb.Append("],");
            }

            sb.Append($"\"id\": \"{Id}\",");
            if (IsUsingCustomType)
                sb.Append($"\"type\": \"{CustomType}\"");
            else
                sb.Append($"\"type\": \"{Type.Name}\"");
            sb.Append('}');
        }




    }
}
