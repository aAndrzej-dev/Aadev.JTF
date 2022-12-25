using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtIntNodeSource : JtValueNodeSource
    {
        public override JtNodeType Type => JtNodeType.Int;

        public int Max { get; set; }
        public int Min { get; set; }
        public int Default { get; set; }
        public override IJtSuggestionCollectionSource Suggestions { get; }
        public JtIntNodeSource(ICustomSourceParent parent) : base(parent)
        {
            Max = int.MaxValue;
            Min = int.MinValue;
            Suggestions = JtSuggestionCollectionSource<int>.Create(this);
        }
        internal JtIntNodeSource(JtInt node) : base(node)
        {
            Max = node.Max;
            Min = node.Min;
            Default = node.Default;
            Suggestions = node.Suggestions.CreateSource(this);
        }
        internal JtIntNodeSource(ICustomSourceParent parent, JObject source) : base(parent, source)
        {
            Suggestions = JtSuggestionCollectionSource<int>.Create(this, source["suggestions"]);
            Min = (int)(source["min"] ?? int.MinValue);
            Max = (int)(source["max"] ?? int.MaxValue);
            Default = (int)(source["default"] ?? 0);
        }
        internal JtIntNodeSource(ICustomSourceParent parent, JtIntNodeSource @base, JObject? @override) : base(parent, @base, @override)
        {
            Min = (int)(@override?["minLength"] ?? @base.Min);
            Max = (int)(@override?["maxLength"] ?? @base.Max);
            Default = (int)(@override?["default"] ?? @base.Default);
            Suggestions = @base.Suggestions;
        }


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
        public override JtNode CreateInstance(IJtNodeParent parent, JToken? @override) => new JtInt(parent, this, @override);
        public override JtNodeSource CreateOverride(ICustomSourceParent parent, JObject? @override) => new JtIntNodeSource(parent, this, @override);
    }
}