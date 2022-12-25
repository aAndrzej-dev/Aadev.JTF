using Aadev.JTF.CustomSources;
using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Aadev.JTF
{
    public sealed class JTemplate : IJtFile, ICustomSourceProvider, IJtNodeParent
    {
        internal static readonly Regex identifierRegex = new Regex("^[a-z]+[a-z0-9_]*$", RegexOptions.Compiled);
        private string name;
        private int version;
        private string? description;

        public static void RemoveCustomSourcesCache() => CustomSourceDeclaration.RemoveGlobalCache();

        public const int JTF_VERSION = 2;
        /// <summary>
        /// Name of template. Default is file name
        /// </summary>
        [Category("General")] public string Name { get => name; set { if (ReadOnly) return; name = value; } }
        /// <summary>
        /// Absolute path to jtf file
        /// </summary>
        [Category("General")] public string Filename { get; }
        public bool ReadOnly { get; }

        /// <summary>
        /// Version of tempalte
        /// </summary>
        [Category("General")] public int Version { get => version; set { if (ReadOnly) return; version = value; } }

        /// <summary>
        /// Description of template
        /// </summary>
        [Category("General")] public string? Description { get => description; set { if (ReadOnly) return; description = value; } }
        /// <summary>
        /// Root elemnts in template
        /// </summary>
        [Browsable(false)] public IList<JtNode> Roots { get; }



        public bool Debug { get; set; }
#if DEBUG
        = true;
#endif

        public JtFileType Type => JtFileType.Template;


        public CustomSourceDeclarationCollection CustomSources { get; }

        JtContainer? IJtNodeParent.Owner => null;

        public IIdentifiersManager IdentifiersManager { get; }

        JTemplate IJtNodeParent.Template => this;

        bool IJtNodeParent.HasExternalChildren => false;

        ICustomSourceProvider IJtNodeParent.SourceProvider => this;

        public static JTemplate CreateTemplate(string filename, int version = JTF_VERSION, string? name = null, string? description = null, string? customTypeFilename = null)
        {
            JObject obj = new JObject
            {
                ["version"] = version,
                ["type"] = "main"
            };
            if (name != null)
                obj["name"] = name;
            if (description != null)
                obj["description"] = description;
            if (customTypeFilename != null)
                obj["typesFile"] = customTypeFilename;



            DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(filename)!);
            if (!di.Exists)
                di.Create();

            if (File.Exists(filename))
                throw new IOException($"File already exist: '{filename}'");

            File.WriteAllText(filename, obj.ToString(Newtonsoft.Json.Formatting.None));

            return new JTemplate(filename, readOnly: false);


        }

        public string GetJson()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('{');
            sb.Append($"\"name\": \"{Name}\",");
            sb.Append($"\"type\": \"main\",");
            sb.Append($"\"version\": {Version},");

            if (!(Description is null))
                sb.Append($"\"description\": \"{Description}\",");
            if (Roots.Count == 1)
            {
                sb.Append("\"root\": ");
                Roots[0].BuildJson(sb);
                sb.Append('}');
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
        public JTemplate(string filename, string? workingDirectory = null, bool readOnly = true)
        {
            Filename = filename ?? throw new ArgumentNullException(nameof(filename));
            ReadOnly = readOnly;
            if (!File.Exists(filename))
                throw new FileNotFoundException(filename);
            if (workingDirectory != null)
            {
                if (Path.GetRelativePath(workingDirectory, filename).StartsWith("..", StringComparison.Ordinal))
                {
                    throw new Exception($"File: \"{filename}\" is outside working directory: \"{workingDirectory}\"");
                }
            }
            JObject root;
            try
            {
                root = JObject.Parse(File.ReadAllText(Filename));
            }
            catch (Exception ex)
            {
                throw new JtfException($"Cannot convert file `{filename}` to json", ex);
            }
            JtFileType.Template.ThorwIfInvalidType((string?)root["type"], Filename);

            name = (string?)root["name"] ?? Path.GetFileNameWithoutExtension(Filename);
            description = (string?)root["description"];

            IdentifiersManager = new IdentifiersManager(null);

            if (!int.TryParse((string?)root["version"], out version))
            {
                throw new JtfException($"Parameter 'version' in file `{filename}` must by integer type.");
            }
            string? customSourcesDictionaryFile = (string?)root["valuesDictionaryFile"];


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
                Roots.Add(new JtBlock(this));

        }

        public T? GetCustomSource<T>(JtSourceReference identifier) where T : CustomSource
        {
            if (identifier.Type is JtSourceReferenceType.Dynamic)
                return null;
            if (identifier.Type is JtSourceReferenceType.Direct)
            {
                if (IdentifiersManager.GetNodeById(identifier.Identifier)?.CreateSource() is T value)
                    return value;
                return null;
            }
            return CustomSources.GetCustomSource<T>(identifier);
        }


        public CustomSource? GetCustomSource(JtSourceReference identifier)
        {
            if (identifier.Type is JtSourceReferenceType.Dynamic)
                return null;
            if (identifier.Type is JtSourceReferenceType.Direct)
                return IdentifiersManager.GetNodeById(identifier.Identifier)?.CreateSource();
            return CustomSources.GetCustomSource(identifier);
        }

        public IIdentifiersManager GetIdentifiersManagerForChild() => IdentifiersManager;
    }
}