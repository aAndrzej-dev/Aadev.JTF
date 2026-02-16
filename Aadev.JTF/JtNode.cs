using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Aadev.JTF.Common;
using Aadev.JTF.CustomSources;
using Aadev.JTF.Nodes;
using Aadev.JTF.Tools;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF;

[DebuggerDisplay("JtNode {Name}:{Type.Name} ({Id.ToString()})")]
public abstract class JtNode : ICustomSourceProvider, IJtInstanceStructureElement, IJtCommonNode
{
    private IJtNodeParent parent;
    private JTemplate template;
    private IdentifiersManager? identifiersManager;
    internal JtNodeSource? currentSource;

    private string? name;
    private string? displayName;
    private JtIdentifier id;
    private string? description;
    private bool? required;
    private string? condition;

    public JtNodeSource? Base { get; set; }


    [Browsable(false)]
    public IdentifiersManager IdentifiersManager => identifiersManager ??= Parent.GetIdentifiersManagerForChild();


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
            if (IsRootChild)
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
            if (IsRootChild)
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
            if (RegularExpressions.identifierRegex.IsMatch(value.Value))
            {
                IdentifiersManager.RegisterNode(value, this);
            }
            else
            {
                throw new JtfException($"Invalid identifier: {value}", Template);
            }
        }
    }
    [Category("General")] public string? Condition { get => condition ?? Base?.Condition; set => condition = value; }



    [Browsable(false)] public JTemplate Template => template;
    [Browsable(false)] public bool IsArrayPrefab => Parent.Owner?.ContainerDisplayType is JtContainerType.Array;
    [Browsable(false)] public bool IsDynamicName => Parent.Owner is { ContainerDisplayType: JtContainerType.Array, ContainerJsonType: JtContainerType.Block };
    [Browsable(false)] public bool IsRootChild => Parent.OwnersMainCollection == Template.Roots;

    [MemberNotNullWhen(true, nameof(Base))]
    [Browsable(false)] public bool IsExternal => Base?.IsDeclared ?? false;

    IJtCommonParent IJtCommonContentElement.Parent => Parent;
    IJtCommonRoot IJtCommonContentElement.Root => Template;
    IJtCustomSourceDeclaration? IJtCommonContentElement.BaseDeclaration => IsExternal ? Base.Declaration : null;
    ICustomSourceProvider IHaveCustomSourceProvider.SourceProvider => this;


    private protected JtNode(IJtNodeParent parent)
    {
        this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
        template = parent.Template;
    }
    private protected JtNode(IJtNodeParent parent, JObject source)
    {
        this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
        template = parent.Template;

        if (source is null)
            return;

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
            Condition = LegacyExtensions.ConvertLegacyCondition(conditions);
        }
        else if (source["conditions"] is JArray conditions2)
        {
            Condition = LegacyExtensions.ConvertLegacyCondition(conditions2);
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
    public JtNode[] GetTwinFamily()
    {
        if (IsRootChild)
            return Template.Roots.Nodes.ToArray();
        else if (Parent.Owner is null or JtArrayNode)
            return new JtNode[] { this };
        else
            return Parent.Owner.Children.Nodes.Where(x => x.Name == Name).ToArray();
    }
    public IEnumerable<JtNode> EnumerateTwinFamily()
    {
        if (IsRootChild)
            return Template.Roots.Nodes;
        else if (Parent.Owner is null or JtArrayNode)
            return YieldCurrent();
        else
            return Parent.Owner.Children.Nodes.Where(x => x.Name == Name);

        IEnumerable<JtNode> YieldCurrent()
        {
            yield return this;
        }
    }
    internal abstract void BuildJson(JsonBuilder jb);
    private protected virtual void BuildCommonJson(JsonBuilder jb)
    {
        jb.StartBlock();
        if (Base is not null)
        {
            if (Base.IsDeclared)
                jb.AddProperty("base", Base);

            if (Name != Base.Name)
                jb.AddProperty("name", Name);

            if (Description != Base.Description)
                jb.AddProperty("description", Description);

            if (DisplayName != Base.DisplayName && DisplayName != Name)
                jb.AddProperty("displayName", DisplayName);

            if (Id != Base.Id)
                jb.AddProperty("id", Id);

            if (Required != Base.Required)
                jb.AddProperty("required", Required);

            if (Condition != Base.Condition)
                jb.AddProperty("condition", Condition);

            return;
        }

        if (!IsRootChild && (!IsArrayPrefab || !string.IsNullOrEmpty(Name)))
            jb.AddProperty("name", Name);

        jb.AddProperty("type", Type.Name);

        if (!string.IsNullOrWhiteSpace(Description))
            jb.AddProperty("description", Description);

        if (DisplayName != Name)
            jb.AddProperty("displayName", DisplayName);

        if (!id.IsEmpty)
            jb.AddProperty("id", Id);

        if (Required)
            jb.AddProperty("required", Required);

        if (!string.IsNullOrEmpty(Condition))
            jb.AddProperty("condition", Condition);
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
        JsonBuilder jb = new JsonBuilder();
        BuildJson(jb);
        return jb.ToString();
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

        string typeString = (string?)source["type"] ?? throw new JtfException($"Item '{source["name"]}' doesn't have type", parent.Template);

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
    IEnumerable<JtNode> IJtInstanceStructureElement.GetNodes()
    {
        yield return this;
    }

    IJtSourceStructureElement IJtInstanceStructureElement.CreateSource() => CreateSource();
    public IEnumerable<IJtCustomSourceDeclaration> EnumerateCustomSources()
    {
        return Enumerable.Concat(Template.EnumerateCustomSources(), IdentifiersManager.EnumerateRegisteredNodes().Select(x => x.CreateSource().Declaration));
    }

    void IJsonBuildable.BuildJson(JsonBuilder jb) => BuildJson(jb);
}