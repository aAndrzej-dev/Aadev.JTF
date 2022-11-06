using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtFloatNodeSource : JtValueNodeSource
    {
        public float Max { get; set; }
        public float Min { get; set; }
        public float Default { get; set; }
        public override IJtSuggestionCollectionSource Suggestions { get; }
        public override JtNodeType Type => JtNodeType.Float;

        internal JtFloatNodeSource(JtFloat node, ICustomSourceProvider sourceProvider) : base(node, sourceProvider)
        {
            Max = node.Max;
            Min = node.Min;
            Default = node.Default;
            Suggestions = node.Suggestions.CreateSource(this);
        }

        internal JtFloatNodeSource(ICustomSourceParent parent, JObject source, ICustomSourceProvider sourceProvider) : base(parent, source, sourceProvider)
        {
            Suggestions = JtSuggestionCollectionSource<float>.Create(this, source["suggestions"], sourceProvider);
            Min = (float)(source["min"] ?? float.MinValue);
            Max = (float)(source["max"] ?? float.MaxValue);
            Default = (float)(source["default"] ?? 0);
        }


        internal JtFloatNodeSource(ICustomSourceParent parent, JtFloatNodeSource @base, JObject? @override) : base(parent, @base, @override)
        {
            Min = (float)(@override?["minLength"] ?? @base.Min);
            Max = (float)(@override?["maxLength"] ?? @base.Max);
            Default = (float)(@override?["default"] ?? @base.Default);
            Suggestions = @base.Suggestions;
        }
        public override JtNode CreateInstance(JToken? @override, IJtNodeParent parent) => new JtFloat(this, @override, parent);
        internal override void BuildJsonDeclaration(StringBuilder sb)
        {
            BuildCommonJson(sb);
            if (Max != float.MaxValue)
                sb.Append( $", \"max\": {Max}");
            if (Min != float.MinValue)
                sb.Append( $", \"min\": {Min}");
            if (Default != 0)
                sb.Append( $", \"default\": {Default}");
            if (Suggestions.IsSaveable)
            {
                sb.Append(", \"suggestions\": ");
                Suggestions.BuildJson(sb);
            }
            sb.Append('}');
        }
        internal override JtNodeSource CreateOverride(ICustomSourceParent parent, JObject? item) => new JtFloatNodeSource(parent, this, item);
    }
}