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
        public string? CustomValueId
        {
            get => customValueId; set
            {

                if (customValueId == value) return;
                customValueId = value;
                if (customValueId is null)
                {
                    Prefabs.ReadOnly = false;
                    return;
                }
                Prefabs.Clear();
                Prefabs.AddRange((JtToken[])(Template.GetCustomValue(CustomValueId!))!.Value);
            }
        }

        public override bool HasExternalSources => !(CustomValueId is null);

        public JtArray(JTemplate template) : base(template)
        {
            Prefabs = new TokensCollection(this);
        }

        internal JtArray(JObject obj, JTemplate template) : base(obj, template)
        {
            Prefabs = new TokensCollection(this);

            MakeAsObject = (bool)(obj["makeObject"] ?? false);
            FixedSize = (int)(obj["fixedSize"] ?? -1);
            DefaultPrefabIndex = (int)(obj["defaultPrefabIndex"] ?? 0);

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
                    throw new System.Exception("Custom values name must starts with '@'");

                customValueId = str.AsSpan(1).ToString();
                Prefabs.ReadOnly = true;
                Prefabs.AddRange((JtToken[])(Template.GetCustomValue(CustomValueId!))!.Value);

            }
        }

        internal override void BulidJson(StringBuilder sb)
        {
            BuildCommonJson(sb);
            sb.Append(',');


            if (MakeAsObject)
                sb.Append($"\"makeObject\": true,");
            if (DefaultPrefabIndex != 0)
                sb.Append($"\"defaultPrefabIndex\": {DefaultPrefabIndex},");

            if (customValueId is null)
            {
                sb.Append("\"prefabs\": [");

                for (int i = 0; i < Prefabs.Count; i++)
                {
                    if (i != 0)
                        sb.Append(',');

                    Prefabs[i].BulidJson(sb);
                }

                sb.Append(']');
            }
            else
            {
                sb.Append($"\"prefabs\": \"@{customValueId}\"");
            }
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
