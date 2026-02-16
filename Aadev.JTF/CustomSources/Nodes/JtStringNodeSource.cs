using System.ComponentModel;
using Aadev.JTF.Nodes;
using Aadev.JTF.Tools;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.CustomSources.Nodes;

public sealed class JtStringNodeSource : JtValueNodeSource
{
    private IJtSuggestionCollectionSource? suggestions;
    public override JtNodeType Type => JtNodeType.String;


    [DefaultValue("")] public string Default { get; set; }
    [DefaultValue(-1)] public int MaxLength { get; set; }
    [DefaultValue(0)] public int MinLength { get; set; }
    public override IJtSuggestionCollectionSource Suggestions => suggestions ??= JtSuggestionCollectionSource<string>.Create(this);

    public override JTokenType JsonType => JTokenType.String;
    internal static JtStringNodeSource CreateSelf(IJtNodeSourceParent parent) => new JtStringNodeSource(parent);
    internal static JtStringNodeSource CreateSelf(IJtNodeSourceParent parent, JObject source) => new JtStringNodeSource(parent, source);
    public JtStringNodeSource(IJtNodeSourceParent parent) : base(parent)
    {
        Default = string.Empty;
        MaxLength = -1;
        MinLength = 0;
    }
    internal JtStringNodeSource(JtStringNode node) : base(node)
    {
        MinLength = node.MinLength;
        MaxLength = node.MaxLength;
        Default = node.Default;
        suggestions = node.TryGetSuggestions()?.CreateSource(this);
    }
    internal JtStringNodeSource(IJtNodeSourceParent parent, JObject source) : base(parent, source)
    {
        MinLength = (int)(source["minLength"] ?? 0);
        MaxLength = (int)(source["maxLength"] ?? -1);
        Default = (string?)source["default"] ?? string.Empty;
        suggestions = JtSuggestionCollectionSource<string>.TryCreate(this, source["suggestions"]);
    }
    internal JtStringNodeSource(IJtNodeSourceParent parent, JtStringNodeSource @base, JObject? @override) : base(parent, @base, @override)
    {
        MinLength = (int)(@override?["minLength"] ?? @base.MinLength);
        MaxLength = (int)(@override?["maxLength"] ?? @base.MaxLength);
        Default = (string?)@override?["default"] ?? @base.Default;
        suggestions = @base.Suggestions;
    }


    internal override void BuildJsonDeclaration(JsonBuilder jb)
    {
        BuildCommonJson(jb);
        if (MaxLength != -1)
            jb.AddProperty("maxLength", MaxLength);
        if (MinLength != 0)
            jb.AddProperty("minLength", MinLength);
        if (!string.IsNullOrEmpty(Default))
            jb.AddProperty("default", Default);
        if (!Suggestions.IsEmpty)
        {
            jb.AddProperty("suggestions", Suggestions);
        }

        jb.EndBlock();
    }
    public override JtNode CreateInstance(IJtNodeParent parent, JToken? @override) => new JtStringNode(parent, this, @override);
    public override JtNodeSource CreateOverride(IJtNodeSourceParent parent, JObject? @override) => new JtStringNodeSource(parent, this, @override);
    public override JToken CreateDefaultValue() => new JValue(Default);
    internal override IJtSuggestionCollectionSource? TryGetSuggestions() => suggestions;
}