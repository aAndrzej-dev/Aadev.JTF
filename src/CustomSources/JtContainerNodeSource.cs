using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.CustomSources
{
    public abstract class JtContainerNodeSource : JtNodeSource
    {
        public abstract JtNodeCollectionSource Children { get; }
        protected internal JtContainerNodeSource(ICustomSourceParent parent, JObject source, ICustomSourceProvider sourceProvider) : base(parent, source, sourceProvider)
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


        protected internal JtContainerNodeSource(JtContainer node, ICustomSourceProvider sourceProvider) : base(node, sourceProvider)
        {
            DisableCollapse = node.DisableCollapse;
            ContainerJsonType = node.ContainerJsonType;
        }

        protected internal JtContainerNodeSource(ICustomSourceParent parent, JtContainerNodeSource @base, JObject? @override) : base(parent, @base, @override)
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

        public bool DisableCollapse { get; set; }
        public JtContainerType ContainerJsonType { get; set; }
        public abstract JtContainerType ContainerDisplayType { get; }

    }
}
