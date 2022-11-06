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
        [Browsable(false)] public abstract JtNodeCollection Children { get; }

        [Browsable(false)] public abstract JtContainerType ContainerDisplayType { get; }

        [DefaultValue(false)] public bool DisableCollapse { get => disableCollapse ?? Base?.DisableCollapse ?? false; set => disableCollapse = value; }
        public JtContainerType ContainerJsonType { get; set; }


        public override JTokenType JsonType => ContainerJsonType is JtContainerType.Array ? JTokenType.Array : JTokenType.Object;

        JtContainer IJtNodeParent.Owner => this;

        public bool HasExternalChildren => Children.HasExternalChildren;

        protected internal JtContainer(IJtNodeParent parent) : base(parent)
        {
            ContainerJsonType = ContainerDisplayType;
        }
        protected internal JtContainer(JObject obj, IJtNodeParent parent) : base(obj, parent)
        {
            DisableCollapse = (bool?)obj["disableCollapse"] ?? false;
            if (obj["jsonType"] is JValue jt)
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

        protected internal JtContainer(JtContainerNodeSource source, JToken? @override, IJtNodeParent parent) : base(source, @override, parent)
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

        protected internal override void BuildCommonJson(StringBuilder sb)
        {
            base.BuildCommonJson(sb);
            if (ContainerJsonType != ContainerDisplayType)
                sb.Append($", \"jsonType\": \"{ContainerJsonType.ToString().ToLowerInvariant()}\"");
            if (DisableCollapse)
                sb.Append(", \"disableCollapse\": true");
        }
        IIdentifiersManager IJtNodeParent.GetIdentifiersManagerForChild() => IdentifiersManager;


    }
    public enum JtContainerType
    {
        Array,
        Block
    }
}
