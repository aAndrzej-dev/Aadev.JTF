using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtDoubleNodeSource : JtValueNodeSource
    {
        private IJtSuggestionCollectionSource? suggestions;
        public override JtNodeType Type => JtNodeType.Double;

        public double Max { get; set; }
        public double Min { get; set; }
        public double Default { get; set; }
        public override IJtSuggestionCollectionSource Suggestions => suggestions ??= JtSuggestionCollectionSource<double>.Create(this);


        public override JTokenType JsonType => JTokenType.Float;
        public JtDoubleNodeSource(IJtNodeSourceParent parent) : base(parent)
        {
            Max = double.MaxValue;
            Min = double.MinValue;
        }
        internal JtDoubleNodeSource(JtDoubleNode node) : base(node)
        {
            Max = node.Max;
            Min = node.Min;
            Default = node.Default;
            suggestions = node.TryGetSuggestions()?.CreateSource(this);
        }
        internal JtDoubleNodeSource(IJtNodeSourceParent parent, JObject source) : base(parent, source)
        {
            Min = (double)(source["min"] ?? double.MinValue);
            Max = (double)(source["max"] ?? double.MaxValue);
            Default = (double)(source["default"] ?? 0);
            suggestions = JtSuggestionCollectionSource<double>.TryCreate(this, source["suggestions"]);
        }
        internal JtDoubleNodeSource(IJtNodeSourceParent parent, JtDoubleNodeSource @base, JObject? @override) : base(parent, @base, @override)
        {
            Min = (double)(@override?["minLength"] ?? @base.Min);
            Max = (double)(@override?["maxLength"] ?? @base.Max);
            Default = (double)(@override?["default"] ?? @base.Default);
            suggestions = @base.Suggestions;
        }


        internal override void BuildJsonDeclaration(StringBuilder sb)
        {
            BuildCommonJson(sb);
            if (Max != double.MaxValue)
                sb.Append($", \"max\": {Max}");
            if (Min != double.MinValue)
                sb.Append($", \"min\": {Min}");
            if (Default != 0)
                sb.Append($", \"default\": {Default}");
            if (Suggestions.IsSavable)
            {
                sb.Append(", \"suggestions\": ");
                Suggestions.BuildJson(sb);
            }
            sb.Append('}');
        }
        public override JtNode CreateInstance(IJtNodeParent parent, JToken? @override) => new JtDoubleNode(parent, this, @override);
        public override JtNodeSource CreateOverride(IJtNodeSourceParent parent, JObject? @override) => new JtDoubleNodeSource(parent, this, @override);
        public override JToken CreateDefaultValue() => new JValue(Default);
        internal override IJtSuggestionCollectionSource? TryGetSuggestions() => suggestions;
    }
}