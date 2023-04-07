using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtIntNode : JtValueNode
    {
        private const int minValue = int.MinValue;
        private const int maxValue = int.MaxValue;
        private int? @default;
        private int? min;
        private int? max;

        public new JtIntNodeSource? Base => (JtIntNodeSource?)base.Base;
        public override JTokenType JsonType => JTokenType.Integer;
        public override JtNodeType Type => JtNodeType.Int;





        [DefaultValue(minValue), RefreshProperties(RefreshProperties.All)] public int Min { get => min ?? Base?.Min ?? minValue; set { min = value; max = max.Max(min); @default = @default.Clamp(Min, Max); } }
        [DefaultValue(maxValue), RefreshProperties(RefreshProperties.All)] public int Max { get => max ?? Base?.Max ?? maxValue; set { max = value; min = min.Min(max); @default = @default.Clamp(Min, Max); } }
        [DefaultValue(0)] public int Default { get => @default ?? Base?.Default ?? 0; set => @default = value.Clamp(Min, Max); }
        public override IJtSuggestionCollection Suggestions { get; }



        public JtIntNode(IJtNodeParent parent) : base(parent)
        {
            Min = minValue;
            Max = maxValue;
            Default = 0;
            Suggestions = JtSuggestionCollection<int>.Create();
        }
        internal JtIntNode(IJtNodeParent parent, JObject source) : base(parent, source)
        {
            Min = (int)(source["min"] ?? minValue);
            Max = (int)(source["max"] ?? maxValue);
            Default = (int)(source["default"] ?? 0);

            Suggestions = JtSuggestionCollection<int>.Create(this, source["suggestions"]);
        }

        internal JtIntNode(IJtNodeParent parent, JtIntNodeSource source, JToken? @override) : base(parent, source, @override)
        {
            Suggestions = source.Suggestions.CreateInstance();
            if (@override is null)
                return;
            min = (int?)@override["min"];
            max = (int?)@override["max"];
            @default = (int?)@override["default"];
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
            int? val = (int?)value;
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
        public override JtNodeSource CreateSource() => currentSource ??= new JtIntNodeSource(this);
    }
}