using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Aadev.JTF
{
    public abstract class JtNode
    {


        private JtContainer? parent;
        private JTemplate template;
        private string? name;
        private string? displayName;
        private string? description;
        private bool required;
        private string? id;
        private string? condition;

        [Browsable(false)]
        public IIdentifiersManager IdentifiersManager { get; }
        [Browsable(false)]
        public abstract JTokenType JsonType { get; }
        [Browsable(false)]
        public abstract JtNodeType Type { get; }


        [Category("General"), Description("Name of current token used as key in json file"), RefreshProperties(RefreshProperties.All)]
        public string? Name
        {
            get => IsRoot ? "root" : name;
            set
            {
                if (IsArrayPrefab || IsRoot || ReadOnly)
                    return;
                if (DisplayName == name)
                    DisplayName = value;
                name = value;
            }
        }
        [Category("General")]
        public string? Description
        {
            get => description; set
            {
                if (ReadOnly)
                    return;
                description = value;
            }
        }
        [DefaultValue(false), Category("General")]
        public bool Required
        {
            get => required; set
            {
                if (ReadOnly)
                    return;
                required = value;
            }
        }
        [Category("General")]
        public string? DisplayName
        {
            get => IsRoot ? "root" : displayName;
            set
            {
                if (IsArrayPrefab || IsRoot || ReadOnly)
                    return;
                displayName = value;
            }
        }
        [Browsable(false)]
        public JtContainer? Parent
        {
            get => parent; set
            {
                parent = value;
                if (parent is null)
                    return;
                template = parent.Template;
            }
        }
        [Category("General")]
        public string? Id
        {
            get => id;
            set
            {


                if (ReadOnly)
                    return;
                if (id != null)
                {
                    IdentifiersManager.UnregisterNode(id!);
                }

                if (value is null)
                    return;

                if (JTemplate.identifierRegex.IsMatch(value.ToLower()))
                {
                    id = value.ToLower();
                    IdentifiersManager.RegisterNode(id, this);
                }
                else
                {
                    throw new Exception($"Invalid id: {value}");
                }

            }
        }
        [Category("General")] public string? Condition { get => condition; set => condition = value; }



        [Browsable(false)] public JTemplate Template => template;
        [Browsable(false)] public bool IsArrayPrefab => Parent?.ContainerDisplayType is JtContainerType.Array;
        [Browsable(false)] public bool IsInArrayPrefab => IsArrayPrefab || Parent?.IsInArrayPrefab is true;
        [Browsable(false)] public bool IsDynamicName => Parent is { ContainerDisplayType: JtContainerType.Array, ContainerJsonType: JtContainerType.Block };
        [Browsable(false)] public bool IsRoot => Template.Root == this;
        [Browsable(false)] public bool ReadOnly { get; private set; }
        [Browsable(false)] public virtual bool HasExternalSources { get; }
        [Browsable(false)] public bool IsExternal => HasExternalSources || Parent?.IsExternal is true;


        protected internal JtNode(JTemplate template, IIdentifiersManager identifiersManager)
        {
            this.template = template;
            IdentifiersManager = identifiersManager;
        }
        protected internal JtNode(JObject obj, JTemplate template, IIdentifiersManager identifiersManager)
        {
            this.template = template;
            IdentifiersManager = identifiersManager;

            Name = (string?)obj["name"];
            Description = (string?)obj["description"];
            Required = (bool)(obj["required"] ?? false);
            DisplayName = (string?)(obj["displayName"] ?? Name);
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
        public JtNode[] GetTwinFamily() => Parent is null ? (new JtNode[] { this }) : Parent.Children.Where(x => x.Name == Name && x.Condition == Condition).ToArray();
        internal abstract void BulidJson(StringBuilder sb);
        protected internal virtual void BuildCommonJson(StringBuilder sb)
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
            if (Required)
                sb.Append(", \"required\": true");
            if (!(Condition is null))
            {
                sb.Append($", \"condition\": \"{Condition}\"");
            }
        }
        public string GetJson()
        {
            StringBuilder sb = new StringBuilder();
            BulidJson(sb);
            return sb.ToString();
        }
        public static JtNode Create(JObject item, JTemplate template, IIdentifiersManager identifiersManager)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));
            if (template is null)
                throw new ArgumentNullException(nameof(template));
            if (((JValue?)item["type"])?.Value is int typeId)
            {
                if (typeId == 11)
                {
                    bool allowCustom = (bool?)item["allowCustom"] ?? false;
                    if (!allowCustom)
                    {
                        item["forceSuggestions"] = true;
                    }
                }


                return JtNodeType.GetById(typeId).CreateInstance(item, template, identifiersManager);
            }
            string typeString = (string?)item["type"] ?? throw new Exception($"Item '{item["name"]}' dont have type");

            if (typeString.ToLower() is "enum")
            {
                bool allowCustom = (bool?)item["allowCustom"] ?? false;
                if (!allowCustom)
                {
                    item["forceSuggestions"] = true;
                }
            }

            return JtNodeType.GetByName(typeString).CreateInstance(item, template, identifiersManager);
        }
        public abstract JToken CreateDefaultValue();
    }
}