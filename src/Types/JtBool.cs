using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtBool : JtNode
    {
        private bool? @default;
        private bool? constant;

        public new JtBoolNodeSource? Base => (JtBoolNodeSource?)base.Base;
        public override JTokenType JsonType => JTokenType.Boolean;
        public override JtNodeType Type => JtNodeType.Bool;

        [DefaultValue(false)] public bool Default { get => @default ?? Base?.Default ?? false; set => @default = value; }
        [DefaultValue(false)] public bool Constant { get => constant ?? Base?.Constant ?? false; set => constant = value; }

        public JtBool(IJtNodeParent parent) : base(parent)
        {
        }
        internal JtBool(IJtNodeParent parent, JObject source) : base(parent, source)
        {
            Default = (bool?)source["default"] ?? false;
            Constant = (bool?)source["constant"] ?? false;
        }
        internal JtBool(IJtNodeParent parent, JtBoolNodeSource source, JToken? @override) : base(parent, source, @override)
        {
            if (@override is null)
                return;
            @default = (bool?)@override["default"];
            constant = (bool?)@override["constant"];
        }

        internal override void BuildJson(StringBuilder sb)
        {
            BuildCommonJson(sb);

            if (Default)
                sb.Append($", \"default\": true");
            if (Constant)
                sb.Append(", \"constant\": true");
            sb.Append('}');
        }
        public override JToken CreateDefaultValue() => new JValue(Default);
        public override JtNodeSource CreateSource() => currentSource ??= new JtBoolNodeSource(this);
    }
}