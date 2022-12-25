using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public abstract class JtNodeSource : CustomSource, IJtNodeCollectionSourceChild
    {
        public abstract JtNodeType Type { get; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? DisplayName { get; set; }
        public string? Condition { get; set; }
        public JtIdentifier Id { get; set; }
        public bool Required { get; set; }

        public bool IsArrayPrefab => Parent is JtArrayNodeSource;

        public bool IsDynamicName => Parent is JtArrayNodeSource { ContainerJsonType: Types.JtContainerType.Block };

        public bool IsInArrayPrefab => Parent is JtNodeSource { IsInArrayPrefab: true };

        public JtNodeSource(ICustomSourceParent parent) : base(parent) { }

        private protected JtNodeSource(JtNode node) : base(new CustomSourceFormInstanceDeclaration(node))
        {
            Name = node.Name;
            Description = node.Description;
            Required = node.Required;
            DisplayName = node.DisplayName;
            Id = node.Id;
            Condition = node.Condition;
        }
        private protected JtNodeSource(ICustomSourceParent parent, JObject? source) : base(parent)
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
        private protected JtNodeSource(ICustomSourceParent parent, JtNodeSource @base, JObject? @override) : base(parent)
        {
            Name = (string?)(@override?["name"] ?? @base.Name);
            Description = (string?)(@override?["description"] ?? @base.Description);
            Required = (bool)(@override?["required"] ?? @base.Required);
            DisplayName = (string?)(@override?["displayName"] ?? @override?["name"] ?? @base.DisplayName);
            Id = (string?)(@override?["id"] ?? @base.Id.Identifier);
            Condition = (string?)(@override?["condition"] ?? @base.Condition);
        }

        private protected virtual void BuildCommonJson(StringBuilder sb)
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
        public abstract JtNodeSource CreateOverride(ICustomSourceParent parent, JObject? @override);
        public abstract JtNode CreateInstance(IJtNodeParent parent, JToken? @override);

        internal static JtNodeSource Create(ICustomSourceParent parent, JToken source)
        {
            if (source.Type is JTokenType.String)
            {
                return parent.SourceProvider.GetCustomSource<JtNodeSource>((string)source!) ?? new JtUnknownNodeSource(parent, null);
            }

            string? id = ((string?)source["type"])?.ToLowerInvariant();

            return id switch
            {
                "bool" => new JtBoolNodeSource(parent, (JObject)source),
                "byte" => new JtByteNodeSource(parent, (JObject)source),
                "short" => new JtShortNodeSource(parent, (JObject)source),
                "int" => new JtIntNodeSource(parent, (JObject)source),
                "long" => new JtLongNodeSource(parent, (JObject)source),
                "float" => new JtFloatNodeSource(parent, (JObject)source),
                "double" => new JtDoubleNodeSource(parent, (JObject)source),
                "string" => new JtStringNodeSource(parent, (JObject)source),
                "block" => new JtBlockNodeSource(parent, (JObject)source),
                "array" => new JtArrayNodeSource(parent, (JObject)source),
                _ => new JtUnknownNodeSource(parent, (JObject)source),
            };
        }


        IJtNodeCollectionChild IJtNodeCollectionSourceChild.CreateInstance(IJtNodeParent parent, JToken? @override) => CreateInstance(parent, @override);
        IJtNodeCollectionSourceChild IJtNodeCollectionSourceChild.CreateOverride(ICustomSourceParent parent, JToken? @override) => CreateOverride(parent, (JObject?)@override);
        void IJtNodeCollectionSourceChild.BuildJson(StringBuilder sb) => BuildJson(sb);
    }
}
