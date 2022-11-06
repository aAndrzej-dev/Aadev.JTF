using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtDoubleNodeSource : JtValueNodeSource
    {
        internal JtDoubleNodeSource(JtDouble node, ICustomSourceProvider sourceProvider) : base(node, sourceProvider)
        {
            Max = node.Max;
            Min = node.Min;
            Default = node.Default;
            Suggestions = node.Suggestions.CreateSource(this);
        }

        internal JtDoubleNodeSource(ICustomSourceParent parent, JtDoubleNodeSource @base, JObject? @override) : base(parent, @base, @override)
        {
            Min = (double)(@override?["minLength"] ?? @base.Min);
            Max = (double)(@override?["maxLength"] ?? @base.Max);
            Default = (double)(@override?["default"] ?? @base.Default);
            Suggestions = @base.Suggestions;
        }

        internal JtDoubleNodeSource(ICustomSourceParent parent, JObject source, ICustomSourceProvider sourceProvider) : base(parent, source, sourceProvider)
        {
            Suggestions = JtSuggestionCollectionSource<double>.Create(this, source["suggestions"], sourceProvider);
            Min = (double)(source["min"] ?? double.MinValue);
            Max = (double)(source["max"] ?? double.MaxValue);
            Default = (double)(source["default"] ?? 0);
        }


        public double Max { get; internal set; }
        public double Min { get; internal set; }
        public double Default { get; internal set; }
        public override IJtSuggestionCollectionSource Suggestions { get; }

        public override JtNodeType Type => JtNodeType.Double;

        public override JtNode CreateInstance(JToken? @override, IJtNodeParent parent) => new JtDouble(this, @override, parent);
        internal override void BuildJsonDeclaration(StringBuilder sb)
        {
            BuildCommonJson(sb);
            if (Max != double.MaxValue)
                sb.Append( $", \"max\": {Max}");
            if (Min != double.MinValue)
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
        internal override JtNodeSource CreateOverride(ICustomSourceParent parent, JObject? item) => new JtDoubleNodeSource(parent, this, item);
    }
}