using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public abstract class JtContainer : JtNode, IJtNodeParent
    {
        private bool? disableCollapse;

        public new JtContainerNodeSource? Base => (JtContainerNodeSource?)base.Base;
        public override JTokenType JsonType => ContainerJsonType is JtContainerType.Array ? JTokenType.Array : JTokenType.Object;

        [Browsable(false)] public abstract JtNodeCollection Children { get; }
        [Browsable(false)] public abstract JtContainerType ContainerDisplayType { get; }

        [DefaultValue(false)] public bool DisableCollapse { get => disableCollapse ?? Base?.DisableCollapse ?? false; set => disableCollapse = value; }
        public JtContainerType ContainerJsonType { get; set; }





        public bool HasExternalChildren => Children.HasExternalChildren;

        private protected JtContainer(IJtNodeParent parent) : base(parent)
        {
            ContainerJsonType = ContainerDisplayType;
        }
        private protected JtContainer(IJtNodeParent parent, JObject source) : base(parent, source)
        {
            DisableCollapse = (bool?)source["disableCollapse"] ?? false;
            if (source["jsonType"] is JValue jt)
            {
                ContainerJsonType = (string?)jt.Value switch
                {
                    "array" => JtContainerType.Array,
                    "block" => JtContainerType.Block,
                    _ => ContainerDisplayType,
                };
            }
            else
                ContainerJsonType = ContainerDisplayType;
        }
        private protected JtContainer(IJtNodeParent parent, JtContainerNodeSource source, JToken? @override) : base(parent, source, @override)
        {
            if (@override?["jsonType"] is JValue jt)
            {
                ContainerJsonType = (string?)jt.Value switch
                {
                    "array" => JtContainerType.Array,
                    "block" => JtContainerType.Block,
                    _ => ContainerDisplayType,
                };
            }
            else
                ContainerJsonType = source.ContainerJsonType;
            disableCollapse = (bool?)(@override?["disableCollapse"]);
        }

        public override JToken CreateDefaultValue()
        {
            if (ContainerJsonType is JtContainerType.Array)
                return new JArray();
            else
                return new JObject();
        }

        private protected override void BuildCommonJson(StringBuilder sb)
        {
            base.BuildCommonJson(sb);
            if (ContainerJsonType != ContainerDisplayType)
                sb.Append($", \"jsonType\": \"{ContainerJsonType.ToString().ToLowerInvariant()}\"");
            if (DisableCollapse)
                sb.Append(", \"disableCollapse\": true");
        }

        IIdentifiersManager IJtNodeParent.GetIdentifiersManagerForChild() => IdentifiersManager;
        ICustomSourceProvider IJtNodeParent.SourceProvider => this;
        JtContainer IJtNodeParent.Owner => this;
    }
    public enum JtContainerType
    {
        Array,
        Block
    }
}
