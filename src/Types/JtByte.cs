using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtByte : JtNode
    {
        private const byte minValue = byte.MinValue;
        private const byte maxValue = byte.MaxValue;
        private byte @default;
        private byte min;
        private byte max;

        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.Integer;
        /// <inheritdoc/>
        public override JtNodeType Type => JtNodeType.Byte;


        [DefaultValue(minValue), RefreshProperties(RefreshProperties.All)] public byte Min { get => min; set { min = value; max = max.Max(min); @default = @default.Clamp(min, max); } }
        [DefaultValue(maxValue), RefreshProperties(RefreshProperties.All)] public byte Max { get => max; set { max = value; min = min.Min(max); @default = @default.Clamp(min, max); } }
        [DefaultValue(0)] public byte Default { get => @default; set => @default = value.Clamp(min, max); }

        public JtByte(JTemplate template, IIdentifiersManager identifiersManager) : base(template, identifiersManager)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;
        }
        internal JtByte(JObject obj, JTemplate template, IIdentifiersManager identifiersManager) : base(obj, template, identifiersManager)
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