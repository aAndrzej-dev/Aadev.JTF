using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtInt : JtToken
    {
        private const int minValue = int.MinValue;
        private const int maxValue = int.MaxValue;
        private int @default;
        private int min;
        private int max;
        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.Integer;
        /// <inheritdoc/>
        public override JtTokenType Type => JtTokenType.Int;


        [DefaultValue(minValue)] public int Min { get => min; set { min = value.Min(Max); @default = value.Clamp(Min, Max); } }
        [DefaultValue(maxValue)] public int Max { get => max; set { max = value.Max(Min); @default = value.Clamp(Min, Max); } }
        [DefaultValue(0)] public int Default { get => @default; set => @default = value.Clamp(Min, Max); }
        public JtInt(JTemplate template) : base(template)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;
        }
        internal JtInt(JObject obj, JTemplate template) : base(obj, template)
        {
            Min = (int)(obj["min"] ?? minValue);
            Max = (int)(obj["max"] ?? maxValue);
            Default = (int)(obj["default"] ?? 0);
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