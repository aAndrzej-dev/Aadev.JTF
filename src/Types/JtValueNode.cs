using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public abstract class JtValueNode : JtNode
    {
        private bool? constant;
        private bool? forceUsingSuggestions;


        public new JtValueNodeSource? Base => (JtValueNodeSource?)base.Base;

        [DefaultValue(false)] public bool ForceUsingSuggestions { get => forceUsingSuggestions ?? Base?.ForceUsingSuggestions ?? false; set => forceUsingSuggestions = value; }
        [DefaultValue(false)] public bool Constant { get => constant ?? Base?.Constant ?? false; set => constant = value; }
        public abstract IJtSuggestionCollection Suggestions { get; }

        private protected JtValueNode(IJtNodeParent parent) : base(parent)
        {
        }
        private protected JtValueNode(IJtNodeParent parent, JObject source) : base(parent, source)
        {
            ForceUsingSuggestions = (bool?)source["forceSuggestions"] ?? false;
            Constant = (bool?)source["constant"] ?? false;
        }
        private protected JtValueNode(IJtNodeParent parent, JtValueNodeSource source, JToken? @override) : base(parent, source, @override)
        {
            if (@override is null)
                return;
            forceUsingSuggestions = (bool?)@override["forceSuggestions"];
            constant = (bool?)@override["constant"];
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
            }
            if (Constant)
                sb.Append(", \"constant\": true");
        }
        public abstract object GetDefaultValue();

        public abstract string? GetDisplayString(JToken? value);
        
    }
}
