using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtStringNode : JtValueNode
    {
        private int? maxLength;
        private int? minLength;
        private string? @default;


        public new JtStringNodeSource? Base => (JtStringNodeSource?)base.Base;
        public override JTokenType JsonType => JTokenType.String;
        public override JtNodeType Type => JtNodeType.String;


        [DefaultValue(0), RefreshProperties(RefreshProperties.All)] public int MinLength { get => minLength ?? Base?.MinLength ?? 0; set { minLength = value.Max(0); maxLength = maxLength.Max(value); } }
        [DefaultValue(-1), RefreshProperties(RefreshProperties.All)] public int MaxLength { get => maxLength ?? Base?.MaxLength ?? -1; set { maxLength = value.Max(-1); minLength = minLength.Min(value).Max(0); } }
        public string Default { get => @default ?? Base?.Default ?? string.Empty; set => @default = value; }
        public override IJtSuggestionCollection Suggestions { get; }


        public JtStringNode(IJtNodeParent parent) : base(parent)
        {
            Suggestions = JtSuggestionCollection<string>.Create();
            MinLength = 0;
            MaxLength = -1;
            Default = string.Empty;
        }
        internal JtStringNode(IJtNodeParent parent, JObject source) : base(parent, source)
        {
            MinLength = (int)(source["minLength"] ?? 0);
            MaxLength = (int)(source["maxLength"] ?? -1);
            Default = (string?)source["default"] ?? string.Empty;

            Suggestions = JtSuggestionCollection<string>.Create(this, source["suggestions"]);
        }
        internal JtStringNode(IJtNodeParent parent, JtStringNodeSource source, JToken? @override) : base(parent, source, @override)
        {
            Suggestions = source.Suggestions.CreateInstance();
            if (@override is null)
                return;
            minLength = (int?)@override["minLength"];
            maxLength = (int?)@override["maxLength"];
            @default = (string?)@override["default"];
        }

        internal override void BuildJson(StringBuilder sb)
        {
            BuildCommonJson(sb);

            if (MinLength != 0)
                sb.Append($", \"minLength\": {MinLength}");
            if (MaxLength != -1)
                sb.Append($", \"maxLength\": {MaxLength}");
            if (!string.IsNullOrEmpty(Default))
                sb.Append($", \"default\": \"{Default}\"");

            sb.Append('}');
        }
        public override string? GetDisplayString(JToken? value)
        {
            if (value is null or not JValue)
                return null;
            return value.ToString();
        }
        public override JToken CreateDefaultValue() => new JValue(Default);
        public override object GetDefaultValue() => Default;
        public override JtNodeSource CreateSource() => currentSource ??= new JtStringNodeSource(this);
    }
}