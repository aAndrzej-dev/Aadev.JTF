using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.Types
{
    public abstract class JtContainer : JtNode
    {
        private bool disableCollapse;

        public abstract JtNodeCollection Children { get; }
        public abstract JtContainerType ContainerDisplayType { get; }


        public bool DisableCollapse { get => disableCollapse; set => disableCollapse = value; }
        public JtContainerType ContainerJsonType { get; set; }


        public override JTokenType JsonType => ContainerJsonType is JtContainerType.Array ? JTokenType.Array : JTokenType.Object;

        internal JtContainer(JTemplate template, IIdentifiersManager identifiersManager) : base(template, identifiersManager)
        {
            ContainerJsonType = ContainerDisplayType;
        }

        internal JtContainer(JObject obj, JTemplate template, IIdentifiersManager identifiersManager) : base(obj, template, identifiersManager)
        {
            DisableCollapse = (bool?)obj["disableCollapse"] ?? false;

            if (obj["jsonType"] is JValue jt)
            {
                if ((string?)jt.Value is "array")
                    ContainerJsonType = JtContainerType.Array;
                else if ((string?)jt.Value is "block")
                    ContainerJsonType = JtContainerType.Block;
                else
                    ContainerJsonType = ContainerDisplayType;
            }
            else
                ContainerJsonType = ContainerDisplayType;
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
                sb.Append($", \"jsonType\": \"{ContainerJsonType.ToString().ToLower()}\"");
            if (DisableCollapse)
                sb.Append(", \"disableCollapse\": true");
        }
    }
    public enum JtContainerType
    {
        Array,
        Block
    }
}
