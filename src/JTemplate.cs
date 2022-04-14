using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Aadev.JTF
{
    public sealed class JTemplate
    {
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
        [Browsable(false)] public JtToken Root { get; }

        [Browsable(false)] private CustomValue[] CustomValues { get; }

        public static JTemplate CreateTemplate(string filename, int version, string? name = null, string? description = null, string? customTypeFilename = null)
        {
            JObject obj = new JObject
            {
                ["version"] = version
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

            JObject? root;
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
                string? absoluteTypeFile = Path.GetFullPath(CustomValuesDictionaryFile, Path.GetDirectoryName(Filename)!);

                if (!File.Exists(absoluteTypeFile))
                    throw new FileNotFoundException(absoluteTypeFile);

                JObject valuesDictionaryRoot = JObject.Parse(File.ReadAllText(absoluteTypeFile!));

                if (!((string?)valuesDictionaryRoot["type"]).Compare("valuesdictionary", true))
                {
                    throw new InvalidJtfFileTypeException(absoluteTypeFile!, "valuesdictionary", (string?)valuesDictionaryRoot["type"]);
                }
                List<CustomValue> types = new List<CustomValue>();
                foreach (JValue item in valuesDictionaryRoot["values"]!)
                {
                    string? source = (string?)item.Value;

                    if (source is null)
                        return;

                    source = Path.GetFullPath(source.ToString(), Path.GetDirectoryName(absoluteTypeFile)!);

                    if (!File.Exists(source))
                        throw new FileNotFoundException(source);

                    types.Add(CustomValue.LoadFormFile(source.ToString(), this));
                }
                CustomValues = types.ToArray();



            }
            CustomValues ??= Array.Empty<CustomValue>();
            if (root["root"] is null)
                return;

            Root = JtToken.Create((JObject)root["root"], this);




        }


        /// <summary>
        /// Gets custom type in <see cref="JTemplate"/> with specific id
        /// </summary>
        /// <param name="id">Id of custom type</param>
        /// <returns>Custom type with specific id</returns>
        public CustomValue? GetCustomValue(string id) => CustomValues?.FirstOrDefault(x => x.Id == id);

        /// <summary>
        /// Gets all custom types in <see cref="JTemplate"/>
        /// </summary>
        /// <returns>Custom types registered in template</returns>
        public CustomValue[] GetCustomValues() => CustomValues;

    }
}
