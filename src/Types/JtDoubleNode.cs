using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtDoubleNode : JtValueNode
    {
        private const double minValue = double.MinValue;
        private const double maxValue = double.MaxValue;
        private double? @default;
        private double? min;
        private double? max;


        public new JtDoubleNodeSource? Base => (JtDoubleNodeSource?)base.Base;
        public override JTokenType JsonType => JTokenType.Float;
        public override JtNodeType Type => JtNodeType.Double;



        [DefaultValue(minValue), RefreshProperties(RefreshProperties.All)] public double Min { get => min ?? Base?.Min ?? minValue; set { min = value; max = max.Max(min); @default = @default.Clamp(Min, Max); } }
        [DefaultValue(maxValue), RefreshProperties(RefreshProperties.All)] public double Max { get => max ?? Base?.Max ?? maxValue; set { max = value; min = min.Min(max); @default = @default.Clamp(Min, Max); } }
        [DefaultValue(0)] public double Default { get => @default ?? Base?.Default ?? 0; set => @default = value.Clamp(Min, Max); }
        public override IJtSuggestionCollection Suggestions { get; }


        public JtDoubleNode(IJtNodeParent parent) : base(parent)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;
            Suggestions = JtSuggestionCollection<double>.Create();
        }
        internal JtDoubleNode(IJtNodeParent parent, JObject source) : base(parent, source)
        {
            Min = (double)(source["min"] ?? minValue);
            Max = (double)(source["max"] ?? maxValue);
            Default = (double)(source["default"] ?? 0);



            Suggestions = JtSuggestionCollection<double>.Create(this, source["suggestions"]);
        }
        internal JtDoubleNode(IJtNodeParent parent, JtDoubleNodeSource source, JToken? @override) : base(parent, source, @override)
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
        public override string? GetDisplayString(JToken? value)
        {
            if (value is null or not JValue)
                return null;
            double? val = (double?)value;
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
        public override JtNodeSource CreateSource() => currentSource ??= new JtDoubleNodeSource(this);
    }
}