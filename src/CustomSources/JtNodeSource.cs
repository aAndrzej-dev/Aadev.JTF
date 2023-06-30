using System.ComponentModel;
using System.Text;
using Aadev.JTF.Common;
using Aadev.JTF.CustomSources.Declarations;
using Aadev.JTF.CustomSources.Nodes;
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
    public bool IsDynamicName => Parent?.Owner is JtArrayNodeSource { ContainerJsonType: Types.JtContainerType.Block };
    [Browsable(false)]
    public abstract JTokenType JsonType { get; }
    public override IJtCustomSourceDeclaration? BaseDeclaration => base.BaseDeclaration ?? @base?.Declaration;
    [Browsable(false)] public override bool IsExternal => @base?.IsDeclared ?? IsDeclared;

    [Browsable(false)] public new IJtNodeSourceParent? Parent => (IJtNodeSourceParent?)base.Parent;

    IJtCommonParent IJtCommonContentElement.Parent => (IJtCommonParent)Parent;

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

    private protected virtual void BuildCommonJson(StringBuilder sb)
    {
        sb.Append('{');
        if (!IsArrayPrefab || !string.IsNullOrEmpty(Name))
            sb.Append($"\"name\": \"{Name}\",");
        sb.Append($"\"type\": \"{Type.Name}\"");
        if (!string.IsNullOrWhiteSpace(Description))
            sb.Append($", \"description\": \"{Description}\"");
        if (DisplayName != Name)
            sb.Append($", \"displayName\": \"{DisplayName}\"");
        if (!string.IsNullOrEmpty(Id.Value))
            sb.Append($", \"id\": \"{Id.Value}\"");
        if (Required)
            sb.Append(", \"required\": true");
        if (!string.IsNullOrEmpty(Condition))
            sb.Append($", \"condition\": \"{Condition}\"");
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
        StringBuilder sb = new StringBuilder();
        BuildJsonDeclaration(sb);
        return sb.ToString();
    }
}
