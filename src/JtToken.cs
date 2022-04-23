using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Aadev.JTF
{
    public abstract class JtToken
    {
        private JtConditionCollection? conditions;
        private IJtParentType? parent;
        private JTemplate template;
        private string? name;
        private string? displayName;

        /// <summary>
        /// Type of json node
        /// </summary>
        [Browsable(false)] public abstract JTokenType JsonType { get; }
        /// <summary>
        /// Type of <see cref="JtToken"/>
        /// </summary>
        [Browsable(false)] public abstract JtTokenType Type { get; }
        /// <summary>
        /// Name of current token used as key in json file; <see langword="null"/> when <see cref="IsArrayPrefab"/> is <see langword="true"/>
        /// </summary>
        [Category("General"), Description("Name of current token used as key in json file")]
        public string? Name { get => name; set { if (IsArrayPrefab || IsRoot) return; if (DisplayName == name) DisplayName = value; name = value; } }
        /// <summary>
        /// Description of current <see cref="JtToken"/>
        /// </summary>
        [Category("General")] public string? Description { get; set; }
        /// <summary>
        /// Specify whether create json node event value is <see langword="null"/>
        /// </summary>
        [DefaultValue(false), Category("General")] public bool Required { get; set; }
        /// <summary>
        /// Name displayed in editor
        /// </summary>
        [Category("General")] public string? DisplayName { get => displayName; set { if (IsArrayPrefab || IsRoot) return; displayName = value; } }
        /// <summary>
        /// Parent element
        /// </summary>
        [Browsable(false)] public IJtParentType? Parent { get => parent; set { parent = value; if (parent is null) return; template = parent.Template; } }
        /// <summary>
        /// Unique id; used in conditions
        /// </summary>
        [Category("General")] public string? Id { get; set; }

        /// <summary>
        /// Root template
        /// </summary>
        [Browsable(false)] public JTemplate Template => template;
        /// <summary>
        /// <see langword="true"/> if parent is <see cref="JtArray"/>
        /// </summary>
#if NET5_0_OR_GREATER
        [MemberNotNullWhen(true, nameof(Parent))]
#endif
        [Browsable(false)] public bool IsArrayPrefab => Parent is JtArray;
        /// <summary>
        /// <see langword="true"/> if one of parents is <see cref="JtArray"/>
        /// </summary>
#if NET5_0_OR_GREATER
        [MemberNotNullWhen(true, nameof(Parent))]
#endif
        [Browsable(false)] public bool IsInArrayPrefab => IsArrayPrefab || Parent?.IsInArrayPrefab is true;
        /// <summary>
        /// <see langword="true"/> if parent is <see cref="JtArray"/> and MakeAsObject is true
        /// </summary>
#if NET5_0_OR_GREATER
        [MemberNotNullWhen(true, nameof(Parent))]
#endif
        [Browsable(false)] public bool IsDynamicName => Parent is JtArray array && array.MakeAsObject;

        [Browsable(false)] public bool IsRoot => Template.Root == this;

        /// <summary>
        /// Conditions of current element
        /// </summary>
        [Category("General")] public JtConditionCollection Conditions => conditions ??= new JtConditionCollection();

        [Browsable(false)] public virtual bool HasExternalSources { get; }

        /// <summary>
        /// Create empty instace of current element
        /// </summary>
        /// <param name="template">Root template</param>
        protected internal JtToken(JTemplate template)
        {
            this.template = template;
        }




        /// <summary>
        /// Create new instance with properties loaded form obj
        /// </summary>
        /// <param name="template">Root template</param>
        /// <param name="obj">Object to load proerties from</param>
        protected internal JtToken(JObject obj, JTemplate template)
        {
            this.template = template;
            Name = (string?)obj["name"];
            Description = (string?)obj["description"];
            Required = (bool)(obj["required"] ?? false);
            DisplayName = (string?)(obj["displayName"] ?? Name);
            Id = (string?)obj["id"];

            if (obj["if"] is JArray conditions)
            {
                foreach (JObject item in conditions)
                {
                    Conditions.Add(new JtCondition(item));
                }
            }
            else if (obj["conditions"] is JArray conditions2)
            {
                foreach (JObject item in conditions2)
                {
                    Conditions.Add(new JtCondition(item));
                }
            }
        }

        /// <summary>
        /// Get all elements witch has this same name ant diffrent type in this same parent
        /// </summary>
        /// <returns></returns>
        public JtToken[] GetTwinFamily() => Parent is null ? (new JtToken[] { this }) : Parent.Children.Where(x => x.Name == Name && x.Conditions == Conditions).ToArray();

        internal abstract void BulidJson(StringBuilder sb);
        protected internal void BuildCommonJson(StringBuilder sb)
        {
            sb.Append('{');
            if (!IsArrayPrefab && !IsRoot)
                sb.Append($"\"name\": \"{Name}\",");
            sb.Append($"\"type\": \"{Type.Name}\"");
            if (!string.IsNullOrWhiteSpace(Description))
                sb.Append($", \"description\": \"{Description}\"");
            if (DisplayName != Name)
                sb.Append($", \"displayName\": \"{DisplayName}\"");
            if (!(Id is null))
                sb.Append($", \"id\": \"{Id}\"");

            if (Conditions.Count > 0)
            {
                sb.Append(", \"conditions\": [");

                for (int i = 0; i < Conditions.Count; i++)
                {
                    if (i != 0)
                        sb.Append(',');

                    Conditions[i].BulidJson(sb);
                }

                sb.Append(']');
            }
        }

        /// <summary>
        /// Convert current tag to json 
        /// </summary>
        /// <returns></returns>
        public string GetJson()
        {
            StringBuilder sb = new StringBuilder();
            BulidJson(sb);
            return sb.ToString();
        }

        /// <summary>
        /// Create new instace of <see cref="JtToken"/>
        /// </summary>
        /// <param name="item">Object to load <see cref="JtToken"/> from</param>
        /// <param name="template">Root template</param>
        /// <returns>New instace of JtToken</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public static JtToken Create(JObject item, JTemplate template)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            if (template is null) throw new ArgumentNullException(nameof(template));


            if (((JValue?)item["type"])?.Value is int typeId)
            {
                return JtTokenType.GetById(typeId).CreateInstance(item, template);
            }

            string typeString = (string?)item["type"] ?? throw new Exception($"Item '{item["name"]}' dont have type");

            return JtTokenType.GetByName(typeString).CreateInstance(item, template);

        }

        /// <summary>
        /// Creates <see cref="JToken"/> with default value of <see cref="JtToken"/>
        /// </summary>
        /// <returns>New <see cref="JToken"/> with default value </returns>
        public abstract JToken CreateDefaultValue();
    }
}
