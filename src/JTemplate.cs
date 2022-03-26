namespace Aadev.JTF
{
    public class JTemplate : IJtParentType
    {
        public string Name { get; set; }
        public string Filename { get; }
        public int Version { get; set; }
        public string? CustomTypesFile { get; set; }
        public string? Description { get; set; }

        [Browsable(false)] public TokensCollection Children { get; }
        [Browsable(false)] public JTemplate Template => this;
        [Browsable(false)] public bool IsInArrayPrefab => false;
        [Browsable(false)] public JtTokenType Type => JtTokenType.Unknown;
        [Browsable(false)] public bool IsExternal => false;

        private CustomType[]? CustomTypes { get; }


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

        public JTemplate(string filename)
        {
            Filename = filename ?? throw new ArgumentNullException(nameof(filename));
            Children = new TokensCollection(this);



            JObject? root;
            try
            {

                root = JObject.Parse(File.ReadAllText(Filename));
            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot convert file `{filename}` to json", ex);
            }

            if ((string?)root["type"] != "Main")
            {
                throw new InvalidJtfFileTypeException(Filename, "Main", (string?)root["type"]);
            }


            Name = (string?)root["name"] ?? Path.GetFileNameWithoutExtension(Filename);
            CustomTypesFile = (string?)root["typesFile"];
            Description = (string?)root["description"];


            if (!int.TryParse((string?)root["version"], out int ver))
            {
                throw new Exception($"Parameter 'version' in file `{filename}` must by integer type.");
            }


            Version = ver;

            if (!string.IsNullOrEmpty(CustomTypesFile))
            {
                string? absoluteTypeFile = Path.GetFullPath(CustomTypesFile.Replace('/', '\\'), Path.GetDirectoryName(Filename)!);
                try
                {
                    JObject typesRoot = JObject.Parse(File.ReadAllText(absoluteTypeFile!));

                    if (typesRoot?["type"]?.ToString() != "Types")
                    {
                        throw new InvalidJtfFileTypeException(absoluteTypeFile!, "Types", typesRoot?["type"]?.ToString());
                    }
                    List<CustomType> types = new List<CustomType>();
                    foreach (JToken item in typesRoot["types"]!)
                    {
                        try
                        {
                            string? id = (string?)item["id"];
                            string? source = (string?)item["source"];

                            if (id is null)
                                continue;
                            if (source is null)
                                continue;

                            source = Path.GetFullPath(source, Path.GetDirectoryName(absoluteTypeFile)!);



                            types.Add(new CustomType(id, source));
                        }
                        catch (Exception)
                        {
                            CustomTypes = types.ToArray();
                            throw;
                        }



                    }
                    CustomTypes = types.ToArray();
                }
                catch (Exception ex)
                {
                    throw new Exception("STOP CODE: JTF_TEMPLATE_CUSTOMTYPES_PARSE_EXCEPTION", ex);
                }


            }



            foreach (JObject item in root["content"]!)
            {
                Children.Add(JtToken.Create(item, this));
            }




        }

        public CustomType? GetCustomType(string id) => CustomTypes?.FirstOrDefault(x => x.Id == id);
        public CustomType[]? GetCustomTypes() => CustomTypes;

    }
}
