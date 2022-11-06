using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public abstract class JtValueNodeSource : JtNodeSource
    {
        protected internal JtValueNodeSource(ICustomSourceParent parent, JtValueNodeSource @base, JObject? @override) : base(parent, @base, @override)
        {
            ForecUsingSuggestions = (bool)(@override?["forceSuggestions"] ?? @base.ForecUsingSuggestions);
            Constant = (bool)(@override?["constant"] ?? @base.Constant);
        }

        protected internal JtValueNodeSource(JtValue node, ICustomSourceProvider sourceProvider) : base(node, sourceProvider)
        {
            ForecUsingSuggestions = node.ForecUsingSuggestions;
            Constant = node.Constant;
        }

        protected internal JtValueNodeSource(ICustomSourceParent parent, JObject source, ICustomSourceProvider sourceProvider) : base(parent, source, sourceProvider)
        {
            ForecUsingSuggestions = (bool?)source["forceSuggestions"] ?? false;
            Constant = (bool?)source["constant"] ?? false;
        }


        public bool ForecUsingSuggestions { get; set; }
        public bool Constant { get; set; }
        public abstract IJtSuggestionCollectionSource Suggestions { get; }
        protected override void BuildCommonJson(StringBuilder sb)
        {
            base.BuildCommonJson(sb);
            if (ForecUsingSuggestions)
                sb.Append($", \"forceSuggestions\": true");
            if (Constant)
                sb.Append($", \"constant\": true");
        }
    }
}
