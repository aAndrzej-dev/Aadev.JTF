using Aadev.JTF.CustomSources;
using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Aadev.JTF
{
    public abstract class JtNode : ICustomSourceProvider, IJtNodeCollectionChild
    {
        private IJtNodeParent parent;
        private JTemplate template;
        private string? name;
        private string? displayName;
        private JtIdentifier id;
        private string? description;
        private bool? required;
        public JtNodeSource? Base { get; }

        internal JtNodeSource? currentSource;
        private string? condition;

        [Browsable(false)] public IIdentifiersManager IdentifiersManager { get; }
        [Browsable(false)] public abstract JTokenType JsonType { get; }
        [Browsable(false)] public abstract JtNodeType Type { get; }


        [Category("General"), Description("Name of current token used as key in json file"), RefreshProperties(RefreshProperties.All)]
        public string? Name
        {
            get => name ?? Base?.Name;
            set
            {
                if (IsArrayPrefab || IsRoot)
                    return;
                if (DisplayName == name)
                    DisplayName = value;
                name = value;
            }
        }
        [Category("General")] public string? Description { get => description ?? Base?.Description; set => description = value; }
        [DefaultValue(false), Category("General")] public bool Required { get => required ?? Base?.Required ?? false; set => required = value; }
        [Category("General")]
        public string? DisplayName
        {
            get => displayName ?? Base?.DisplayName;
            set
            {
                if (IsArrayPrefab || IsRoot)
                    return;
                displayName = value;
            }
        }
        [Browsable(false)]
        public IJtNodeParent Parent
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
        public JtIdentifier Id
        {
            get => id.IsEmpty ? (Base?.Id ?? JtIdentifier.Empty) : id;
            set
            {
                if (!id.IsEmpty)
                {
                    IdentifiersManager.UnregisterNode(id);
                }

                if (value.IsEmpty)
                    return;
                id = value;
                if (JTemplate.identifierRegex.IsMatch(value.Identifier))
                {
                    IdentifiersManager.RegisterNode(value, this);
                }
                else
                {
                    throw new JtfException($"Invalid identifier: {value}");
                }

            }
        }
        [Category("General")] public string? Condition { get => condition ?? Base?.Condition; set => condition = value; }



        [Browsable(false)] public JTemplate Template => template;
        [Browsable(false)] public bool IsArrayPrefab => Parent.Owner?.ContainerDisplayType is JtContainerType.Array;
        [Browsable(false)] public bool IsInArrayPrefab => IsArrayPrefab || Parent.Owner?.IsInArrayPrefab is true;
        [Browsable(false)] public bool IsDynamicName => Parent.Owner is { ContainerDisplayType: JtContainerType.Array, ContainerJsonType: JtContainerType.Block };
        [Browsable(false)] public bool IsRoot => Template.Roots.Contains(this);


        protected internal JtNode(IJtNodeParent parent)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
            template = parent.Template;
            IdentifiersManager = parent.GetIdentifiersManagerForChild();
        }
        protected internal JtNode(JObject obj, IJtNodeParent parent)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
            template = parent.Template;
            IdentifiersManager = parent.GetIdentifiersManagerForChild();

            Name = (string?)obj["name"];
            Description = (string?)obj["description"];
            Required = (bool)(obj["required"] ?? false);
            DisplayName = (string?)(obj["displayName"] ?? Name);
            Id = (string?)obj["id"];
            Condition = (string?)obj["condition"] ?? (string?)obj["if"];



            if (Condition != null)
                return;
            if (obj["if"] is JArray conditions)
            {
                Condition = ConvertLegacyCondition(conditions);
            }
            else if (obj["conditions"] is JArray conditions2)
            {
                Condition = ConvertLegacyCondition(conditions2);
            }
        }


        protected internal JtNode(JtNodeSource source, JToken? @override, IJtNodeParent parent)
        {
            Base = source;
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
            template = parent.Template;
            IdentifiersManager = parent.GetIdentifiersManagerForChild();

            if (@override is null)
            {
                if (!Id.IsEmpty)
                {
                    IdentifiersManager.RegisterNode(Id, this);
                }
                return;
            }


            Name = (string?)@override["name"];
            Description = (string?)@override["description"];
            required = (bool?)@override["required"];
            DisplayName = (string?)(@override["displayName"] ?? @override["name"]);
            Id = (string?)@override["id"];
            Condition = (string?)@override["condition"];
            if (!Id.IsEmpty)
            {
                IdentifiersManager.RegisterNode(Id, this);
            }

        }

        public JtNode[] GetTwinFamily()
        {
            if (IsRoot)
                return Template.Roots.ToArray();
            else if (Parent.Owner is null || Parent.Owner is JtArray)
                return new JtNode[] { this };
            else
                return Parent.Owner.Children.Nodes!.Where(x => x.Name == Name && x.Condition == Condition).ToArray();
        }
        internal abstract void BuildJson(StringBuilder sb);
        protected internal virtual void BuildCommonJson(StringBuilder sb)
        {
            sb.Append('{');
            if (Base != null)
            {
                bool needComma = false;
                if (Base.IsDeclarated)
                {
                    sb.Append($"\"base\": ");
                    Base.BuildJson(sb);
                    needComma = true;
                }

                if (Name != Base.Name)
                {
                    if (needComma)
                        sb.Append(',');
                    sb.Append($"\"name\": \"{Name}\"");
                    needComma = true;
                }
                if (Description != Base.Description)
                {
                    if (needComma)
                        sb.Append(',');
                    sb.Append($"\"description\": \"{Description}\"");
                    needComma = true;
                }
                if (DisplayName != Base.DisplayName && DisplayName != Name)
                {
                    if (needComma)
                        sb.Append(',');
                    sb.Append($"\"displayName\": \"{DisplayName}\"");
                    needComma = true;
                }
                if (Id != Base.Id)
                {
                    if (needComma)
                        sb.Append(',');
                    sb.Append($"\"id\": \"{Id}\"");
                    needComma = true;
                }
                if (Required != Base.Required)
                {
                    if (needComma)
                        sb.Append(',');
                    sb.Append($"\"required\": {Required}");
                    needComma = true;
                }
                if (Condition != Base.Condition)
                {
                    if (needComma)
                        sb.Append(',');
                    sb.Append($"\"condition\": \"{Condition}\"");
                }
                return;
            }
            if (!IsArrayPrefab && !IsRoot)
                sb.Append($"\"name\": \"{Name}\",");
            sb.Append($"\"type\": \"{Type.Name}\"");
            if (!string.IsNullOrWhiteSpace(Description))
                sb.Append($", \"description\": \"{Description}\"");
            if (DisplayName != Name)
                sb.Append($", \"displayName\": \"{DisplayName}\"");
            if (!id.IsEmpty)
                sb.Append($", \"id\": \"{Id}\"");
            if (Required)
                sb.Append(", \"required\": true");
            if (!string.IsNullOrEmpty(Condition))
                sb.Append($", \"condition\": \"{Condition}\"");

        }

        public virtual bool IsOverriden()
        {
            if (Base is null)
                return false;

            if ((name is null || name == Base.Name) && (displayName is null || displayName == Base.DisplayName) && (required is null || required == Base.Required) && (condition is null || condition == Base.Condition) && (id.IsEmpty || id == Base.Id) && (description is null || description == Base.Description))
            {
                return false;
            }
            return true;
        }

        public string GetJson()
        {
            StringBuilder sb = new StringBuilder();
            BuildJson(sb);
            return sb.ToString();
        }
        public static JtNode Create(JToken item, IJtNodeParent parent, ICustomSourceProvider sourceProvider)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));
            if (parent is null)
                throw new ArgumentNullException(nameof(parent));

            if (item.Type is JTokenType.String)
            {
                string? id = (string?)item;
                if (id is null)
                    return new JtUnknown(parent);
                JtCustomResourceIdentifier customResourceIdentifier = new JtCustomResourceIdentifier(id);
                if (customResourceIdentifier.Type is JtCustomResourceIdentifierType.None)
                    return new JtUnknown(parent);

                return sourceProvider.GetCustomSource<JtNodeSource>(id)?.CreateInstance(null, parent)!;

            }
            if (item["base"]?.Type is JTokenType.String)
            {
                return sourceProvider.GetCustomSource<JtNodeSource>((string)item["base"]!)?.CreateInstance(item, parent) ?? new JtUnknown(parent);

            }

            if (((JValue?)item["type"])?.Value is int typeId)
            {
                if (typeId is 11)
                {
                    bool allowCustom = (bool?)item["allowCustom"] ?? false;
                    if (!allowCustom)
                    {
                        item["forceSuggestions"] = true;
                    }
                }


                return JtNodeType.GetById(typeId).CreateInstance((JObject)item, parent);
            }
            string typeString = (string?)item["type"] ?? throw new JtfException($"Item '{item["name"]}' dont have type");

            if (typeString.ToLowerInvariant() is "enum")
            {
                bool allowCustom = (bool?)item["allowCustom"] ?? false;
                if (!allowCustom)
                {
                    item["forceSuggestions"] = true;
                }
            }

            return JtNodeType.GetByName(typeString).CreateInstance((JObject)item, parent);
        }
        public abstract JToken CreateDefaultValue();
        public abstract JtNodeSource CreateSource();

        internal static string ConvertLegacyCondition(JArray legacyCondition)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < legacyCondition.Count; i++)
            {
                if (i != 0)
                    sb.Append("||");
                JObject item = (JObject)legacyCondition[i];
                sb.Append($"'$({(string?)item["id"]})'");
                switch (((string?)item["type"])?.ToLowerInvariant())
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
                        throw new JtfException($"Invalid condition type `{item["type"]}`");
                }
                sb.Append($"'{(string?)item["value"]}'");
            }
            return sb.ToString();
        }

        public T? GetCustomSource<T>(JtCustomResourceIdentifier identifier) where T : CustomSource
        {
            if (identifier.Type is JtCustomResourceIdentifierType.Direct)
            {
                if (IdentifiersManager.GetNodeById(identifier.Identifier)?.CreateSource() is T value)
                    return value;
                return null;
            }
            return Template.GetCustomSource<T>(identifier);
        }
        public CustomSource? GetCustomSource(JtCustomResourceIdentifier identifier) => GetCustomSource<CustomSource>(identifier);
        public IEnumerable<JtNode> GetNodes()
        {
            yield return this;
        }

        void IJtNodeCollectionChild.BuildJson(StringBuilder sb) => BuildJson(sb);
    }
}