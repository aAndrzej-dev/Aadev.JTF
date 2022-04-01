using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public class JtArray : JtToken, IJtParentType
    {
        public override JTokenType JsonType => MakeAsObject ? JTokenType.Object : JTokenType.Array;
        public override JtTokenType Type => JtTokenType.Array;

        [Browsable(false)]
        public TokensCollection Prefabs { get; }
        [DefaultValue(false)] public bool MakeAsObject { get; set; }

        TokensCollection IJtParentType.Children => Prefabs;

        public JtArray(JTemplate template) : base(template)
        {
            Prefabs = new TokensCollection(this);
        }

        public JtArray(JObject obj, JTemplate template) : base(obj, template)
        {
            Prefabs = new TokensCollection(this);

            MakeAsObject = (bool)(obj["makeObject"] ?? false);


            if (obj["prefab"] is JArray arr)
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
                sb.Append("\"if\": [");

                for (int i = 0; i < Conditions.Count; i++)
                {
                    if (i != 0)
                        sb.Append(',');

                    sb.Append(Conditions[i].GetString());
                }

                sb.Append("],");
            }

            sb.Append($"\"id\": \"{Id}\"");
            sb.Append($"\"type\": \"{Type.Name}\"");
            sb.Append('}');
        }




    }
}
