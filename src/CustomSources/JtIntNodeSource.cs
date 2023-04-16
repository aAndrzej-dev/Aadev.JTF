﻿using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtIntNodeSource : JtValueNodeSource
    {
        private IJtSuggestionCollectionSource? suggestions;
        public override JtNodeType Type => JtNodeType.Int;

        public int Max { get; set; }
        public int Min { get; set; }
        public int Default { get; set; }
        public override IJtSuggestionCollectionSource Suggestions => suggestions ??= JtSuggestionCollectionSource<int>.Create(this);

        public override JTokenType JsonType => JTokenType.Integer;
        public JtIntNodeSource(IJtNodeSourceParent parent) : base(parent)
        {
            Max = int.MaxValue;
            Min = int.MinValue;
        }
        internal JtIntNodeSource(JtIntNode node) : base(node)
        {
            Max = node.Max;
            Min = node.Min;
            Default = node.Default;
            suggestions = node.TryGetSuggestions()?.CreateSource(this);
        }
        internal JtIntNodeSource(IJtNodeSourceParent parent, JObject source) : base(parent, source)
        {
            Min = (int)(source["min"] ?? int.MinValue);
            Max = (int)(source["max"] ?? int.MaxValue);
            Default = (int)(source["default"] ?? 0);
            suggestions = JtSuggestionCollectionSource<int>.TryCreate(this, source["suggestions"]);
        }
        internal JtIntNodeSource(IJtNodeSourceParent parent, JtIntNodeSource @base, JObject? @override) : base(parent, @base, @override)
        {
            Min = (int)(@override?["minLength"] ?? @base.Min);
            Max = (int)(@override?["maxLength"] ?? @base.Max);
            Default = (int)(@override?["default"] ?? @base.Default);
            suggestions = @base.Suggestions;
        }


        internal override void BuildJsonDeclaration(StringBuilder sb)
        {
            BuildCommonJson(sb);
            if (Max != int.MaxValue)
                sb.Append($", \"max\": {Max}");
            if (Min != int.MinValue)
                sb.Append($", \"min\": {Min}");
            if (Default != 0)
                sb.Append($", \"default\": {Default}");
            if (Suggestions.IsSavable)
            {
                sb.Append(", \"suggestions\": ");
                Suggestions.BuildJson(sb);
            }
            sb.Append('}');
        }
        public override JtNode CreateInstance(IJtNodeParent parent, JToken? @override) => new JtIntNode(parent, this, @override);
        public override JtNodeSource CreateOverride(IJtNodeSourceParent parent, JObject? @override) => new JtIntNodeSource(parent, this, @override);
        public override JToken CreateDefaultValue() => new JValue(Default);
        internal override IJtSuggestionCollectionSource? TryGetSuggestions() => suggestions;
    }
}