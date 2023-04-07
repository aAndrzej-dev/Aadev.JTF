using Aadev.JTF.AbstractStructure;
using Aadev.JTF.CustomSources;
using Aadev.JTF.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Aadev.JTF
{
    public sealed class JTemplate : IJtFile, ICustomSourceProvider, IJtNodeParent, IJtStructureTemplateElement
    {
        internal static JsonLoadSettings jsonLoadSettings = new JsonLoadSettings()
        {
            CommentHandling = CommentHandling.Ignore,
            LineInfoHandling = LineInfoHandling.Ignore
        };

        internal static readonly Regex identifierRegex = new Regex("^[a-z]+[a-z0-9_]*$", RegexOptions.Compiled);
        private string name;
        private int version;
        private string? description;

        public static void RemoveCustomSourcesCache() => CustomSourceDeclaration.RemoveGlobalCache();

        public const int JTF_VERSION = 2;
        /// <summary>
        /// Name of template. Default is file name
        /// </summary>
        [Category("General")] public string Name { get => name; set { ThrowIfReadOnly(); name = value; } }
        /// <summary>
        /// Absolute path to jtf file
        /// </summary>
        [Category("General")] public string Filename { get; }
        [Browsable(false)] public bool ReadOnly { get; }

        /// <summary>
        /// Version of template
        /// </summary>
        [Category("General")] public int Version { get => version; set { ThrowIfReadOnly(); version = value; } }
        /// <summary>
        /// Description of template
        /// </summary>
        [Category("General")] public string? Description { get => description; set { ThrowIfReadOnly(); description = value; } }
        /// <summary>
        /// Root elements in template
        /// </summary>
        [Browsable(false)] public IList<JtNode> Roots { get; }



        [Browsable(false)]
        public bool Debug { get; set; }
#if DEBUG
        = true;
#endif

        [Browsable(false)]
        public JtFileType Type => JtFileType.Template;


        [Browsable(false)] public CustomSourceDeclarationCollection CustomSources { get; }

        JtContainerNode? IJtNodeParent.Owner => null;

        [Browsable(false)]
        public IIdentifiersManager IdentifiersManager { get; }

        JTemplate IJtNodeParent.Template => this;

        bool IJtNodeParent.HasExternalChildren => false;

        ICustomSourceProvider IJtNodeParent.SourceProvider => this;


        public static JTemplate Create(string filename, int version = JTF_VERSION, string? name = null, string? description = null, string? customTypeFilename = null)
        {
            JObject obj = new JObject
            {
                ["version"] = version,
                ["type"] = "main"
            };
            if (name is not null)
                obj["name"] = name;
            if (description is not null)
                obj["description"] = description;
            if (customTypeFilename is not null)
                obj["typesFile"] = customTypeFilename;



            DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(filename)!);
            if (!di.Exists)
                di.Create();

            if (File.Exists(filename))
                throw new IOException($"File already exist: '{filename}'");

            File.WriteAllText(filename, obj.ToString(Newtonsoft.Json.Formatting.None));

            return new JTemplate(filename, readOnly: false);


        }
        internal void ThrowIfReadOnly()
        {
            if (ReadOnly)
                throw new ReadOnlyException("Cannot change items in read-only template.");
        }

        public string GetJson()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('{');
            sb.Append($"\"name\": \"{Name}\"");
            sb.Append($", \"type\": \"main\"");
            sb.Append($", \"version\": {Version}");

            if (Description is not null)
                sb.Append($", \"description\": \"{Description}\"");
            if (CustomSources.Count > 0)
            {
                sb.Append(", \"customSources\": ");
                CustomSources.BuildJson(sb);
            }
            if (Roots.Count == 1)
            {
                sb.Append(", \"root\": ");
                Roots[0].BuildJson(sb);
                sb.Append('}');
            }
            else if (Roots.Count > 1)
            {
                sb.Append(",\"roots\": [");
                for (int i = 0; i < Roots.Count; i++)
                {
                    if (i > 0)
                        sb.Append(',');
                    Roots[i].BuildJson(sb);
                }
                sb.Append(']');
            }

            return sb.ToString();

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="workingDirectory"></param>
        /// <param name="readOnly"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="JtfException"></exception>
        /// <exception cref="Exception"></exception>
        public static JTemplate Load(string filename, string? workingDirectory = null, bool readOnly = true) => new JTemplate(filename, workingDirectory, readOnly);


        private JTemplate(string filename, string? workingDirectory = null, bool readOnly = true)
        {
            Filename = filename ?? throw new ArgumentNullException(nameof(filename));
            ReadOnly = readOnly;
            if (!File.Exists(filename))
                throw new FileNotFoundException(filename);
            if (workingDirectory is not null)
            {
                if (Path.GetRelativePath(workingDirectory, filename).StartsWith("..", StringComparison.Ordinal))
                {
                    throw new OutOfWorkingDirectoryException($"File is outside working directory!\nFile name: \"{filename}\"\nWorking directory: \"{workingDirectory}\"");
                }
            }
            JObject root;
            try
            {
                using StreamReader sr = new StreamReader(filename);
                using JsonReader jr = new JsonTextReader(sr);
                
                
                root = JObject.Load(jr, jsonLoadSettings);

                jr.Close();
            }
            catch (Exception ex)
            {
                throw new JtfException($"Cannot convert file `{filename}` to json", ex);
            }
            JtFileType.Template.ThrowIfInvalidType((string?)root["type"], Filename);

            name = (string?)root["name"] ?? Path.GetFileNameWithoutExtension(Filename);
            description = (string?)root["description"];

            IdentifiersManager = new IdentifiersManager(null);

            if (!int.TryParse((string?)root["version"], out version))
            {
                throw new JtfException($"Parameter 'version' in file `{filename}` must by integer type.");
            }
            string? customSourcesDictionaryFile = (string?)root["customSources"] ?? (string?)root["valuesDictionaryFile"];


            if (!string.IsNullOrEmpty(customSourcesDictionaryFile))
            {
                string? absoluteTypeFilename = Path.GetFullPath(customSourcesDictionaryFile, Path.GetDirectoryName(Filename)!);
                CustomSources = CustomSourceDeclarationCollection.LoadFormFile(this, absoluteTypeFilename, workingDirectory, true);
            }
            else
            {
                CustomSources = CustomSourceDeclarationCollection.CreateEmpty(this);
            }
            Roots = new List<JtNode>();
            if (root["root"] is JToken rootToken)
                Roots.Add(JtNode.Create(this, rootToken));
            else if (root["roots"] is JArray rootsArray)
            {
                foreach (JToken item in rootsArray)
                {
                    Roots.Add(JtNode.Create(this, item));
                }
            }
            else
                Roots.Add(new JtBlockNode(this));

        }

        public T? GetCustomSource<T>(JtSourceReference identifier) where T : CustomSource
        {
            return identifier.Type switch
            {
                JtSourceReferenceType.None => null,
                JtSourceReferenceType.Dynamic => null,
                JtSourceReferenceType.Direct => IdentifiersManager.GetNodeById(identifier.Identifier)?.CreateSource() is T value ? value : null,
                _ => CustomSources.GetCustomSource<T>(identifier),
            };
        }


        public CustomSource? GetCustomSource(JtSourceReference identifier)
        {
            return identifier.Type switch
            {
                JtSourceReferenceType.None => null,
                JtSourceReferenceType.Dynamic => null,
                JtSourceReferenceType.Direct => IdentifiersManager.GetNodeById(identifier.Identifier)?.CreateSource(),
                _ => CustomSources.GetCustomSource(identifier),
            };
        }

        public IIdentifiersManager GetIdentifiersManagerForChild() => IdentifiersManager;
        public IJtStructureNodeElement CreateNodeElement(IJtStructureParentElement parent, JtNodeType type) => JtNode.Create((IJtNodeParent)parent, type);
        public IEnumerable<IJtStructureInnerElement> GetStructureChildren() => Roots.Select(x => (IJtStructureInnerElement)x);
        public IJtStructureCollectionElement CreateCollectionElement(IJtStructureParentElement parent) => JtNodeCollection.Create((IJtNodeParent)parent);
    }
}