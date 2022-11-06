using Aadev.JTF.CustomSources;
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
        private long? @default;
        private long? min;
        private long? max;
        public new JtLongNodeSource? Base => (JtLongNodeSource?)base.Base;

        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.Integer;
        /// <inheritdoc/>
        public override JtNodeType Type => JtNodeType.Long;


        [DefaultValue(minValue), RefreshProperties(RefreshProperties.All)] public long Min { get => min ?? Base?.Min ?? minValue; set { min = value; max = max.Max(min); @default = @default.Clamp(Min, Max); } }
        [DefaultValue(maxValue), RefreshProperties(RefreshProperties.All)] public long Max { get => max ?? Base?.Max ?? maxValue; set { max = value; min = min.Min(max); @default = @default.Clamp(Min, Max); } }
        [DefaultValue(0)] public long Default { get => @default ?? Base?.Default ?? 0; set => @default = value.Clamp(Min, Max); }

        public override IJtSuggestionCollection Suggestions { get; }

        public override Type ValueType => typeof(long);
        public JtLong(IJtNodeParent parent) : base(parent)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;

            Suggestions = JtSuggestionCollection<long>.Create();
        }
        internal JtLong(JObject obj, IJtNodeParent parent) : base(obj,parent)
        {
            Min = (long)(obj["min"] ?? minValue);
            Max = (long)(obj["max"] ?? maxValue);
            Default = (long)(obj["default"] ?? 0);

            Suggestions = JtSuggestionCollection<long>.Create(obj["suggestions"], this);
        }
        internal JtLong(JtLongNodeSource source, JToken? @override, IJtNodeParent parent) : base(source, @override, parent)
        {
            Suggestions = source.Suggestions.CreateInstance();
            if (@override is null)
                return;
            min = (long?)@override["min"];
            max = (long?)@override["max"];
            @default = (long?)@override["default"];
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
        public override JtNodeSource CreateSource() => currentSource ??= new JtLongNodeSource(this, this);
    }
}