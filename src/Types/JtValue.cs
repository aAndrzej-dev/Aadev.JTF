using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public abstract class JtValue : JtNode
    {
        private bool? constant;
        private bool? forecUsingSuggestions;


        public new JtValueNodeSource? Base => (JtValueNodeSource?)base.Base;

        [DefaultValue(false)] public bool ForecUsingSuggestions { get => forecUsingSuggestions ?? Base?.ForecUsingSuggestions ?? false; set => forecUsingSuggestions = value; }
        [DefaultValue(false)] public bool Constant { get => constant ?? Base?.Constant ?? false; set => constant = value; }
        public abstract IJtSuggestionCollection Suggestions { get; }

        private protected JtValue(IJtNodeParent parent) : base(parent)
        {
        }
        private protected JtValue(IJtNodeParent parent, JObject source) : base(parent, source)
        {
            ForecUsingSuggestions = (bool?)source["forceSuggestions"] ?? false;
            Constant = (bool?)source["constant"] ?? false;
        }
        private protected JtValue(IJtNodeParent parent, JtValueNodeSource source, JToken? @override) : base(parent, source, @override)
        {
            if (@override is null)
                return;
            forecUsingSuggestions = (bool?)@override["forceSuggestions"];
            constant = (bool?)@override["constant"];
        }

        private protected override void BuildCommonJson(StringBuilder sb)
        {
            base.BuildCommonJson(sb);
            if (!Suggestions.IsEmpty)
            {
                sb.Append($", \"suggestions\": ");
                Suggestions.BuildJson(sb);

                if (ForecUsingSuggestions)
                    sb.Append(", \"forceSuggestions\": true");
            }
            if (Constant)
                sb.Append(", \"constant\": true");
        }
        public abstract object GetDefaultValue();
    }
}
