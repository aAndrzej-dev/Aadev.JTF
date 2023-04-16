﻿using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtByteNodeSource : JtValueNodeSource
    {
        private IJtSuggestionCollectionSource? suggestions;
        public override JtNodeType Type => JtNodeType.Byte;


        public byte Max { get; set; }
        public byte Min { get; set; }
        public byte Default { get; set; }
        public override IJtSuggestionCollectionSource Suggestions => suggestions ??= JtSuggestionCollectionSource<byte>.Create(this);

        public override JTokenType JsonType => JTokenType.Integer;

        public JtByteNodeSource(IJtNodeSourceParent parent) : base(parent)
        {
            Max = byte.MaxValue;
            Min = byte.MinValue;
        }
        internal JtByteNodeSource(JtByteNode node) : base(node)
        {
            Max = node.Max;
            Min = node.Min;
            Default = node.Default;
            suggestions = node.TryGetSuggestions()?.CreateSource(this);
        }
        internal JtByteNodeSource(IJtNodeSourceParent parent, JObject source) : base(parent, source)
        {
            Min = (byte)(source["min"] ?? byte.MinValue);
            Max = (byte)(source["max"] ?? byte.MaxValue);
            Default = (byte)(source["default"] ?? 0);
            suggestions = JtSuggestionCollectionSource<byte>.TryCreate(this, source["suggestions"]);
        }
        internal JtByteNodeSource(IJtNodeSourceParent parent, JtByteNodeSource @base, JObject? @override) : base(parent, @base, @override)
        {
            Min = (byte)(@override?["minLength"] ?? @base.Min);
            Max = (byte)(@override?["maxLength"] ?? @base.Max);
            Default = (byte)(@override?["default"] ?? @base.Default);
            suggestions = @base.Suggestions;
        }


        internal override void BuildJsonDeclaration(StringBuilder sb)
        {
            BuildCommonJson(sb);
            if (Max != byte.MaxValue)
                sb.Append($", \"max\": {Max}");
            if (Min != byte.MinValue)
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
        public override JtNode CreateInstance(IJtNodeParent parent, JToken? @override) => new JtByteNode(parent, this, @override);
        public override JtNodeSource CreateOverride(IJtNodeSourceParent parent, JObject? @override) => new JtByteNodeSource(parent, this, @override);
        public override JToken CreateDefaultValue() => new JValue(Default);
        internal override IJtSuggestionCollectionSource? TryGetSuggestions() => suggestions;
    }
}