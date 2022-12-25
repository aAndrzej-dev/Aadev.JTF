using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public abstract class JtValueNodeSource : JtNodeSource
    {
        public abstract IJtSuggestionCollectionSource Suggestions { get; }
        public bool ForecUsingSuggestions { get; set; }
        public bool Constant { get; set; }

        public JtValueNodeSource(ICustomSourceParent parent) : base(parent) { }
        private protected JtValueNodeSource(JtValue node) : base(node)
        {
            ForecUsingSuggestions = node.ForecUsingSuggestions;
            Constant = node.Constant;
        }
        private protected JtValueNodeSource(ICustomSourceParent parent, JObject source) : base(parent, source)
        {
            ForecUsingSuggestions = (bool?)source["forceSuggestions"] ?? false;
            Constant = (bool?)source["constant"] ?? false;
        }
        private protected JtValueNodeSource(ICustomSourceParent parent, JtValueNodeSource @base, JObject? @override) : base(parent, @base, @override)
        {
            ForecUsingSuggestions = (bool)(@override?["forceSuggestions"] ?? @base.ForecUsingSuggestions);
            Constant = (bool)(@override?["constant"] ?? @base.Constant);
        }


        private protected override void BuildCommonJson(StringBuilder sb)
        {
            base.BuildCommonJson(sb);
            if (ForecUsingSuggestions)
                sb.Append($", \"forceSuggestions\": true");
            if (Constant)
                sb.Append($", \"constant\": true");
        }
    }
}
