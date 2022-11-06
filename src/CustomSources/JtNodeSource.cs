using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public abstract class JtNodeSource : CustomSource, IJtNodeCollectionSourceChild
    {
        private readonly JtNode? node;

        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? DisplayName { get; set; }
        public string? Condition { get; set; }
        public JtIdentifier Id { get; set; }
        public bool Required { get; set; }
        public abstract JtNodeType Type { get; }
        protected internal JtNodeSource(ICustomSourceParent parent, JObject? source, ICustomSourceProvider sourceProvider) : base(parent, sourceProvider)
        {
            if (source is null)
                return;
            Name = (string?)source["name"];
            Description = (string?)source["description"];
            Required = (bool)(source["required"] ?? false);
            DisplayName = (string?)(source["displayName"] ?? Name);
            Id = (string?)source["id"];
            Condition = (string?)source["condition"];
        }
        protected internal JtNodeSource(JtNode node, ICustomSourceProvider sourceProvider) : base(new CustomSourceFormInstanceDeclaration(node), sourceProvider)
        {
            Name = node.Name;
            Description = node.Description;
            Required = node.Required;
            DisplayName = node.DisplayName;
            Id = node.Id;
            Condition = node.Condition;
            this.node = node;
        }
        protected internal JtNodeSource(ICustomSourceParent parent, JtNodeSource @base, JObject? @override) : base(parent, @base.SourceProvider)
        {
            Name = (string?)(@override?["name"] ?? @base.Name);
            Description = (string?)(@override?["description"] ?? @base.Description);
            Required = (bool)(@override?["required"] ?? @base.Required);
            DisplayName = (string?)(@override?["displayName"] ?? @override?["name"] ?? @base.DisplayName);
            Id = (string?)(@override?["id"] ?? @base.Id.Identifier);
            Condition = (string?)(@override?["condition"] ?? @base.Condition);
        }
        internal override void BuildJson(StringBuilder sb)
        {
            if (node is null)
                base.BuildJson(sb);
            else
                sb.Append($"#{node.Id}");
        }
        protected virtual void BuildCommonJson(StringBuilder sb)
        {
            sb.Append('{');
            sb.Append($"\"name\": \"{Name}\",");
            sb.Append($"\"type\": \"{Type.Name}\"");
            if (!string.IsNullOrWhiteSpace(Description))
                sb.Append($", \"description\": \"{Description}\"");
            if (DisplayName != Name)
                sb.Append($", \"displayName\": \"{DisplayName}\"");
            if (!string.IsNullOrEmpty(Id.Identifier))
                sb.Append($", \"id\": \"{Id.Identifier}\"");
            if (Required)
                sb.Append(", \"required\": true");
            if (!string.IsNullOrEmpty(Condition))
                sb.Append($", \"condition\": \"{Condition}\"");
        }
        internal static JtNodeSource Create(ICustomSourceParent parent, JToken item, ICustomSourceProvider sourceProvider)
        {
            if (item.Type is JTokenType.String)
            {
                return sourceProvider.GetCustomSource<JtNodeSource>((string)item!) ?? new JtUnknownNodeSource(parent, null, sourceProvider);
            }

            string? id = ((string?)item["type"])?.ToLowerInvariant();

            return id switch
            {
                "bool" => new JtBoolNodeSource(parent, (JObject)item, sourceProvider),
                "byte" => new JtByteNodeSource(parent, (JObject)item, sourceProvider),
                "short" => new JtShortNodeSource(parent, (JObject)item, sourceProvider),
                "int" => new JtIntNodeSource(parent, (JObject)item, sourceProvider),
                "long" => new JtLongNodeSource(parent, (JObject)item, sourceProvider),
                "float" => new JtFloatNodeSource(parent, (JObject)item, sourceProvider),
                "double" => new JtDoubleNodeSource(parent, (JObject)item, sourceProvider),
                "string" => new JtStringNodeSource(parent, (JObject)item, sourceProvider),
                "block" => new JtBlockNodeSource(parent, (JObject)item, sourceProvider),
                "array" => new JtArrayNodeSource(parent, (JObject)item, sourceProvider),
                _ => new JtUnknownNodeSource(parent, (JObject)item, sourceProvider),
            };
        }
        internal abstract JtNodeSource CreateOverride(ICustomSourceParent parent, JObject? @override);
        public abstract JtNode CreateInstance(JToken? @override, IJtNodeParent parent);
        IJtNodeCollectionChild IJtNodeCollectionSourceChild.CreateInstance(IJtNodeParent parent, JToken? @override) => CreateInstance(@override, parent);
        void IJtNodeCollectionSourceChild.BuildJson(StringBuilder sb) => BuildJson(sb);
    }
}
