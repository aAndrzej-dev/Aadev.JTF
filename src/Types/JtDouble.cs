using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtDouble : JtValue
    {
        private const double minValue = double.MinValue;
        private const double maxValue = double.MaxValue;
        private double? @default;
        private double? min;
        private double? max;

        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.Float;
        /// <inheritdoc/>
        public override JtNodeType Type => JtNodeType.Double;

        public new JtDoubleNodeSource? Base => (JtDoubleNodeSource?)base.Base;

        [DefaultValue(minValue), RefreshProperties(RefreshProperties.All)] public double Min { get => min ?? Base?.Min ?? minValue; set { min = value; max = max.Max(min); @default = @default.Clamp(Min, Max); } }
        [DefaultValue(maxValue), RefreshProperties(RefreshProperties.All)] public double Max { get => max ?? Base?.Max ?? maxValue; set { max = value; min = min.Min(max); @default = @default.Clamp(Min, Max); } }
        [DefaultValue(0)] public double Default { get => @default ?? Base?.Default ?? 0; set => @default = value.Clamp(Min, Max); }

        public override IJtSuggestionCollection Suggestions { get; }
        public override Type ValueType => typeof(double);

        public JtDouble(IJtNodeParent parent) : base(parent)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;
            Suggestions = JtSuggestionCollection<double>.Create();
        }
        internal JtDouble(JObject obj, IJtNodeParent parent) : base(obj, parent)
        {
            Min = (double)(obj["min"] ?? minValue);
            Max = (double)(obj["max"] ?? maxValue);
            Default = (double)(obj["default"] ?? 0);



            Suggestions = JtSuggestionCollection<double>.Create(obj["suggestions"], this);
        }
        internal JtDouble(JtDoubleNodeSource source, JToken? @override, IJtNodeParent parent) : base(source, @override, parent)
        {
            Suggestions = source.Suggestions.CreateInstance();
            if (@override is null)
                return;
            min = (double?)@override["min"];
            max = (double?)@override["max"];
            @default = (double?)@override["default"];
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
        public override JtNodeSource CreateSource() => currentSource ??= new JtDoubleNodeSource(this, this);
    }
}