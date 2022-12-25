using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace Aadev.JTF.CustomSources
{
    public abstract class JtContainerNodeSource : JtNodeSource
    {
        public abstract JtNodeCollectionSource Children { get; }
        public abstract JtContainerType ContainerDisplayType { get; }
        public bool DisableCollapse { get; set; }
        public JtContainerType ContainerJsonType { get; set; }

        public JtContainerNodeSource(ICustomSourceParent parent) : base(parent) { ContainerJsonType = ContainerDisplayType; }
        private protected JtContainerNodeSource(JtContainer node) : base(node)
        {
            DisableCollapse = node.DisableCollapse;
            ContainerJsonType = node.ContainerJsonType;
        }
        private protected JtContainerNodeSource(ICustomSourceParent parent, JObject source) : base(parent, source)
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
        private protected JtContainerNodeSource(ICustomSourceParent parent, JtContainerNodeSource @base, JObject? @override) : base(parent, @base, @override)
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
                ContainerJsonType = @base.ContainerJsonType;
            DisableCollapse = (bool)(@override?["disableCollapse"] ?? @base.DisableCollapse);
        }
    }
}
