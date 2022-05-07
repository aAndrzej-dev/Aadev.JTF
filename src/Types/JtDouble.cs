using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtDouble : JtNode
    {
        private const double minValue = double.MinValue;
        private const double maxValue = double.MaxValue;
        private double @default;
        private double min;
        private double max;

        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.Float;
        /// <inheritdoc/>
        public override JtNodeType Type => JtNodeType.Double;


        [DefaultValue(minValue)] public double Min { get => min; set { min = value.Min(Max); @default = value.Clamp(Min, Max); } }
        [DefaultValue(maxValue)] public double Max { get => max; set { max = value.Max(Min); @default = value.Clamp(Min, Max); } }
        [DefaultValue(0)] public double Default { get => @default; set => @default = value.Clamp(Min, Max); }
        public JtDouble(JTemplate template) : base(template)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;
        }
        internal JtDouble(JObject obj, JTemplate template) : base(obj, template)
        {
            Min = (double)(obj["min"] ?? minValue);
            Max = (double)(obj["max"] ?? maxValue);
            Default = (double)(obj["default"] ?? 0);
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