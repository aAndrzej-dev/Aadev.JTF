using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtString : JtNode
    {
        private int maxLength;
        private int minLength;

        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.String;
        /// <inheritdoc/>
        public override JtNodeType Type => JtNodeType.String;

        [DefaultValue(0)] public int MinLength { get => minLength; set { minLength = value.Max(0); maxLength = maxLength.Max(value); } }
        [DefaultValue(-1)] public int MaxLength { get => maxLength; set { maxLength = value.Max(-1); minLength = minLength.Min(value).Max(0); } }
        public string Default { get; set; }
        public JtString(JTemplate template) : base(template)
        {
            MinLength = 0;
            MaxLength = int.MaxValue;
            Default = string.Empty;
        }
        internal JtString(JObject obj, JTemplate template) : base(obj, template)
        {
            MinLength = (int)(obj["minLength"] ?? 0);
            MaxLength = (int)(obj["maxLength"] ?? -1);
            Default = (string?)obj["default"] ?? string.Empty;
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

            sb.Append('}');
        }


        /// <inheritdoc/>
        public override JToken CreateDefaultValue() => new JValue(Default);
    }
}
