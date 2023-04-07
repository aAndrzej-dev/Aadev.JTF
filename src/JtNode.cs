using Aadev.JTF.AbstractStructure;
using Aadev.JTF.CustomSources;
using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Aadev.JTF
{
    [DebuggerDisplay("JtNode {Name}:{Type.Name} ({Id.ToString()})")]
    public abstract class JtNode : ICustomSourceProvider, IJtNodeCollectionChild, IJtStructureNodeElement
    {
        private IJtNodeParent parent;
        private JTemplate template;
        private IIdentifiersManager? identifiersManager;
        internal JtNodeSource? currentSource;
        
        private string? name;
        private string? displayName;
        private JtIdentifier id;
        private string? description;
        private bool? required;
        private string? condition;

        public JtNodeSource? Base { get; }


        [Browsable(false)]
        public IIdentifiersManager IdentifiersManager => identifiersManager ??= Parent.GetIdentifiersManagerForChild();


        [Browsable(false)] public abstract JTokenType JsonType { get; }


        [Browsable(false)] public abstract JtNodeType Type { get; }

        /// <summary>
        /// Name of current token used as key in json file
        /// </summary>
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
        [Category("General")] public string? Description { get => description ?? Base?.Description; set => description = value; }
        [DefaultValue(false), Category("General")] public bool Required { get => required ?? Base?.Required ?? false; set => required = value; }

        [Category("General"), Description("Display name used in JTF Json Editor"), DisplayName("Display Name")]
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
                if (JTemplate.identifierRegex.IsMatch(value.Value))
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
        [Browsable(false)] public bool IsRoot => Parent == Template || Template.Roots.Contains(this);


        [MemberNotNullWhen(true, nameof(Base))]
        [Browsable(false)] public bool IsExternal => Base?.IsDeclared ?? false;

        IJtCustomSourceDeclaration? IJtStructureInnerElement.Base => IsExternal ? Base.Declaration : null;

        private protected JtNode(IJtNodeParent parent)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
            template = parent.Template;
        }
        private protected JtNode(IJtNodeParent parent, JObject source)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
            template = parent.Template;

            Name = (string?)source["name"];
            Description = (string?)source["description"];
            Required = (bool)(source["required"] ?? false);
            DisplayName = (string?)(source["displayName"] ?? Name);
            Id = (string?)source["id"];
            Condition = (string?)source["condition"] ?? (string?)source["if"];



            if (Condition is not null)
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
            else if (Parent.Owner is null or JtArrayNode)
                return new JtNode[] { this };
            else
                return Parent.Owner.Children.Nodes!.Where(x => x.Name == Name && x.Condition == Condition);
        }
        internal abstract void BuildJson(StringBuilder sb);
        private protected virtual void BuildCommonJson(StringBuilder sb)
        {
            sb.Append('{');
            if (Base is not null)
            {
                bool needComma = false;
                if (Base.IsDeclared)
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

        public virtual bool IsOverridden()
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
                    return CreateUnknown(parent);
                JtSourceReference customResourceIdentifier = new JtSourceReference(id);
                if (customResourceIdentifier.Type is JtSourceReferenceType.None)
                    return CreateUnknown(parent);

                return parent.SourceProvider.GetCustomSource<JtNodeSource>(id)?.CreateInstance(parent, null) ?? CreateUnknown(parent);

            }
            if (source["base"]?.Type is JTokenType.String)
            {
                string? baseId = (string?)source["base"];
                if (baseId is null)
                    return CreateUnknown(parent);
                JtSourceReference baseReference = new JtSourceReference(baseId);
                if (baseReference.Type is JtSourceReferenceType.None)
                    return CreateUnknown(parent);
                return parent.SourceProvider.GetCustomSource<JtNodeSource>(baseReference)?.CreateInstance(parent, source) ?? CreateUnknown(parent);

            }

            if (((JValue?)source["type"])?.Value is int typeId)
            {
                if (typeId is 11) //Enum type
                {
                    bool allowCustom = (bool?)source["allowCustom"] ?? false;
                    if (!allowCustom)
                    {
                        source["forceSuggestions"] = true;
                    }
                }


                return JtNodeType.GetById(typeId).CreateInstance(parent, (JObject)source);
            }
            string typeString = (string?)source["type"] ?? throw new JtfException($"Item '{source["name"]}' doesn't have type");

            if (typeString.Equals("enum", StringComparison.OrdinalIgnoreCase))
            {
                bool allowCustom = (bool?)source["allowCustom"] ?? false;
                if (!allowCustom)
                {
                    source["forceSuggestions"] = true;
                }
            }

            return JtNodeType.GetByName(typeString).CreateInstance(parent, (JObject)source);

            static JtUnknownNode CreateUnknown(IJtNodeParent parent) => new JtUnknownNode(parent);
        }
        [return: NotNullIfNotNull(nameof(type))]
        public static JtNode? Create(IJtNodeParent parent, JtNodeType? type) => type?.CreateEmptyInstance(parent);
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