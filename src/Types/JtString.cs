using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtString : JtValue
    {
        private int? maxLength;
        private int? minLength;
        private string? @default;

        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.String;
        /// <inheritdoc/>
        public override JtNodeType Type => JtNodeType.String;
        public new JtStringNodeSource? Base => (JtStringNodeSource?)base.Base;

        [DefaultValue(0), RefreshProperties(RefreshProperties.All)] public int MinLength { get => minLength ?? Base?.MinLength ?? 0; set { minLength = value.Max(0); maxLength = maxLength.Max(value); } }
        [DefaultValue(-1), RefreshProperties(RefreshProperties.All)] public int MaxLength { get => maxLength ?? Base?.MaxLength ?? -1; set { maxLength = value.Max(-1); minLength = minLength.Min(value).Max(0); } }
        public string Default { get => @default ?? Base?.Default ?? string.Empty; set => @default = value; }
        public override Type ValueType => typeof(string);

        public override IJtSuggestionCollection Suggestions { get; }

        public JtString(IJtNodeParent parent) : base(parent)
        {
            Suggestions = JtSuggestionCollection<string>.Create();
            MinLength = 0;
            MaxLength = -1;
            Default = string.Empty;
        }
        internal JtString(JObject obj, IJtNodeParent parent) : base(obj, parent)
        {
            MinLength = (int)(obj["minLength"] ?? 0);
            MaxLength = (int)(obj["maxLength"] ?? -1);
            Default = (string?)obj["default"] ?? string.Empty;

            Suggestions = JtSuggestionCollection<string>.Create(obj["suggestions"], this);
        }
        internal JtString(JtStringNodeSource source, JToken? @override, IJtNodeParent parent) : base(source,@override, parent)
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


        /// <inheritdoc/>
        public override JToken CreateDefaultValue() => new JValue(Default);

        public override object GetDefaultValue() => Default;
        public override JtNodeSource CreateSource() => currentSource ??= new JtStringNodeSource(this, this);
    }
}