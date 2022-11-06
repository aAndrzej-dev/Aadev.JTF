using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtIntNodeSource : JtValueNodeSource
    {
        internal JtIntNodeSource(JtInt node, ICustomSourceProvider sourceProvider) : base(node, sourceProvider)
        {
            Max = node.Max;
            Min = node.Min;
            Default = node.Default;
            Suggestions = node.Suggestions.CreateSource(this);
        }

        internal JtIntNodeSource(ICustomSourceParent parent, JtIntNodeSource @base, JObject? @override) : base(parent, @base, @override)
        {
            Min = (int)(@override?["minLength"] ?? @base.Min);
            Max = (int)(@override?["maxLength"] ?? @base.Max);
            Default = (int)(@override?["default"] ?? @base.Default);
            Suggestions = @base.Suggestions;
        }

        internal JtIntNodeSource(ICustomSourceParent parent, JObject source, ICustomSourceProvider sourceProvider) : base(parent, source, sourceProvider)
        {
            Suggestions = JtSuggestionCollectionSource<int>.Create(this, source["suggestions"], sourceProvider);
            Min = (int)(source["min"] ?? int.MinValue);
            Max = (int)(source["max"] ?? int.MaxValue);
            Default = (int)(source["default"] ?? 0);
        }

        public int Max { get; internal set; }
        public int Min { get; internal set; }
        public int Default { get; internal set; }
        public override IJtSuggestionCollectionSource Suggestions { get; }

        public override JtNodeType Type => JtNodeType.Int;

        public override JtNode CreateInstance(JToken? @override, IJtNodeParent parent) => new JtInt(this, @override, parent);

        internal override void BuildJsonDeclaration(StringBuilder sb)
        {
            BuildCommonJson(sb);
            if (Max != int.MaxValue)
                sb.Append( $", \"max\": {Max}");
            if (Min != int.MinValue)
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
        internal override JtNodeSource CreateOverride(ICustomSourceParent parent, JObject? item) => new JtIntNodeSource(parent, this, item);
    }
}