using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Aadev.JTF
{
    public abstract class JtNode
    {
        private IJtParentNode? parent;
        private JTemplate template;
        private string? name;
        private string? displayName;
        private string? description;
        private bool required;
        private string? id;

        /// <summary>
        /// Type of json node
        /// </summary>
        [Browsable(false)] public abstract JTokenType JsonType { get; }
        /// <summary>
        /// Type of <see cref="JtNode"/>
        /// </summary>
        [Browsable(false)] public abstract JtNodeType Type { get; }
        /// <summary>
        /// Name of current token used as key in json file; <see langword="null"/> when <see cref="IsArrayPrefab"/> is <see langword="true"/>
        /// </summary>
        [Category("General"), Description("Name of current token used as key in json file")]
        public string? Name { get => name; set { if (IsArrayPrefab || IsRoot || ReadOnly) return; if (DisplayName == name) DisplayName = value; name = value; } }
        /// <summary>
        /// Description of current <see cref="JtNode"/>
        /// </summary>
        [Category("General")] public string? Description { get => description; set { if (ReadOnly) return; description = value; } }
        /// <summary>
        /// Specify whether create json node event value is <see langword="null"/>
        /// </summary>
        [DefaultValue(false), Category("General")] public bool Required { get => required; set { if (ReadOnly) return; required = value; } }
        /// <summary>
        /// Name displayed in editor
        /// </summary>
        [Category("General")] public string? DisplayName { get => displayName; set { if (IsArrayPrefab || IsRoot || ReadOnly) return; displayName = value; } }
        /// <summary>
        /// Parent element
        /// </summary>
        [Browsable(false)] public IJtParentNode? Parent { get => parent; set { parent = value; if (parent is null) return; template = parent.Template; } }
        /// <summary>
        /// Unique id; used in conditions
        /// </summary>
        [Category("General")] public string? Id { get => id; set { if (ReadOnly) return; id = value; } }

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

        public bool ReadOnly { get; private set; }
        /// <summary>
        /// Conditions of current element
        /// </summary>
        [Category("General")] public string? Condition { get; set; }

        [Browsable(false)] public virtual bool HasExternalSources { get; }


        /// <summary>
        /// Create empty instace of current element
        /// </summary>
        /// <param name="template">Root template</param>
        protected internal JtNode(JTemplate template)
        {
            this.template = template;
        }




        /// <summary>
        /// Create new instance with properties loaded form obj
        /// </summary>
        /// <param name="template">Root template</param>
        /// <param name="obj">Object to load proerties from</param>
        protected internal JtNode(JObject obj, JTemplate template)
        {
            this.template = template;
            Name = (string?)(obj["name"] ?? obj["n"]);
            Description = (string?)(obj["description"] ?? obj["desc"]);
            Required = (bool)(obj["required"] ?? obj["req"] ?? false);
            DisplayName = (string?)(obj["displayName"] ?? obj["dn"] ?? Name);
            Id = (string?)obj["id"];
            Condition = (string?)obj["condition"];



            if (Condition != null)
                return;

            if (obj["if"] is JValue jval && jval.Value is string con)
            {
                Condition = con;
            }

            if (obj["if"] is JArray conditions)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < conditions.Count; i++)
                {
                    if (i != 0)
                        sb.Append("||");
                    JObject item = (JObject)conditions[i];

                    sb.Append($"'$({(string?)item["id"]})'");

                    switch ((string?)item["type"])
                    {
                        case "Equal":
                            sb.Append("==");
                            break;
                        case "NotEqual":
                            sb.Append("!=");
                            break;
                        case "Less":
                            sb.Append('<');
                            break;
                        case "Bigger":
                            sb.Append('>');
                            break;
                        default:
                            throw new Exception($"Invalid condition type `{item["type"]}`");
                    }
                    sb.Append($"'{(string?)item["value"]}'");

                }
                Condition = sb.ToString();
            }
            else if (obj["conditions"] is JArray conditions2)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < conditions2.Count; i++)
                {
                    if (i != 0)
                        sb.Append("||");
                    JObject item = (JObject)conditions2[i];

                    sb.Append($"'$({(string?)item["id"]})'");

                    switch (((string?)item["type"])?.ToLower())
                    {
                        case "equal":
                            sb.Append("==");
                            break;
                        case "notEqual":
                            sb.Append("!=");
                            break;
                        case "less":
                            sb.Append('<');
                            break;
                        case "bigger":
                            sb.Append('>');
                            break;
                        default:
                            throw new Exception($"Invalid condition type `{item["type"]}`");
                    }
                    sb.Append($"'{(string?)item["value"]}'");

                }
                Condition = sb.ToString();
            }
        }

        /// <summary>
        /// Get all elements witch has this same name ant diffrent type in this same parent
        /// </summary>
        /// <returns></returns>
        public JtNode[] GetTwinFamily() => Parent is null ? (new JtNode[] { this }) : Parent.Children.Where(x => x.Name == Name && x.Condition == Condition).ToArray();

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

            if (!(Condition is null))
            {
                sb.Append($", \"condition\": \"{Condition}\"");
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
        /// Create new instace of <see cref="JtNode"/>
        /// </summary>
        /// <param name="item">Object to load <see cref="JtNode"/> from</param>
        /// <param name="template">Root template</param>
        /// <returns>New instace of JtToken</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public static JtNode Create(JObject item, JTemplate template)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));
            if (template is null)
                throw new ArgumentNullException(nameof(template));


            if (((JValue?)item["type"])?.Value is int typeId)
            {
                return JtNodeType.GetById(typeId).CreateInstance(item, template);
            }

            string typeString = (string?)item["type"] ?? throw new Exception($"Item '{item["name"]}' dont have type");

            return JtNodeType.GetByName(typeString).CreateInstance(item, template);

        }

        /// <summary>
        /// Creates <see cref="JToken"/> with default value of <see cref="JtNode"/>
        /// </summary>
        /// <returns>New <see cref="JToken"/> with default value </returns>
        public abstract JToken CreateDefaultValue();

    }
}
