using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtLongNodeSource : JtValueNodeSource
    {
        public override JtNodeType Type => JtNodeType.Long;

        public long Max { get; set; }
        public long Min { get; set; }
        public long Default { get; set; }
        public override IJtSuggestionCollectionSource Suggestions { get; }

        public JtLongNodeSource(ICustomSourceParent parent) : base(parent)
        {
            Max = long.MaxValue;
            Min = long.MinValue;
            Suggestions = JtSuggestionCollectionSource<long>.Create(this);
        }
        internal JtLongNodeSource(JtLong node) : base(node)
        {
            Max = node.Max;
            Min = node.Min;
            Default = node.Default;
            Suggestions = node.Suggestions.CreateSource(this);
        }
        internal JtLongNodeSource(ICustomSourceParent parent, JObject source) : base(parent, source)
        {
            Suggestions = JtSuggestionCollectionSource<long>.Create(this, source["suggestions"]);
            Min = (long)(source["min"] ?? long.MinValue);
            Max = (long)(source["max"] ?? long.MaxValue);
            Default = (long)(source["default"] ?? 0);
        }
        internal JtLongNodeSource(ICustomSourceParent parent, JtLongNodeSource @base, JObject? @override) : base(parent, @base, @override)
        {
            Min = (long)(@override?["minLength"] ?? @base.Min);
            Max = (long)(@override?["maxLength"] ?? @base.Max);
            Default = (long)(@override?["default"] ?? @base.Default);
            Suggestions = @base.Suggestions;
        }





        internal override void BuildJsonDeclaration(StringBuilder sb)
        {
            BuildCommonJson(sb);
            if (Max != long.MaxValue)
                sb.Append($", \"max\": {Max}");
            if (Min != long.MinValue)
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
        public override JtNode CreateInstance(IJtNodeParent parent, JToken? @override) => new JtLong(parent, this, @override);
        public override JtNodeSource CreateOverride(ICustomSourceParent parent, JObject? @override) => new JtLongNodeSource(parent, this, @override);
    }
}