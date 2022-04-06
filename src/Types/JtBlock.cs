using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;

namespace Aadev.JTF.Types
{
    public class JtBlock : JtToken, IJtParentType
    {
        private TokensCollection? children;
        public override JTokenType JsonType => JTokenType.Object;
        public override JtTokenType Type => JtTokenType.Block;
        [Browsable(false)]
        public TokensCollection Children
        {
            get
            {
                if (children is null)
                {
                    children = new TokensCollection(this);
                    if (CustomType?.Object["children"] is JArray arr)
                    {
                        foreach (JObject item in arr)
                        {
                            children.Add(Create(item, Template));
                        }
                    }
                }

                return children;
            }
        }
        public JtBlock(JTemplate template) : base(template) { }
        public JtBlock(JObject obj, JTemplate template) : base(obj, template)
        {


            if (IsUsingCustomType) return;

            children = new TokensCollection(this);




            if (obj["children"] is JArray arr)
            {
                foreach (JObject item in arr)
                {
                    children.Add(Create(item, Template));
                }
            }


        }

        internal override void BulidJson(StringBuilder sb)
        {
            sb.Append('{');
            if (!IsArrayPrefab)
                sb.Append($"\"name\": \"{Name}\",");
            if (!string.IsNullOrWhiteSpace(Description))
                sb.Append($"\"description\": \"{Description}\",");
            if (DisplayName != Name)
                sb.Append($"\"displayName\": \"{DisplayName}\",");

            if (!IsUsingCustomType)
            {
                sb.Append("\"children\": [");

                for (int i = 0; i < Children.Count; i++)
                {
                    if (i != 0)
                        sb.Append(',');

                    Children[i].BulidJson(sb);
                }

                sb.Append("],");
            }


            if (Conditions.Count > 0)
            {
                sb.Append("\"conditions\": [");

                for (int i = 0; i < Conditions.Count; i++)
                {
                    if (i != 0)
                        sb.Append(',');

                    sb.Append(Conditions[i].GetString());
                }

                sb.Append("],");
            }

            sb.Append($"\"id\": \"{Id}\",");
            if (IsUsingCustomType)
                sb.Append($"\"type\": \"{CustomType}\"");
            else
                sb.Append($"\"type\": \"{Type.Name}\"");
            sb.Append('}');
        }


    }
}
