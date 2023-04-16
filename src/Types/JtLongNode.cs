using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtLongNode : JtValueNode
    {
        private const long minValue = long.MinValue;
        private const long maxValue = long.MaxValue;
        private IJtSuggestionCollection? suggestions;
        private long? @default;
        private long? min;
        private long? max;

        public new JtLongNodeSource? Base => (JtLongNodeSource?)base.Base;
        public override JTokenType JsonType => JTokenType.Integer;
        public override JtNodeType Type => JtNodeType.Long;


        [DefaultValue(minValue), RefreshProperties(RefreshProperties.All)] public long Min { get => min ?? Base?.Min ?? minValue; set { min = value; max = max.Max(min); @default = @default.Clamp(Min, Max); } }
        [DefaultValue(maxValue), RefreshProperties(RefreshProperties.All)] public long Max { get => max ?? Base?.Max ?? maxValue; set { max = value; min = min.Min(max); @default = @default.Clamp(Min, Max); } }
        [DefaultValue(0)] public long Default { get => @default ?? Base?.Default ?? 0; set => @default = value.Clamp(Min, Max); }
        public override IJtSuggestionCollection Suggestions => suggestions ??= JtSuggestionCollection<long>.Create();

        public JtLongNode(IJtNodeParent parent) : base(parent)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;
        }
        internal JtLongNode(IJtNodeParent parent, JObject source) : base(parent, source)
        {
            Min = (long)(source["min"] ?? minValue);
            Max = (long)(source["max"] ?? maxValue);
            Default = (long)(source["default"] ?? 0);

            suggestions = JtSuggestionCollection<long>.TryCreate(this, source["suggestions"]);
        }
        internal JtLongNode(IJtNodeParent parent, JtLongNodeSource source, JToken? @override) : base(parent, source, @override)
        {
            suggestions = source.TryGetSuggestions()?.CreateInstance();
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
        public override string? GetDisplayString(JToken? value)
        {
            if (value is null or not JValue)
                return null;
            long? val = (long?)value;
            if (val is null)
                return null;
            if (val == Default)
            {
                return $"Default ({val})";
            }
            if (val == Max)
            {
                return $"Max ({val})";
            }
            if (val == Min)
            {
                return $"Min ({val})";
            }
            return val.ToString();
        }
        public override JToken CreateDefaultValue() => new JValue(Default);
        public override object GetDefaultValue() => Default;
        public override JtNodeSource CreateSource() => currentSource ??= new JtLongNodeSource(this);

        public override IJtSuggestionCollection? TryGetSuggestions() => suggestions;
    }
}