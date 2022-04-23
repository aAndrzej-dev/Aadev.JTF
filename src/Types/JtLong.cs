using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtLong : JtToken
    {
        private const long minValue = long.MinValue;
        private const long maxValue = long.MaxValue;
        private long @default;
        private long min;
        private long max;

        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.Integer;
        /// <inheritdoc/>
        public override JtTokenType Type => JtTokenType.Long;


        [DefaultValue(minValue)] public long Min { get => min; set { min = value.Min(Max); @default = value.Clamp(Min, Max); } }
        [DefaultValue(maxValue)] public long Max { get => max; set { max = value.Max(Min); @default = value.Clamp(Min, Max); } }
        [DefaultValue(0)] public long Default { get => @default; set => @default = value.Clamp(Min, Max); }
        public JtLong(JTemplate template) : base(template)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;
        }
        internal JtLong(JObject obj, JTemplate template) : base(obj, template)
        {
            Min = (long)(obj["min"] ?? minValue);
            Max = (long)(obj["max"] ?? maxValue);
            Default = (long)(obj["default"] ?? 0);
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