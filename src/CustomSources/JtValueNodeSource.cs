using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public abstract class JtValueNodeSource : JtNodeSource
    {
        public abstract IJtSuggestionCollectionSource Suggestions { get; }
        [DefaultValue(false)] public bool ForceUsingSuggestions { get; set; }
        [DefaultValue(false)] public bool Constant { get; set; }

        private protected JtValueNodeSource(IJtNodeSourceParent parent) : base(parent) { }
        private protected JtValueNodeSource(JtValueNode node) : base(node)
        {
            ForceUsingSuggestions = node.ForceUsingSuggestions;
            Constant = node.Constant;
        }
        private protected JtValueNodeSource(IJtNodeSourceParent parent, JObject source) : base(parent, source)
        {
            ForceUsingSuggestions = (bool?)source["forceSuggestions"] ?? false;
            Constant = (bool?)source["constant"] ?? false;
        }
        private protected JtValueNodeSource(IJtNodeSourceParent parent, JtValueNodeSource @base, JObject? @override) : base(parent, @base, @override)
        {
            ForceUsingSuggestions = (bool)(@override?["forceSuggestions"] ?? @base.ForceUsingSuggestions);
            Constant = (bool)(@override?["constant"] ?? @base.Constant);
        }


        private protected override void BuildCommonJson(StringBuilder sb)
        {
            base.BuildCommonJson(sb);
            if (ForceUsingSuggestions)
                sb.Append($", \"forceSuggestions\": true");
            if (Constant)
                sb.Append($", \"constant\": true");
        }
    }
}
