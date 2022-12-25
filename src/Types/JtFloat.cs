using Aadev.JTF.CustomSources;
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
        private float? @default;
        private float? min;
        private float? max;


        public new JtFloatNodeSource? Base => (JtFloatNodeSource?)base.Base;
        public override JTokenType JsonType => JTokenType.Float;
        public override JtNodeType Type => JtNodeType.Float;



        [DefaultValue(minValue), RefreshProperties(RefreshProperties.All)] public float Min { get => min ?? Base?.Min ?? minValue; set { min = value; max = max.Max(min); @default = @default.Clamp(Min, Max); } }
        [DefaultValue(maxValue), RefreshProperties(RefreshProperties.All)] public float Max { get => max ?? Base?.Max ?? maxValue; set { max = value; min = min.Min(max); @default = @default.Clamp(Min, Max); } }
        [DefaultValue(0)] public float Default { get => @default ?? Base?.Default ?? 0; set => @default = value.Clamp(Min, Max); }
        public override IJtSuggestionCollection Suggestions { get; }



        public JtFloat(IJtNodeParent parent) : base(parent)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;
            Suggestions = JtSuggestionCollection<float>.Create();
        }
        internal JtFloat(IJtNodeParent parent, JObject source) : base(parent, source)
        {
            Min = (float)(source["min"] ?? minValue);
            Max = (float)(source["max"] ?? maxValue);
            Default = (float)(source["default"] ?? 0);

            Suggestions = JtSuggestionCollection<float>.Create(this, source["suggestions"]);
        }
        internal JtFloat(IJtNodeParent parent, JtFloatNodeSource source, JToken? @override) : base(parent, source, @override)
        {
            Suggestions = source.Suggestions.CreateInstance();
            if (@override is null)
                return;
            min = (float?)@override["min"];
            max = (float?)@override["max"];
            @default = (float?)@override["default"];
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
        public override JtNodeSource CreateSource() => currentSource ??= new JtFloatNodeSource(this);
    }
}