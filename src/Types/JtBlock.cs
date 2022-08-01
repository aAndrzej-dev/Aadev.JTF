using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtBlock : JtNode, IJtParentNode
    {
        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.Object;
        /// <inheritdoc/>
        public override JtNodeType Type => JtNodeType.Block;
        [Browsable(false)] public JtNodeCollection Children { get; }
        [Browsable(false)]public CustomValue? CustomValueSource => Children.CustomSourceId is null || !Children.CustomSourceId.StartsWith("@") ? null : Template.GetCustomValue(Children.CustomSourceId.AsSpan(1).ToString())!;

        public override bool HasExternalSources => !(Children.CustomSourceId is null);

        public JtBlock(JTemplate template, IIdentifiersManager identifiersManager) : base(template, identifiersManager)
        {
            Children = new JtNodeCollection(this);
        }
        internal JtBlock(JObject obj, JTemplate template, IIdentifiersManager identifiersManager) : base(obj, template, identifiersManager)
        {
            Children = new JtNodeCollection(this, obj["children"]);
        }

        internal override void BulidJson(StringBuilder sb)
        {
            BuildCommonJson(sb);

            sb.Append(",\"children\": ");
            Children.BuildJson(sb);
            sb.Append('}');
        }
        /// <inheritdoc/>
        public override JToken CreateDefaultValue() => new JObject();


    }
}