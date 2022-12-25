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


        public new JtByteNodeSource? Base => (JtByteNodeSource?)base.Base;
        public override JTokenType JsonType => JTokenType.Integer;
        public override JtNodeType Type => JtNodeType.Byte;



        [DefaultValue(minValue), RefreshProperties(RefreshProperties.All)] public byte Min { get => min ?? Base?.Min ?? minValue; set { min = value; max = max.Max(min); @default = @default.Clamp(Min, Max); } }
        [DefaultValue(maxValue), RefreshProperties(RefreshProperties.All)] public byte Max { get => max ?? Base?.Max ?? maxValue; set { max = value; min = min.Min(max); @default = @default.Clamp(Min, Max); } }
        [DefaultValue(0)] public byte Default { get => @default ?? Base?.Default ?? 0; set => @default = value.Clamp(Min, Max); }

        public override IJtSuggestionCollection Suggestions { get; }


        public JtByte(IJtNodeParent parent) : base(parent)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;
            Suggestions = JtSuggestionCollection<byte>.Create();
        }
        internal JtByte(IJtNodeParent parent, JObject source) : base(parent, source)
        {
            Min = (byte)(source["min"] ?? minValue);
            Max = (byte)(source["max"] ?? maxValue);
            Default = (byte)(source["default"] ?? 0);


            Suggestions = JtSuggestionCollection<byte>.Create(this, source["suggestions"]);
        }
        internal JtByte(IJtNodeParent parent, JtByteNodeSource source, JToken? @override) : base(parent, source, @override)
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
        public override JToken CreateDefaultValue() => new JValue(Default);
        public override object GetDefaultValue() => Default;
        public override JtNodeSource CreateSource() => currentSource ??= new JtByteNodeSource(this);
    }
}