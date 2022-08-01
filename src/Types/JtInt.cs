﻿
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtInt : JtValue
    {

        private const int minValue = int.MinValue;
        private const int maxValue = int.MaxValue;
        private int @default;
        private int min;
        private int max;


        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.Integer;
        /// <inheritdoc/>
        public override JtNodeType Type => JtNodeType.Int;


        [DefaultValue(minValue), RefreshProperties(RefreshProperties.All)] public int Min { get => min; set { min = value; max = max.Max(min); @default = @default.Clamp(min, max); } }
        [DefaultValue(maxValue), RefreshProperties(RefreshProperties.All)] public int Max { get => max; set { max = value; min = min.Min(max); @default = @default.Clamp(min, max); } }
        [DefaultValue(0)] public int Default { get => @default; set => @default = value.Clamp(min, max); }

        public override IJtSuggestionCollection Suggestions { get; }

        public override Type ValueType => typeof(int);


        public JtInt(JTemplate template, IIdentifiersManager identifiersManager) : base(template, identifiersManager)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;
        }
        internal JtInt(JObject obj, JTemplate template, IIdentifiersManager identifiersManager) : base(obj, template, identifiersManager)
        {
            Min = (int)(obj["min"] ?? minValue);
            Max = (int)(obj["max"] ?? maxValue);
            Default = (int)(obj["default"] ?? 0);


            Suggestions = new JtSuggestionCollection<int>(this, obj["suggestions"]);
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