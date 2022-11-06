using Aadev.JTF.CustomSources;
using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Aadev.JTF
{
    public sealed class JTemplate : IJtFile, ICustomSourceProvider, IJtNodeParent
    {
        internal static readonly Regex identifierRegex = new Regex("^[a-z]+[a-z0-9_]*$", RegexOptions.Compiled);


        public static void RemoveCustomSourcesCache() => CustomSourceDeclaration.RemoveGlobalCache();

        public const int JTFVERSION = 2;
        /// <summary>
        /// Name of template. Default is file name
        /// </summary>
        [Category("General")] public string Name { get; set; }
        /// <summary>
        /// Absolute path to jtf file
        /// </summary>
        [Category("General")] public string Filename { get; }
        /// <summary>
        /// Version of tempalte
        /// </summary>
        [Category("General")] public int Version { get; set; }
        /// <summary>
        /// Relative path to types dictionary file
        /// </summary>

        [Category("General")] public string? CustomSourcesDictionaryFile { get; set; }
        /// <summary>
        /// Description of template
        /// </summary>
        [Category("General")] public string? Description { get; set; }
        /// <summary>
        /// Root elemnts in template
        /// </summary>
        [Browsable(false)] public List<JtNode> Roots { get; }



        public bool Debug { get; set; }
#if DEBUG
        = true;
#endif

        public JtFileType Type => JtFileType.Template;


        public List<CustomSourceDeclaration> CustomSources { get; }

        JtContainer? IJtNodeParent.Owner => null;

        public IIdentifiersManager IdentifiersManager { get; }

        JTemplate IJtNodeParent.Template => this;

        bool IJtNodeParent.HasExternalChildren => false;

        public static JTemplate CreateTemplate(string filename, int version = JTFVERSION, string? name = null, string? description = null, string? customTypeFilename = null)
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

            return new JTemplate(filename);


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
            if (!(CustomSourcesDictionaryFile is null))
                sb.Append($"\"valuesDictionaryFile\": \"{CustomSourcesDictionaryFile}\",");
            if (Roots.Count == 1)
            {
                sb.Append("\"root\": ");
                Roots[0].BuildJson(sb);
                sb.Append('}');
            }

            return sb.ToString();

        }


        /// <summary>
        /// Load template form file
        /// </summary>
        /// <param name="filename">File to load form</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        /// <exception cref="InvalidJtfFileTypeException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public JTemplate(string filename, string? workingDir = null)
        {
            Filename = filename ?? throw new ArgumentNullException(nameof(filename));
            if (!File.Exists(filename))
                throw new FileNotFoundException(filename);

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

            Name = (string?)root["name"] ?? Path.GetFileNameWithoutExtension(Filename);
            CustomSourcesDictionaryFile = (string?)root["valuesDictionaryFile"];
            Description = (string?)root["description"];

            IdentifiersManager = new IdentifiersManager(null);

            if (!int.TryParse((string?)root["version"], out int ver))
            {
                throw new JtfException($"Parameter 'version' in file `{filename}` must by integer type.");
            }


            Version = ver;


            if (!string.IsNullOrEmpty(CustomSourcesDictionaryFile))
            {
                string? absoluteTypeFilename = Path.GetFullPath(CustomSourcesDictionaryFile, Path.GetDirectoryName(Filename)!);

                if (!File.Exists(absoluteTypeFilename))
                    throw new FileNotFoundException(absoluteTypeFilename);

                if (workingDir != null)
                {
                    if (Path.GetRelativePath(workingDir, absoluteTypeFilename).StartsWith("..", StringComparison.Ordinal))
                    {
                        throw new Exception($"File is outside working dir: {absoluteTypeFilename}");
                    }

                }

                JObject valuesDictionaryRoot = JObject.Parse(File.ReadAllText(absoluteTypeFilename));


                JtFileType.CustomValueDictionary.ThorwIfInvalidType((string?)valuesDictionaryRoot["type"], absoluteTypeFilename);
                CustomSources = new List<CustomSourceDeclaration>();

                foreach (JToken item in valuesDictionaryRoot["values"]!)
                {
                    string? source = (string?)item;

                    if (source is null)
                    {
                        continue;
                    }

                    source = Path.GetFullPath(source, Path.GetDirectoryName(absoluteTypeFilename)!);

                    if(workingDir != null)
                    {
                        if(Path.GetRelativePath(workingDir, source).StartsWith("..", StringComparison.Ordinal))
                        {
                            throw new Exception($"File is outside working dir: {source}");
                        }
                        
                    }

                    if (!File.Exists(source))
                    {
                        throw new FileNotFoundException(source);
                    }

                    CustomSources.Add(CustomSourceDeclaration.Create(source, this));
                }
            }
            else
            {
                CustomSources = Enumerable.Empty<CustomSourceDeclaration>().ToList();
            }
            Roots = new List<JtNode>();
            if (root["root"] is JToken rootToken)
                Roots.Add(JtNode.Create(rootToken, this, this));
            else if (root["roots"] is JArray rootsArray)
            {
                foreach (JToken item in rootsArray)
                {
                    Roots.Add(JtNode.Create(item, this, this));
                }
            }
            else
                Roots.Add(new JtBlock(this));

        }

        public T? GetCustomSource<T>(JtCustomResourceIdentifier identifier) where T : CustomSource
        {
            if (identifier.Type is JtCustomResourceIdentifierType.Dynamic)
                return null;
            if (identifier.Type is JtCustomResourceIdentifierType.Direct)
            {
                if (IdentifiersManager.GetNodeById(identifier.Identifier)?.CreateSource() is T value)
                    return value;
                return null;
            }
            return (T?)CustomSources.Where(x => x.Id == identifier.Identifier).FirstOrDefault()?.Value;
        }


        public CustomSource? GetCustomSource(JtCustomResourceIdentifier identifier)
        {
            if (identifier.Type is JtCustomResourceIdentifierType.Dynamic)
                return null;
            if (identifier.Type is JtCustomResourceIdentifierType.Direct)
                return IdentifiersManager.GetRegisteredNodes().Where(x => x.Id == identifier.Identifier).FirstOrDefault()?.CreateSource();
            return CustomSources.Where(x => x.Id == identifier.Identifier).FirstOrDefault()?.Value;
        }

        public IIdentifiersManager GetIdentifiersManagerForChild() => IdentifiersManager;
    }
}