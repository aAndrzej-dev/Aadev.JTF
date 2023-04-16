using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtShortNode : JtValueNode
    {
        private const short minValue = short.MinValue;
        private const short maxValue = short.MaxValue;
        private IJtSuggestionCollection? suggestions;
        private short? @default;
        private short? min;
        private short? max;


        public new JtShortNodeSource? Base => (JtShortNodeSource?)base.Base;
        public override JTokenType JsonType => JTokenType.Integer;
        public override JtNodeType Type => JtNodeType.Short;


        [DefaultValue(minValue), RefreshProperties(RefreshProperties.All)] public short Min { get => min ?? Base?.Min ?? minValue; set { min = value; max = max.Max(min); @default = @default.Clamp(Min, Max); } }
        [DefaultValue(maxValue), RefreshProperties(RefreshProperties.All)] public short Max { get => max ?? Base?.Max ?? maxValue; set { max = value; min = min.Min(max); @default = @default.Clamp(Min, Max); } }
        [DefaultValue(0)] public short Default { get => @default ?? Base?.Default ?? 0; set => @default = value.Clamp(Min, Max); }
        public override IJtSuggestionCollection Suggestions => suggestions ??= JtSuggestionCollection<short>.Create();


        public JtShortNode(IJtNodeParent parent) : base(parent)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;
        }
        internal JtShortNode(IJtNodeParent parent, JObject source) : base(parent, source)
        {
            Min = (short)(source["min"] ?? minValue);
            Max = (short)(source["max"] ?? maxValue);
            Default = (short)(source["default"] ?? 0);

            suggestions = JtSuggestionCollection<short>.TryCreate(this, source["suggestions"]);
        }
        internal JtShortNode(IJtNodeParent parent, JtShortNodeSource source, JToken? @override) : base(parent, source, @override)
        {
            suggestions = source.TryGetSuggestions()?.CreateInstance();
            if (@override is null)
                return;
            min = (short?)@override["min"];
            max = (short?)@override["max"];
            @default = (short?)@override["default"];
        }
        public override string? GetDisplayString(JToken? value)
        {
            if (value is null or not JValue)
                return null;
            short? val = (short?)value;
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
        public override JtNodeSource CreateSource() => currentSource ??= new JtShortNodeSource(this);

        public override IJtSuggestionCollection? TryGetSuggestions() => suggestions;
    }

}