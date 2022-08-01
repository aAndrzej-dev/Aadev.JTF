using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtString : JtValue
    {
        private int maxLength;
        private int minLength;

        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.String;
        /// <inheritdoc/>
        public override JtNodeType Type => JtNodeType.String;

        [DefaultValue(0), RefreshProperties(RefreshProperties.All)] public int MinLength { get => minLength; set { minLength = value.Max(0); maxLength = maxLength.Max(value); } }
        [DefaultValue(-1), RefreshProperties(RefreshProperties.All)] public int MaxLength { get => maxLength; set { maxLength = value.Max(-1); minLength = minLength.Min(value).Max(0); } }
        public string Default { get; set; }
        public override Type ValueType => typeof(string);

        public override IJtSuggestionCollection Suggestions { get; }

        public JtString(JTemplate template, IIdentifiersManager identifiersManager) : base(template, identifiersManager)
        {
            Suggestions = new JtSuggestionCollection<string>(this);
            MinLength = 0;
            MaxLength = -1;
            Default = string.Empty;
        }
        internal JtString(JObject obj, JTemplate template, IIdentifiersManager identifiersManager) : base(obj, template, identifiersManager)
        {
            MinLength = (int)(obj["minLength"] ?? 0);
            MaxLength = (int)(obj["maxLength"] ?? -1);
            Default = (string?)obj["default"] ?? string.Empty;

            if (obj["suggestions"] != null)
            {
                Suggestions = new JtSuggestionCollection<string>(this, obj["suggestions"]);
            }
            else if (obj["values"] != null)
            {
                Suggestions = new JtSuggestionCollection<string>(this, obj["values"]);
            }
            else
                Suggestions = new JtSuggestionCollection<string>(this);
        }

        internal override void BulidJson(StringBuilder sb)
        {
            BuildCommonJson(sb);
            if (MinLength != 0)
                sb.Append($", \"minLength\": {MinLength}");
            if (MaxLength != -1)
                sb.Append($", \"maxLength\": {MaxLength}");
            if (!string.IsNullOrEmpty(Default))
                sb.Append($", \"default\": \"{Default}\"");
            if (Suggestions.Count > 0)
            {
                sb.Append($", \"suggestions\": ");
                Suggestions.BuildJson(sb);

                if (ForecUsingSuggestions)
                    sb.Append(", \"forceSuggestions\": true");
            }
            sb.Append('}');
        }


        /// <inheritdoc/>
        public override JToken CreateDefaultValue() => new JValue(Default);

        public override object GetDefault() => Default;
    }
}