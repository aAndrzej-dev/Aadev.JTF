using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Aadev.JTF
{
    public sealed class JTemplate
    {
        public const int JTF_VERSION = 1;
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

        [Category("General")] public string? CustomValuesDictionaryFile { get; set; }
        /// <summary>
        /// Description of template
        /// </summary>
        [Category("General")] public string? Description { get; set; }
        /// <summary>
        /// Root elemnts in template
        /// </summary>
        [Browsable(false)] public JtNode Root { get; }

        [Browsable(false)] private CustomValuesDictionary CustomValues { get; }

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
                throw new Exception($"File already exist: '{filename}'");

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
            if (!(CustomValuesDictionaryFile is null))
                sb.Append($"\"valuesDictionaryFile\": \"{CustomValuesDictionaryFile}\",");

            sb.Append("\"root\": ");
            Root.BulidJson(sb);
            sb.Append('}');
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
        public JTemplate(string filename)
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
                throw new Exception($"Cannot convert file `{filename}` to json", ex);
            }

            if (!((string?)root["type"]).Compare("main", true))
            {
                throw new InvalidJtfFileTypeException(Filename, "main", (string?)root["type"]);
            }


            Name = (string?)root["name"] ?? Path.GetFileNameWithoutExtension(Filename);
            CustomValuesDictionaryFile = (string?)root["valuesDictionaryFile"];
            Description = (string?)root["description"];


            if (!int.TryParse((string?)root["version"], out int ver))
            {
                throw new Exception($"Parameter 'version' in file `{filename}` must by integer type.");
            }


            Version = ver;

            if (!string.IsNullOrEmpty(CustomValuesDictionaryFile))
            {
                string? absoluteTypeFilename = Path.GetFullPath(CustomValuesDictionaryFile, Path.GetDirectoryName(Filename)!);

                if (!File.Exists(absoluteTypeFilename))
                    throw new FileNotFoundException(absoluteTypeFilename);


                CustomValues = new CustomValuesDictionary(absoluteTypeFilename, this);
            }
            else
            {
                CustomValues = CustomValuesDictionary.Empty;
            }

            if (root["root"] is JObject jobj)
                Root = JtNode.Create(jobj, this);
            else
                Root = new JtBlock(this);

        }

        /// <summary>
        /// Gets custom value in <see cref="JTemplate"/> with specific id
        /// </summary>
        /// <param name="id">Id of custom value</param>
        /// <returns>Custom value with specific id</returns>
        public CustomValue? GetCustomValue(string id) => CustomValues.GetCustomValueById(id);

    }
}
