using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtArray : JtToken, IJtParentType
    {
        private int fixedSize;
        private bool isFixedSize;
        private int defaultPrefabIndex;
        private string? customValueId;

        /// <inheritdoc/>
        public override JTokenType JsonType => MakeAsObject ? JTokenType.Object : JTokenType.Array;
        /// <inheritdoc/>
        public override JtTokenType Type => JtTokenType.Array;

        [Browsable(false)]
        public TokensCollection Prefabs { get; }
        [DefaultValue(false)] public bool MakeAsObject { get; set; }

        [DefaultValue(false)] public bool IsFixedSize { get => isFixedSize; set { isFixedSize = value; fixedSize = isFixedSize ? 0 : -1; } }
        [DefaultValue(-1)] public int FixedSize { get => fixedSize; set { fixedSize = value; isFixedSize = fixedSize >= 0; if (fixedSize < 0) fixedSize = -1; } }
        [DefaultValue(0)] public int DefaultPrefabIndex { get => defaultPrefabIndex; set { if (Prefabs.Count <= value) return; defaultPrefabIndex = value; } }


        TokensCollection IJtParentType.Children => Prefabs;
        public string? CustomValueId { get => customValueId; set { if (customValueId == value) return; customValueId = value; Prefabs.Clear(); Prefabs.AddRange((JtToken[])(Template.GetCustomValue(CustomValueId!))!.Value); } }
        public JtArray(JTemplate template) : base(template)
        {
            Prefabs = new TokensCollection(this);
        }

        internal JtArray(JObject obj, JTemplate template) : base(obj, template)
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
            else if (((JValue?)obj["prefabs"])?.Value is string str)
            {
                if (!str.StartsWith("@"))
                    throw new System.Exception();

                customValueId = str.AsSpan(1).ToString();

                Prefabs.AddRange((JtToken[])(Template.GetCustomValue(CustomValueId!))!.Value);

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

            if (customValueId is null)
            {
                sb.Append("\"prefabs\": [");

                for (int i = 0; i < Prefabs.Count; i++)
                {
                    if (i != 0)
                        sb.Append(',');

                    Prefabs[i].BulidJson(sb);
                }

                sb.Append("],");
            }
            else
            {
                sb.Append($"\"prefabs\": \"{customValueId}\"");
            }


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
            sb.Append($"\"type\": \"{Type.Name}\"");
            sb.Append('}');
        }
        /// <inheritdoc/>
        public override JToken CreateDefaultToken()
        {
            if (MakeAsObject)
                return new JObject();
            return new JArray();
        }
    }
}
