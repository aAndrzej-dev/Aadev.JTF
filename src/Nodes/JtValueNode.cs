using System;
using System.ComponentModel;
using System.Text;
using Aadev.JTF.CustomSources.Nodes;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.Types;

public abstract class JtValueNode : JtNode
{
    private bool? constant;
    private bool? forceUsingSuggestions;
    private JtSuggestionsDisplayType? suggestionsDisplayType;

    public new JtValueNodeSource? Base => (JtValueNodeSource?)base.Base;

    [DefaultValue(false)] public bool ForceUsingSuggestions { get => forceUsingSuggestions ?? Base?.ForceUsingSuggestions ?? false; set => forceUsingSuggestions = value; }
    [DefaultValue(false)] public bool Constant { get => constant ?? Base?.Constant ?? false; set => constant = value; }
    public abstract IJtSuggestionCollection Suggestions { get; }

    [DefaultValue(JtSuggestionsDisplayType.Auto)] public JtSuggestionsDisplayType SuggestionsDisplayType { get => suggestionsDisplayType ?? Base?.SuggestionsDisplayType ?? JtSuggestionsDisplayType.Auto; set => suggestionsDisplayType = value; }

    private protected JtValueNode(IJtNodeParent parent) : base(parent)
    {
    }
    private protected JtValueNode(IJtNodeParent parent, JObject source) : base(parent, source)
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
    private protected JtValueNode(IJtNodeParent parent, JtValueNodeSource source, JToken? @override) : base(parent, source, @override)
    {
        if (@override is null)
            return;
        forceUsingSuggestions = (bool?)@override["forceSuggestions"];
        constant = (bool?)@override["constant"];
        if ((string?)@override["suggestionsDisplayType"] is string displayType)
        {
            if (displayType.Equals("window", StringComparison.OrdinalIgnoreCase))
                SuggestionsDisplayType = JtSuggestionsDisplayType.Window;
            else if (displayType.Equals("dropdown", StringComparison.OrdinalIgnoreCase))
                SuggestionsDisplayType = JtSuggestionsDisplayType.DropDown;
        }
    }

    private protected override void BuildCommonJson(StringBuilder sb)
    {
        base.BuildCommonJson(sb);

        if (!Suggestions.IsEmpty)
        {
            sb.Append($", \"suggestions\": ");
            Suggestions.BuildJson(sb);

            if (ForceUsingSuggestions)
                sb.Append(", \"forceSuggestions\": true");
            if (SuggestionsDisplayType is JtSuggestionsDisplayType.DropDown)
                sb.Append(", \"suggestionsDisplayType\": \"dropdown\"");
            else if (SuggestionsDisplayType is JtSuggestionsDisplayType.Window)
                sb.Append(", \"suggestionsDisplayType\": \"window\"");

        }

        if (Constant)
            sb.Append(", \"constant\": true");
    }
    public abstract object GetDefaultValue();

    public abstract string? GetDisplayString(JToken? value);

    public abstract IJtSuggestionCollection? TryGetSuggestions();
}
