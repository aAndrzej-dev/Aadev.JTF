using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Aadev.JTF.Types
{
    public sealed class JtBlock : JtNode, IJtParentNode
    {
        private JtNodeCollection? children;
        private string? customValueId;

        /// <inheritdoc/>
        public override JTokenType JsonType => JTokenType.Object;
        /// <inheritdoc/>
        public override JtNodeType Type => JtNodeType.Block;
        [Browsable(false)]
        public JtNodeCollection Children
        {
            get
            {
                if (children is null)
                {
                    children = new JtNodeCollection(this);
                    if (customValueId.StartsWith('@'))
                    {
                        children.AddRange((JtNode[])Template.GetCustomValue(customValueId.AsSpan(1).ToString())!.Value);
                    }
                    else if (customValueId.StartsWith('#'))
                    {
                        JtNode? node = IdentifiersManager.GetNodeById(customValueId.AsSpan(1).ToString());
                        if (node is JtBlock block)
                        {

                            children.AddRange(block.Children.ToArray());

                        }
                        else if (node is null)
                        {
                            JtNode? tNode = Template.GetNodeById(customValueId.AsSpan(1).ToString());
                            if (tNode is JtBlock tblock)
                            {
                                children.AddRange(tblock.Children.ToArray());
                            }
                            else
                            {
                                throw new Exception();

                            }
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    children.ReadOnly = true;

                }

                return children;
            }
        }
        public CustomValue? CustomValueSource => customValueId is null || !customValueId.StartsWith("@") ? null : Template.GetCustomValue(customValueId.AsSpan(1).ToString())!;

        public string? CustomValueId
        {
            get => customValueId; set
            {

                if (customValueId == value)
                    return;
                customValueId = value;
                children ??= new JtNodeCollection(this);

                children.ReadOnly = false;
                if (string.IsNullOrWhiteSpace(value))
                {
                    customValueId = null;
                    return;
                }
                if (customValueId is null)
                {
                    return;
                }
                children.Clear();

                if (customValueId.StartsWith('@'))
                {
                    children.AddRange((JtNode[])CustomValueSource!.Value);
                }
                else if (customValueId.StartsWith('#'))
                {
                    JtNode? node = IdentifiersManager.GetNodeById(customValueId.AsSpan(1).ToString());
                    if (node is JtBlock block)
                    {

                        children.AddRange(block.Children.ToArray());
                    }
                    else
                    {
                        throw new Exception();
                    }
                }

                children.ReadOnly = true;
            }
        }
        public override bool HasExternalSources => !(CustomValueId is null);

        public JtBlock(JTemplate template, IIdentifiersManager identifiersManager) : base(template, identifiersManager)
        {
            children = new JtNodeCollection(this);
        }
        internal JtBlock(JObject obj, JTemplate template, IIdentifiersManager identifiersManager) : base(obj, template, identifiersManager)
        {
            if (obj["children"] is null)
            {
                children = new JtNodeCollection(this);
            }
            else if (obj["children"] is JArray arr)
            {
                children = new JtNodeCollection(this);
                foreach (JObject item in arr)
                {
                    children.Add(Create(item, Template, IdentifiersManager));
                }
            }
            else if (((JValue?)obj["children"])?.Value is string str)
            {
                if (!str.StartsWith("@") && !str.StartsWith("#"))
                    throw new Exception("Custom values name must starts with '@' or '#'");

                customValueId = str;
            }



        }

        internal override void BulidJson(StringBuilder sb)
        {
            BuildCommonJson(sb);


            if (customValueId is null)
            {
                if (Children.Count > 0)
                {
                    sb.Append(",\"children\": [");

                    for (int i = 0; i < Children.Count; i++)
                    {
                        if (i != 0)
                            sb.Append(',');

                        Children[i].BulidJson(sb);
                    }

                    sb.Append(']');
                }

            }
            else
            {
                sb.Append($", \"children\": \"{customValueId}\"");
            }
            sb.Append('}');
        }
        /// <inheritdoc/>
        public override JToken CreateDefaultValue() => new JObject();
    }
}