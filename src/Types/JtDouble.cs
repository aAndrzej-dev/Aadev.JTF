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
        private double @default;
        private double min;
        private double max;

        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.Float;
        /// <inheritdoc/>
        public override JtNodeType Type => JtNodeType.Double;


        [DefaultValue(minValue), RefreshProperties(RefreshProperties.All)] public double Min { get => min; set { min = value; max = max.Max(min); @default = @default.Clamp(min, max); } }
        [DefaultValue(maxValue), RefreshProperties(RefreshProperties.All)] public double Max { get => max; set { max = value; min = min.Min(max); @default = @default.Clamp(min, max); } }
        [DefaultValue(0)] public double Default { get => @default; set => @default = value.Clamp(min, max); }

        public override IJtSuggestionCollection Suggestions { get; }

        public override Type ValueType => typeof(double);

        public JtDouble(JTemplate template, IIdentifiersManager identifiersManager) : base(template, identifiersManager)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;
        }
        internal JtDouble(JObject obj, JTemplate template, IIdentifiersManager identifiersManager) : base(obj, template, identifiersManager)
        {
            Min = (double)(obj["min"] ?? minValue);
            Max = (double)(obj["max"] ?? maxValue);
            Default = (double)(obj["default"] ?? 0);


            Suggestions = new JtSuggestionCollection<double>(this, obj["suggestions"]);
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
            if (Suggestions.Count > 0)
            {
                sb.Append($", \"suggestions\": ");
                Suggestions.BuildJson(sb);

                if (ForecUsingSuggestions)
                    sb.Append(", \"forceSuggestions\": true");
            }
            sb.Append('}');
        }


        /// <inheritdoc/>
        public override JToken CreateDefaultValue() => new JValue(Default);

        public override object GetDefault() => Default;
    }
}