using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtLongNodeSource : JtValueNodeSource
    {
        internal JtLongNodeSource(JtLong node, ICustomSourceProvider sourceProvider) : base(node, sourceProvider)
        {
            Max = node.Max;
            Min = node.Min;
            Default = node.Default;
            Suggestions = node.Suggestions.CreateSource(this);
        }

        internal JtLongNodeSource(ICustomSourceParent parent, JtLongNodeSource @base, JObject? @override) : base(parent, @base, @override)
        {
            Min = (long)(@override?["minLength"] ?? @base.Min);
            Max = (long)(@override?["maxLength"] ?? @base.Max);
            Default = (long)(@override?["default"] ?? @base.Default);
            Suggestions = @base.Suggestions;
        }

        internal JtLongNodeSource(ICustomSourceParent parent, JObject source, ICustomSourceProvider sourceProvider) : base(parent, source, sourceProvider)
        {
            Suggestions = JtSuggestionCollectionSource<long>.Create(this, source["suggestions"], sourceProvider);
            Min = (long)(source["min"] ?? long.MinValue);
            Max = (long)(source["max"] ?? long.MaxValue);
            Default = (long)(source["default"] ?? 0);
        }



        public long Max { get; internal set; }
        public long Min { get; internal set; }
        public long Default { get; internal set; }
        public override IJtSuggestionCollectionSource Suggestions { get; }

        public override JtNodeType Type => JtNodeType.Long;

        public override JtNode CreateInstance(JToken? @override, IJtNodeParent parent) => new JtLong(this, @override, parent);
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
        internal override JtNodeSource CreateOverride(ICustomSourceParent parent, JObject? item) => new JtLongNodeSource(parent, this, item);
    }
}