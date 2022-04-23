using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtFloat : JtToken
    {
        private const float minValue = float.MinValue;
        private const float maxValue = float.MaxValue;
        private float @default;
        private float min;
        private float max;
        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.Float;
        /// <inheritdoc/>
        public override JtTokenType Type => JtTokenType.Float;


        [DefaultValue(minValue)] public float Min { get => min; set { min = value.Min(Max); @default = value.Clamp(Min, Max); } }
        [DefaultValue(maxValue)] public float Max { get => max; set { max = value.Max(Min); @default = value.Clamp(Min, Max); } }
        [DefaultValue(0)] public float Default { get => @default; set => @default = value.Clamp(Min, Max); }
        public JtFloat(JTemplate template) : base(template)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;
        }
        internal JtFloat(JObject obj, JTemplate template) : base(obj, template)
        {
            Min = (float)(obj["min"] ?? minValue);
            Max = (float)(obj["max"] ?? maxValue);
            Default = (float)(obj["default"] ?? 0);
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
    }
}
