using System;
using System.ComponentModel;
using Aadev.JTF.Nodes;
using Aadev.JTF.Tools;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.CustomSources.Nodes;

public abstract class JtValueNodeSource : JtNodeSource
{
    public abstract IJtSuggestionCollectionSource Suggestions { get; }
    [DefaultValue(false)] public bool ForceUsingSuggestions { get; set; }
    [DefaultValue(false)] public bool Constant { get; set; }
    [DefaultValue(JtSuggestionsDisplayType.Auto)] public JtSuggestionsDisplayType SuggestionsDisplayType { get; set; }

    private protected JtValueNodeSource(IJtNodeSourceParent parent) : base(parent) { }
    private protected JtValueNodeSource(JtValueNode node) : base(node)
    {
        ForceUsingSuggestions = node.ForceUsingSuggestions;
        Constant = node.Constant;
        SuggestionsDisplayType = node.SuggestionsDisplayType;
    }
    private protected JtValueNodeSource(IJtNodeSourceParent parent, JObject source) : base(parent, source)
    {
        ForceUsingSuggestions = (bool?)source["forceSuggestions"] ?? false;
        Constant = (bool?)source["constant"] ?? false;
        if ((string?)source["suggestionsDisplayType"] is string displayType)
        {
            if (displayType.Equals("window", StringComparison.OrdinalIgnoreCase))
                SuggestionsDisplayType = JtSuggestionsDisplayType.Window;
            else if (displayType.Equals("dropdown", StringComparison.OrdinalIgnoreCase))
                SuggestionsDisplayType = JtSuggestionsDisplayType.DropDown;
            else
                SuggestionsDisplayType = JtSuggestionsDisplayType.Auto;
        }
        else
            SuggestionsDisplayType = JtSuggestionsDisplayType.Auto;
    }
    private protected JtValueNodeSource(IJtNodeSourceParent parent, JtValueNodeSource @base, JObject? @override) : base(parent, @base, @override)
    {
        ForceUsingSuggestions = (bool)(@override?["forceSuggestions"] ?? @base.ForceUsingSuggestions);
        Constant = (bool)(@override?["constant"] ?? @base.Constant);
        if ((string?)@override?["suggestionsDisplayType"] is string displayType)
        {
            if (displayType.Equals("window", StringComparison.OrdinalIgnoreCase))
                SuggestionsDisplayType = JtSuggestionsDisplayType.Window;
            else if (displayType.Equals("dropdown", StringComparison.OrdinalIgnoreCase))
                SuggestionsDisplayType = JtSuggestionsDisplayType.DropDown;
            else
                SuggestionsDisplayType = JtSuggestionsDisplayType.Auto;
        }
        else
            SuggestionsDisplayType = @base.SuggestionsDisplayType;
    }


    private protected override void BuildCommonJson(JsonBuilder jb)
    {
        base.BuildCommonJson(jb);
        if (ForceUsingSuggestions)
            jb.AddProperty("forceSuggestions", true);
        if (Constant)
            jb.AddProperty("constant", true);
    }
    internal abstract IJtSuggestionCollectionSource? TryGetSuggestions();
}
