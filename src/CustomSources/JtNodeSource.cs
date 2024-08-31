using System.ComponentModel;
using Aadev.JTF.Common;
using Aadev.JTF.CustomSources.Declarations;
using Aadev.JTF.CustomSources.Nodes;
using Aadev.JTF.Tools;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.CustomSources;

public abstract class JtNodeSource : CustomSource, IJtSourceStructureElement, IJtCommonNode
{
    private readonly JtNodeSource? @base;
    private string? name;

    [Browsable(false)]
    public abstract JtNodeType Type { get; }
    [Category("General")]
    public string? Name
    {
        get => name; set
        {

            if (DisplayName == name)
                DisplayName = value;
            name = value;
        }
    }
    [Category("General")]
    public string? Description { get; set; }
    [Category("General")]
    public string? DisplayName { get; set; }
    [Category("General")]
    public string? Condition { get; set; }
    [Category("General")]
    public JtIdentifier Id { get; set; }
    [Category("General")]
    public bool Required { get; set; }
    [Browsable(false)]
    public bool IsArrayPrefab => Parent?.Owner is JtArrayNodeSource;
    [Browsable(false)]
    public bool IsDynamicName => Parent?.Owner is JtArrayNodeSource { ContainerJsonType: JtContainerType.Block };
    [Browsable(false)]
    public abstract JTokenType JsonType { get; }
    public override IJtCustomSourceDeclaration? BaseDeclaration => base.BaseDeclaration ?? @base?.Declaration;
    [Browsable(false)] public override bool IsExternal => @base?.IsDeclared ?? IsDeclared;

    [Browsable(false)] public new IJtNodeSourceParent? Parent => (IJtNodeSourceParent?)base.Parent;

    IJtCommonParent? IJtCommonContentElement.Parent => Parent;

    IJtCommonRoot IJtCommonContentElement.Root => Declaration;

    bool IJtCommonContentElement.IsRootChild => IsDeclared;

    private protected JtNodeSource(IJtNodeSourceParent parent) : base(parent) { }

    private protected JtNodeSource(JtNode node) : base(new CustomSourceFormInstanceDeclaration(node, null))
    {
        Name = node.Name;
        Description = node.Description;
        Required = node.Required;
        DisplayName = node.DisplayName;
        Id = node.Id;
        Condition = node.Condition;
    }
    private protected JtNodeSource(IJtNodeSourceParent parent, JObject? source) : base(parent)
    {
        if (source is null)
            return;
        Name = (string?)source["name"];
        Description = (string?)source["description"];
        Required = (bool)(source["required"] ?? false);
        DisplayName = (string?)(source["displayName"] ?? Name);
        Id = (string?)source["id"];
        Condition = (string?)source["condition"];
    }
    private protected JtNodeSource(IJtNodeSourceParent parent, JtNodeSource @base, JObject? @override) : base(parent)
    {
        Name = (string?)(@override?["name"] ?? @base.Name);
        Description = (string?)(@override?["description"] ?? @base.Description);
        Required = (bool)(@override?["required"] ?? @base.Required);
        DisplayName = (string?)(@override?["displayName"] ?? @override?["name"] ?? @base.DisplayName);
        Id = (string?)(@override?["id"] ?? @base.Id.Value);
        Condition = (string?)(@override?["condition"] ?? @base.Condition);
        this.@base = @base;
    }

    private protected virtual void BuildCommonJson(JsonBuilder jb)
    {
        jb.StartBlock();
        if (!IsArrayPrefab || !string.IsNullOrEmpty(Name))
            jb.AddProperty("name", Name);
        jb.AddProperty("type", Type.Name);
        if (!string.IsNullOrWhiteSpace(Description))
            jb.AddProperty("description", Description);
        if (DisplayName != Name)
            jb.AddProperty("displayName", DisplayName);
        if (!string.IsNullOrEmpty(Id.Value))
            jb.AddProperty("id", Id.Value);
        if (Required)
            jb.AddProperty("required", true);
        if (!string.IsNullOrEmpty(Condition))
            jb.AddProperty("condition", Condition);
    }
    public abstract JtNodeSource CreateOverride(IJtNodeSourceParent parent, JObject? @override);
    public abstract JtNode CreateInstance(IJtNodeParent parent, JToken? @override);
    internal static JtNodeSource Create(IJtNodeSourceParent parent, JtNodeType type) => type.CreateEmptySource(parent);
    internal static JtNodeSource Create(IJtNodeSourceParent parent, JToken source)
    {
        if (source.Type is JTokenType.String)
        {
            return parent.SourceProvider.GetCustomSource<JtNodeSource>((string)source!) ?? new JtUnknownNodeSource(parent, null);
        }

        return JtNodeType.GetByName((string?)source["type"]).CreateSource(parent, (JObject)source);
    }


    IJtInstanceStructureElement IJtSourceStructureElement.CreateInstance(IJtNodeParent parent, JToken? @override) => CreateInstance(parent, @override);
    IJtSourceStructureElement IJtSourceStructureElement.CreateOverride(IJtNodeSourceParent parent, JToken? @override) => CreateOverride(parent, (JObject?)@override);
    public abstract JToken CreateDefaultValue();
    public virtual bool IsOverridden()
    {
        if (@base is null)
            return false;

        if ((Name == @base.Name) && (DisplayName == @base.DisplayName) && (Required == @base.Required) && (Condition == @base.Condition) && (Id == @base.Id) && (Description == @base.Description))
        {
            return false;
        }

        return true;
    }

    public string GetJson()
    {
        JsonBuilder jb = new JsonBuilder();
        BuildJsonDeclaration(jb);
        return jb.ToString();
    }
}
