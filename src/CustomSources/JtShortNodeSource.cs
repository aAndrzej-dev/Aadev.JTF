using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtShortNodeSource : JtValueNodeSource
    {
        public override JtNodeType Type => JtNodeType.Short;

        public short Max { get; set; }
        public short Min { get; set; }
        public short Default { get; set; }
        public override IJtSuggestionCollectionSource Suggestions { get; }


        public JtShortNodeSource(ICustomSourceParent parent) : base(parent)
        {
            Max = short.MaxValue;
            Min = short.MinValue;
            Suggestions = JtSuggestionCollectionSource<short>.Create(this);
        }
        internal JtShortNodeSource(JtShort node) : base(node)
        {
            Max = node.Max;
            Min = node.Min;
            Default = node.Default;
            Suggestions = node.Suggestions.CreateSource(this);
        }
        internal JtShortNodeSource(ICustomSourceParent parent, JtShortNodeSource @base, JObject? @override) : base(parent, @base, @override)
        {
            Min = (short)(@override?["minLength"] ?? @base.Min);
            Max = (short)(@override?["maxLength"] ?? @base.Max);
            Default = (short)(@override?["default"] ?? @base.Default);
            Suggestions = @base.Suggestions;
        }
        internal JtShortNodeSource(ICustomSourceParent parent, JObject source) : base(parent, source)
        {
            Suggestions = JtSuggestionCollectionSource<short>.Create(this, source["suggestions"]);
            Min = (short)(source["min"] ?? short.MinValue);
            Max = (short)(source["max"] ?? short.MaxValue);
            Default = (short)(source["default"] ?? 0);
        }


        internal override void BuildJsonDeclaration(StringBuilder sb)
        {
            BuildCommonJson(sb);
            if (Max != short.MaxValue)
                sb.Append($", \"max\": {Max}");
            if (Min != short.MinValue)
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
        public override JtNode CreateInstance(IJtNodeParent parent, JToken? @override) => new JtShort(parent, this, @override);
        public override JtNodeSource CreateOverride(ICustomSourceParent parent, JObject? @override) => new JtShortNodeSource(parent, this, @override);
    }
}