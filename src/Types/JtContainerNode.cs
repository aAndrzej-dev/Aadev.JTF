using Aadev.JTF.AbstractStructure;
using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Aadev.JTF.Types
{
    public abstract class JtContainerNode : JtNode, IJtNodeParent, IJtStructureParentElement
    {
        private bool? disableCollapse;

        public new JtContainerNodeSource? Base => (JtContainerNodeSource?)base.Base;
        public override JTokenType JsonType => ContainerJsonType is JtContainerType.Array ? JTokenType.Array : JTokenType.Object;

        [Browsable(false)] public abstract JtNodeCollection Children { get; }
        [Browsable(false)] public abstract JtContainerType ContainerDisplayType { get; }

        [DefaultValue(false)] public bool DisableCollapse { get => disableCollapse ?? Base?.DisableCollapse ?? false; set => disableCollapse = value; }
        public JtContainerType ContainerJsonType { get; set; }





        [Browsable(false)] public bool HasExternalChildren => Children.HasExternalChildren;

        private protected JtContainerNode(IJtNodeParent parent) : base(parent)
        {
            ContainerJsonType = ContainerDisplayType;
        }
        private protected JtContainerNode(IJtNodeParent parent, JObject source) : base(parent, source)
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
        private protected JtContainerNode(IJtNodeParent parent, JtContainerNodeSource source, JToken? @override) : base(parent, source, @override)
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
                sb.Append($", \"jsonType\": \"{ContainerJsonType.ToLowerString()}\"");
            if (DisableCollapse)
                sb.Append(", \"disableCollapse\": true");
        }

        IIdentifiersManager IJtNodeParent.GetIdentifiersManagerForChild() => IdentifiersManager;
        public IEnumerable<IJtStructureInnerElement> GetStructureChildren() => Children.Select(x => (IJtStructureInnerElement)x);


        ICustomSourceProvider IJtNodeParent.SourceProvider => this;
        JtContainerNode IJtNodeParent.Owner => this;

        [Browsable(false)] public bool HasExternalChildrenSource => Children.IsExternal;

        [Browsable(false)] public IJtStructureCollectionElement ChildrenCollection => Children;
    }

    public enum JtContainerType 
    {
        Array,
        Block
    }
    internal static class JtContainerTypeEx
    {
        public static string ToLowerString(this JtContainerType containerType)
        {
            return containerType switch
            {
                JtContainerType.Array => "array",
                JtContainerType.Block => "block",
                _ => throw new ArgumentOutOfRangeException(nameof(containerType)),
            };
        }
    }
}
