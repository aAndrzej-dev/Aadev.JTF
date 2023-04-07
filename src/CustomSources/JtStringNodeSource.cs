using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtStringNodeSource : JtValueNodeSource
    {
        public override JtNodeType Type => JtNodeType.String;


        public string Default { get; set; }
        public int MaxLength { get; set; }
        public int MinLength { get; set; }
        public override IJtSuggestionCollectionSource Suggestions { get; }

        public override JTokenType JsonType => JTokenType.String;

        public JtStringNodeSource(IJtNodeSourceParent parent) : base(parent)
        {
            Default = string.Empty;
            MaxLength = -1;
            MinLength = 0;
            Suggestions = JtSuggestionCollectionSource<short>.Create(this);
        }
        internal JtStringNodeSource(JtStringNode node) : base(node)
        {
            MinLength = node.MinLength;
            MaxLength = node.MaxLength;
            Default = node.Default;
            Suggestions = node.Suggestions.CreateSource(this);
        }
        internal JtStringNodeSource(IJtNodeSourceParent parent, JObject source) : base(parent, source)
        {
            MinLength = (int)(source["minLength"] ?? 0);
            MaxLength = (int)(source["maxLength"] ?? -1);
            Default = (string?)source["default"] ?? string.Empty;
            Suggestions = JtSuggestionCollectionSource<string>.Create(this, source["suggestions"]);
        }
        internal JtStringNodeSource(IJtNodeSourceParent parent, JtStringNodeSource @base, JObject? @override) : base(parent, @base, @override)
        {
            MinLength = (int)(@override?["minLength"] ?? @base.MinLength);
            MaxLength = (int)(@override?["maxLength"] ?? @base.MaxLength);
            Default = (string?)@override?["default"] ?? @base.Default;
            Suggestions = @base.Suggestions;
        }


        internal override void BuildJsonDeclaration(StringBuilder sb)
        {
            BuildCommonJson(sb);
            if (MaxLength != -1)
                sb.Append($", \"maxLength\": {MaxLength}");
            if (MinLength != 0)
                sb.Append($", \"minLength\": {MinLength}");
            if (!string.IsNullOrEmpty(Default))
                sb.Append($", \"default\": {Default}");
            if (Suggestions.IsSavable)
            {
                sb.Append(", \"suggestions\": ");
                Suggestions.BuildJson(sb);
            }
            sb.Append('}');
        }
        public override JtNode CreateInstance(IJtNodeParent parent, JToken? @override) => new JtStringNode(parent, this, @override);
        public override JtNodeSource CreateOverride(IJtNodeSourceParent parent, JObject? @override) => new JtStringNodeSource(parent, this, @override);
        public override JToken CreateDefaultValue() => new JValue(Default);
    }
}