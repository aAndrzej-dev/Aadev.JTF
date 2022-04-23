using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtByte : JtToken
    {
        private const byte minValue = byte.MinValue;
        private const byte maxValue = byte.MaxValue;
        private byte @default;
        private byte min;
        private byte max;

        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.Integer;
        /// <inheritdoc/>
        public override JtTokenType Type => JtTokenType.Byte;


        [DefaultValue(minValue)] public byte Min { get => min; set { min = value.Min(Max); @default = value.Clamp(Min, Max); } }
        [DefaultValue(maxValue)] public byte Max { get => max; set { max = value.Max(Min); @default = value.Clamp(Min, Max); } }
        [DefaultValue(0)] public byte Default { get => @default; set => @default = value.Clamp(Min, Max); }

        public JtByte(JTemplate template) : base(template)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;
        }
        internal JtByte(JObject obj, JTemplate template) : base(obj, template)
        {
            Min = (byte)(obj["min"] ?? minValue);
            Max = (byte)(obj["max"] ?? maxValue);
            Default = (byte)(obj["default"] ?? 0);
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
