using Newtonsoft.Json.Linq;
using System;

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

        public abstract object GetDefault();
    }
}
