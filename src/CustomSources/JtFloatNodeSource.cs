﻿using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtFloatNodeSource : JtValueNodeSource
    {
        public override JtNodeType Type => JtNodeType.Float;

        public float Max { get; set; }
        public float Min { get; set; }
        public float Default { get; set; }
        public override IJtSuggestionCollectionSource Suggestions { get; }
        public JtFloatNodeSource(ICustomSourceParent parent) : base(parent)
        {
            Max = float.MaxValue;
            Min = float.MinValue;
            Suggestions = JtSuggestionCollectionSource<float>.Create(this);
        }
        internal JtFloatNodeSource(JtFloat node) : base(node)
        {
            Max = node.Max;
            Min = node.Min;
            Default = node.Default;
            Suggestions = node.Suggestions.CreateSource(this);
        }
        internal JtFloatNodeSource(ICustomSourceParent parent, JObject source) : base(parent, source)
        {
            Suggestions = JtSuggestionCollectionSource<float>.Create(this, source["suggestions"]);
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
        public override JtNode CreateInstance(IJtNodeParent parent, JToken? @override) => new JtFloat(parent, this, @override);
        public override JtNodeSource CreateOverride(ICustomSourceParent parent, JObject? @override) => new JtFloatNodeSource(parent, this, @override);
    }
}