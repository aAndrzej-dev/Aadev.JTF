using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtShort : JtValue
    {
        private const short minValue = short.MinValue;
        private const short maxValue = short.MaxValue;
        private short @default;
        private short min;
        private short max;


        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.Integer;
        /// <inheritdoc/>
        public override JtNodeType Type => JtNodeType.Short;


        [DefaultValue(minValue), RefreshProperties(RefreshProperties.All)] public short Min { get => min; set { min = value; max = max.Max(min); @default = @default.Clamp(min, max); } }
        [DefaultValue(maxValue), RefreshProperties(RefreshProperties.All)] public short Max { get => max; set { max = value; min = min.Min(max); @default = @default.Clamp(min, max); } }
        [DefaultValue(0)] public short Default { get => @default; set => @default = value.Clamp(min, max); }

        public override Type ValueType => typeof(short);

        public override IJtSuggestionCollection Suggestions { get; }

        public JtShort(JTemplate template, IIdentifiersManager identifiersManager) : base(template, identifiersManager)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;
            Suggestions = new JtSuggestionCollection<short>(this);
        }
        internal JtShort(JObject obj, JTemplate template, IIdentifiersManager identifiersManager) : base(obj, template, identifiersManager)
        {
            Min = (short)(obj["min"] ?? minValue);
            Max = (short)(obj["max"] ?? maxValue);
            Default = (short)(obj["default"] ?? 0);

            Suggestions = new JtSuggestionCollection<short>(this, obj["suggestions"]);
        }

        internal override void BulidJson(StringBuilder sb)
        {
            BuildCommonJson(sb);

            if (Min != minValue)
                sb.Append($", \"min\": {Min}");
            if (Max != maxValue)
                sb.Append($", \"max\": {Max}");
            if (Default != 0)
                sb.Append($", \"default\": {Default}");
            sb.Append('}');
        }


        /// <inheritdoc/>
        public override JToken CreateDefaultValue() => new JValue(Default);
        public override object GetDefault() => Default;
    }
}