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
                if (IsRoot)
                    return;
                if (DisplayName == name)
                    DisplayName = value;
                name = value;
            }
        }
        [Category("General")] public string? Description { get => description ?? Base?.Description; set { description = value; } }
        [DefaultValue(false), Category("General")] public bool Required { get => required ?? Base?.Required ?? false; set { required = value; } }
        [Category("General")]
        public string? DisplayName
        {
            get => displayName ?? Base?.DisplayName;
            set
            {
                if (IsRoot)
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
        [Category("General")] public string? Condition { get => condition ?? Base?.Condition; set { condition = value; } }



        [Browsable(false)] public JTemplate Template => template;
        [Browsable(false)] public bool IsArrayPrefab => Parent.Owner?.ContainerDisplayType is JtContainerType.Array;
        [Browsable(false)] public bool IsInArrayPrefab => IsArrayPrefab || Parent.Owner?.IsInArrayPrefab is true;
        [Browsable(false)] public bool IsDynamicName => Parent.Owner is { ContainerDisplayType: JtContainerType.Array, ContainerJsonType: JtContainerType.Block };
        [Browsable(false)] public bool IsRoot => Parent == Template || Template.Roots.Contains(this);
        public bool IsFormTemplate => Parent is JTemplate || Parent.Owner?.IsFormTemplate is true;

        public bool IsExternal => Base?.IsDeclarated ?? false;

        private protected JtNode(IJtNodeParent parent)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
            template = parent.Template;
            IdentifiersManager = parent.GetIdentifiersManagerForChild();
        }
        private protected JtNode(IJtNodeParent parent, JObject source)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
            template = parent.Template;
            IdentifiersManager = parent.GetIdentifiersManagerForChild();

            Name = (string?)source["name"];
            Description = (string?)source["description"];
            Required = (bool)(source["required"] ?? false);
            DisplayName = (string?)(source["displayName"] ?? Name);
            Id = (string?)source["id"];
            Condition = (string?)source["condition"] ?? (string?)source["if"];



            if (Condition != null)
                return;
            if (source["if"] is JArray conditions)
            {
                Condition = ConvertLegacyCondition(conditions);
            }
            else if (source["conditions"] is JArray conditions2)
            {
                Condition = ConvertLegacyCondition(conditions2);
            }
        }


        private protected JtNode(IJtNodeParent parent, JtNodeSource source, JToken? @override)
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

        public IEnumerable<JtNode> GetTwinFamily()
        {
            if (IsRoot)
                return Template.Roots;
            else if (Parent.Owner is null || Parent.Owner is JtArray)
                return new JtNode[] { this };
            else
                return Parent.Owner.Children.Nodes!.Where(x => x.Name == Name && x.Condition == Condition);
        }
        internal abstract void BuildJson(StringBuilder sb);
        private protected virtual void BuildCommonJson(StringBuilder sb)
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

            if (!IsRoot && (!IsArrayPrefab || !string.IsNullOrEmpty(Name)))
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
        public static JtNode Create(IJtNodeParent parent, JToken source)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            if (parent is null)
                throw new ArgumentNullException(nameof(parent));

            if (source.Type is JTokenType.String)
            {
                string? id = (string?)source;
                if (id is null)
                    return new JtUnknown(parent);
                JtSourceReference customResourceIdentifier = new JtSourceReference(id);
                if (customResourceIdentifier.Type is JtSourceReferenceType.None)
                    return new JtUnknown(parent);

                return parent.SourceProvider.GetCustomSource<JtNodeSource>(id)?.CreateInstance(parent, null)!;

            }
            if (source["base"]?.Type is JTokenType.String)
            {
                return parent.SourceProvider.GetCustomSource<JtNodeSource>((string)source["base"]!)?.CreateInstance(parent, source) ?? new JtUnknown(parent);

            }

            if (((JValue?)source["type"])?.Value is int typeId)
            {
                if (typeId is 11)
                {
                    bool allowCustom = (bool?)source["allowCustom"] ?? false;
                    if (!allowCustom)
                    {
                        source["forceSuggestions"] = true;
                    }
                }


                return JtNodeType.GetById(typeId).CreateInstance(parent, (JObject)source);
            }
            string typeString = (string?)source["type"] ?? throw new JtfException($"Item '{source["name"]}' dont have type");

            if (typeString.Equals("enum", StringComparison.OrdinalIgnoreCase))
            {
                bool allowCustom = (bool?)source["allowCustom"] ?? false;
                if (!allowCustom)
                {
                    source["forceSuggestions"] = true;
                }
            }

            return JtNodeType.GetByName(typeString).CreateInstance(parent, (JObject)source);
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

        public T? GetCustomSource<T>(JtSourceReference identifier) where T : CustomSource
        {
            if (identifier.Type is JtSourceReferenceType.Direct)
            {
                if (IdentifiersManager.GetNodeById(identifier.Identifier)?.CreateSource() is T value)
                    return value;
                return null;
            }
            return Template.GetCustomSource<T>(identifier);
        }
        public CustomSource? GetCustomSource(JtSourceReference identifier) => GetCustomSource<CustomSource>(identifier);
        public IEnumerable<JtNode> GetNodes()
        {
            yield return this;
        }

        void IJtNodeCollectionChild.BuildJson(StringBuilder sb) => BuildJson(sb);
        IJtNodeCollectionSourceChild IJtNodeCollectionChild.CreateSource() => CreateSource();

    }

}