using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtLong : JtValue
    {
        private const long minValue = long.MinValue;
        private const long maxValue = long.MaxValue;
        private long @default;
        private long min;
        private long max;

        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.Integer;
        /// <inheritdoc/>
        public override JtNodeType Type => JtNodeType.Long;


        [DefaultValue(minValue), RefreshProperties(RefreshProperties.All)] public long Min { get => min; set { min = value; max = max.Max(min); @default = @default.Clamp(min, max); } }
        [DefaultValue(maxValue), RefreshProperties(RefreshProperties.All)] public long Max { get => max; set { max = value; min = min.Min(max); @default = @default.Clamp(min, max); } }
        [DefaultValue(0)] public long Default { get => @default; set => @default = value.Clamp(min, max); }
        public override IJtSuggestionCollection Suggestions { get; }

        public override Type ValueType => typeof(long);
        public JtLong(JTemplate template, IIdentifiersManager identifiersManager) : base(template, identifiersManager)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;
        }
        internal JtLong(JObject obj, JTemplate template, IIdentifiersManager identifiersManager) : base(obj, template, identifiersManager)
        {
            Min = (long)(obj["min"] ?? minValue);
            Max = (long)(obj["max"] ?? maxValue);
            Default = (long)(obj["default"] ?? 0);


            Suggestions = new JtSuggestionCollection<long>(this, obj["suggestions"]);
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