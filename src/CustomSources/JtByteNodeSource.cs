using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtByteNodeSource : JtValueNodeSource
    {
        internal JtByteNodeSource(JtByte node, ICustomSourceProvider sourceProvider) : base(node, sourceProvider)
        {
            Max = node.Max;
            Min = node.Min;
            Default = node.Default;
            Suggestions = (JtSuggestionCollectionSource<byte>)node.Suggestions.CreateSource(this);
        }

        internal JtByteNodeSource(ICustomSourceParent parent, JtByteNodeSource @base, JObject? @override) : base(parent, @base, @override)
        {
            Min = (byte)(@override?["minLength"] ?? @base.Min);
            Max = (byte)(@override?["maxLength"] ?? @base.Max);
            Default = (byte)(@override?["default"] ?? @base.Default);
            Suggestions = @base.Suggestions;
        }

        internal JtByteNodeSource(ICustomSourceParent parent, JObject source, ICustomSourceProvider sourceProvider) : base(parent, source, sourceProvider)
        {
            Suggestions = JtSuggestionCollectionSource<byte>.Create(this, source["suggestions"], sourceProvider);
            Min = (byte)(source["min"] ?? byte.MinValue);
            Max = (byte)(source["max"] ?? byte.MaxValue);
            Default = (byte)(source["default"] ?? 0);
        }

      
        public byte Max { get; internal set; }
        public byte Min { get; internal set; }
        public byte Default { get; internal set; }
        public override IJtSuggestionCollectionSource Suggestions { get; }


        public override JtNodeType Type => JtNodeType.Byte;

        public override JtNode CreateInstance(JToken? @override, IJtNodeParent parent) => new JtByte(this,@override, parent);
        internal override void BuildJsonDeclaration(StringBuilder sb)
        {
            BuildCommonJson(sb);
            if (Max != byte.MaxValue)
                sb.Append( $", \"max\": {Max}");
            if (Min != byte.MinValue)
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
        internal override JtNodeSource CreateOverride(ICustomSourceParent parent, JObject? item) => new JtByteNodeSource(parent, this, item);
    }
}