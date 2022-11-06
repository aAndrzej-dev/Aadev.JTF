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

        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.Boolean;
        /// <inheritdoc/>
        public override JtNodeType Type => JtNodeType.Bool;

        [DefaultValue(false)] public bool Default { get => @default ?? Base?.Default ?? false; set => @default = value; }
        [DefaultValue(false)] public bool Constant { get => constant ?? Base?.Constant ?? false; set => constant = value; }
        public new JtBoolNodeSource? Base => (JtBoolNodeSource?)base.Base;

        public JtBool(IJtNodeParent parent) : base(parent)
        {
        }
        internal JtBool(JObject obj, IJtNodeParent parent) : base(obj, parent)
        {
            Default = (bool?)obj["default"] ?? false;
            Constant = (bool?)obj["constant"] ?? false;
        }

        internal JtBool(JtBoolNodeSource source, JToken? @override, IJtNodeParent parent) : base(source, @override, parent)
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

        /// <inheritdoc/>
        public override JToken CreateDefaultValue() => new JValue(Default);
        public override JtNodeSource CreateSource() => currentSource ??= new JtBoolNodeSource(this, this);

    }
}