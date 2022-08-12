using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtFloat : JtValue
    {
        private const float minValue = float.MinValue;
        private const float maxValue = float.MaxValue;
        private float @default;
        private float min;
        private float max;
        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.Float;
        /// <inheritdoc/>
        public override JtNodeType Type => JtNodeType.Float;


        [DefaultValue(minValue), RefreshProperties(RefreshProperties.All)] public float Min { get => min; set { min = value; max = max.Max(min); @default = @default.Clamp(min, max); } }
        [DefaultValue(maxValue), RefreshProperties(RefreshProperties.All)] public float Max { get => max; set { max = value; min = min.Min(max); @default = @default.Clamp(min, max); } }
        [DefaultValue(0)] public float Default { get => @default; set => @default = value.Clamp(min, max); }
        public override IJtSuggestionCollection Suggestions { get; }

        public override Type ValueType => typeof(float);

        public JtFloat(JTemplate template, IIdentifiersManager identifiersManager) : base(template, identifiersManager)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;
            Suggestions = new JtSuggestionCollection<float>(this);
        }
        internal JtFloat(JObject obj, JTemplate template, IIdentifiersManager identifiersManager) : base(obj, template, identifiersManager)
        {
            Min = (float)(obj["min"] ?? minValue);
            Max = (float)(obj["max"] ?? maxValue);
            Default = (float)(obj["default"] ?? 0);


            Suggestions = new JtSuggestionCollection<float>(this, obj["suggestions"]);
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