using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace Aadev.JTF.Types
{
    public abstract class JtValue : JtNode
    {
        public abstract Type ValueType { get; }
        public abstract IJtSuggestionCollection Suggestions { get; }
        public bool ForecUsingSuggestions { get; set; }
        protected internal JtValue(JTemplate template, IIdentifiersManager identifiersManager) : base(template, identifiersManager)
        {
        }

        protected internal JtValue(JObject obj, JTemplate template, IIdentifiersManager identifiersManager) : base(obj, template, identifiersManager)
        {
            ForecUsingSuggestions = (bool?)obj["forceSuggestions"] ?? false;
        }


        protected internal override void BuildCommonJson(StringBuilder sb)
        {
            base.BuildCommonJson(sb);
            if (Suggestions.Count > 0 || Suggestions.CustomSourceId?.StartsWith('$') is true)
            {
                sb.Append($", \"suggestions\": ");
                Suggestions.BuildJson(sb);

                if (ForecUsingSuggestions)
                    sb.Append(", \"forceSuggestions\": true");
            }
        }

        public abstract object GetDefault();
    }
}
