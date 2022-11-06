using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtShortNodeSource : JtValueNodeSource
    {
        internal JtShortNodeSource(JtShort node, ICustomSourceProvider sourceProvider) : base(node, sourceProvider)
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

        internal JtShortNodeSource(ICustomSourceParent parent, JObject source, ICustomSourceProvider sourceProvider) : base(parent, source, sourceProvider)
        {
            Suggestions = JtSuggestionCollectionSource<short>.Create(this, source["suggestions"], sourceProvider);
            Min = (short)(source["min"] ?? short.MinValue);
            Max = (short)(source["max"] ?? short.MaxValue);
            Default = (short)(source["default"] ?? 0);
        }



        public short Max { get; internal set; }
        public short Min { get; internal set; }
        public short Default { get; internal set; }
        public override IJtSuggestionCollectionSource Suggestions { get; }

        public override JtNodeType Type => JtNodeType.Short;
        public override JtNode CreateInstance(JToken? @override, IJtNodeParent parent) => new JtShort(this, @override, parent);
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
        internal override JtNodeSource CreateOverride(ICustomSourceParent parent, JObject? item) => new JtShortNodeSource(parent, this, item);
    }
}