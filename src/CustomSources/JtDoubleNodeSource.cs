using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtDoubleNodeSource : JtValueNodeSource
    {
        public override JtNodeType Type => JtNodeType.Double;

        public double Max { get; set; }
        public double Min { get; set; }
        public double Default { get; set; }
        public override IJtSuggestionCollectionSource Suggestions { get; }


        public JtDoubleNodeSource(ICustomSourceParent parent) : base(parent)
        {
            Max = double.MaxValue;
            Min = double.MinValue;
            Suggestions = JtSuggestionCollectionSource<double>.Create(this);
        }
        internal JtDoubleNodeSource(JtDouble node) : base(node)
        {
            Max = node.Max;
            Min = node.Min;
            Default = node.Default;
            Suggestions = node.Suggestions.CreateSource(this);
        }
        internal JtDoubleNodeSource(ICustomSourceParent parent, JObject source) : base(parent, source)
        {
            Suggestions = JtSuggestionCollectionSource<double>.Create(this, source["suggestions"]);
            Min = (double)(source["min"] ?? double.MinValue);
            Max = (double)(source["max"] ?? double.MaxValue);
            Default = (double)(source["default"] ?? 0);
        }
        internal JtDoubleNodeSource(ICustomSourceParent parent, JtDoubleNodeSource @base, JObject? @override) : base(parent, @base, @override)
        {
            Min = (double)(@override?["minLength"] ?? @base.Min);
            Max = (double)(@override?["maxLength"] ?? @base.Max);
            Default = (double)(@override?["default"] ?? @base.Default);
            Suggestions = @base.Suggestions;
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
            if (Suggestions.IsSaveable)
            {
                sb.Append(", \"suggestions\": ");
                Suggestions.BuildJson(sb);
            }
            sb.Append('}');
        }
        public override JtNode CreateInstance(IJtNodeParent parent, JToken? @override) => new JtDouble(parent, this, @override);
        public override JtNodeSource CreateOverride(ICustomSourceParent parent, JObject? @override) => new JtDoubleNodeSource(parent, this, @override);
    }
}