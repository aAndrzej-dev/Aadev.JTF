using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtByte : JtValue
    {
        private const byte minValue = byte.MinValue;
        private const byte maxValue = byte.MaxValue;
        private byte? @default;
        private byte? min;
        private byte? max;

        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.Integer;
        /// <inheritdoc/>
        public override JtNodeType Type => JtNodeType.Byte;

        public new JtByteNodeSource? Base => (JtByteNodeSource?)base.Base;

        [DefaultValue(minValue), RefreshProperties(RefreshProperties.All)] public byte Min { get => min ?? Base?.Min ?? minValue; set { min = value; max = max.Max(min); @default = @default.Clamp(Min, Max); } }
        [DefaultValue(maxValue), RefreshProperties(RefreshProperties.All)] public byte Max { get => max ?? Base?.Max ?? maxValue; set { max = value; min = min.Min(max); @default = @default.Clamp(Min, Max); } }
        [DefaultValue(0)] public byte Default { get => @default ?? Base?.Default ?? 0; set => @default = value.Clamp(Min, Max); }

        public override IJtSuggestionCollection Suggestions { get; }

        public override Type ValueType => typeof(byte);

        public JtByte(IJtNodeParent parent) : base(parent)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;
            Suggestions = JtSuggestionCollection<byte>.Create();
        }
        internal JtByte(JObject obj, IJtNodeParent parent) : base(obj, parent)
        {
            Min = (byte)(obj["min"] ?? minValue);
            Max = (byte)(obj["max"] ?? maxValue);
            Default = (byte)(obj["default"] ?? 0);


            Suggestions = JtSuggestionCollection<byte>.Create(obj["suggestions"], this);
        }

        internal JtByte(JtByteNodeSource source, JToken? @override, IJtNodeParent parent) : base(source, @override, parent)
        {
            Suggestions = source.Suggestions.CreateInstance();
            if (@override is null)
                return;
            min = (byte?)@override["min"];
            max = (byte?)@override["max"];
            @default = (byte?)@override["default"];

        }

        internal override void BuildJson(StringBuilder sb)
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

        public override object GetDefaultValue() => Default;
        public override JtNodeSource CreateSource() => currentSource ??= new JtByteNodeSource(this, this);
    }
}