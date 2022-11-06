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

        public abstract Type ValueType { get; }
        public abstract IJtSuggestionCollection Suggestions { get; }
        public JtSuggestionDisplayType SuggestionDisplayType { get; set; }
        [DefaultValue(false)] public bool ForecUsingSuggestions { get => forecUsingSuggestions ?? Base?.ForecUsingSuggestions ?? false; set => forecUsingSuggestions = value; }
        [DefaultValue(false)] public bool Constant { get => constant ?? Base?.Constant ?? false; set => constant = value; }
        protected internal JtValue(IJtNodeParent parent) : base(parent)
        {
        }

        protected internal JtValue(JObject obj, IJtNodeParent parent) : base(obj, parent)
        {
            ForecUsingSuggestions = (bool?)obj["forceSuggestions"] ?? false;
            Constant = (bool?)obj["constant"] ?? false;
        }

        protected internal JtValue(JtValueNodeSource source, JToken? @override, IJtNodeParent parent) : base(source, @override, parent)
        {
            if (@override is null)
                return;
            forecUsingSuggestions = (bool?)@override["forceSuggestions"];
            constant = (bool?)@override["constant"];
        }

        protected internal override void BuildCommonJson(StringBuilder sb)
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
