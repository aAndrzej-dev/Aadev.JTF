using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtStringNodeSource : JtValueNodeSource
    {
        internal JtStringNodeSource(JtString node, ICustomSourceProvider sourceProvider) : base(node, sourceProvider)
        {
            MinLength = node.MinLength;
            MaxLength = node.MaxLength;
            Default = node.Default;
            Suggestions = node.Suggestions.CreateSource(this);
        }

        internal JtStringNodeSource(ICustomSourceParent parent, JtStringNodeSource @base, JObject? @override) : base(parent, @base, @override)
        {
            MinLength = (int)(@override?["minLength"] ?? @base.MinLength);
            MaxLength = (int)(@override?["maxLength"] ?? @base.MaxLength);
            Default = (string?)@override?["default"] ?? @base.Default;
            Suggestions = @base.Suggestions;
        }

        internal JtStringNodeSource(ICustomSourceParent parent, JObject source, ICustomSourceProvider sourceProvider) : base(parent, source, sourceProvider)
        {
            MinLength = (int)(source["minLength"] ?? 0);
            MaxLength = (int)(source["maxLength"] ?? -1);
            Default = (string?)source["default"] ?? string.Empty;
            Suggestions = JtSuggestionCollectionSource<string>.Create(this, source["suggestions"], sourceProvider);
        }


        public string Default { get; internal set; }
        public int MaxLength { get; internal set; }
        public int MinLength { get; internal set; }
        public override IJtSuggestionCollectionSource Suggestions { get; }


        public override JtNodeType Type => JtNodeType.String;
        public override JtNode CreateInstance(JToken? @override, IJtNodeParent parent) => new JtString(this, @override, parent);
        internal override void BuildJsonDeclaration(StringBuilder sb)
        {
            BuildCommonJson(sb);
            if (MaxLength != -1)
                sb.Append($", \"maxLength\": {MaxLength}");
            if (MinLength != 0)
                sb.Append($", \"minLength\": {MinLength}");
            if (!string.IsNullOrEmpty(Default))
                sb.Append($", \"default\": {Default}");
            if (Suggestions.IsSaveable)
            {
                sb.Append(", \"suggestions\": ");
                Suggestions.BuildJson(sb);
            }
            sb.Append('}');
        }
        internal override JtNodeSource CreateOverride(ICustomSourceParent parent, JObject? item) => new JtStringNodeSource(parent, this, item);
    }
}