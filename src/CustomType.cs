using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Aadev.JTF
{
    public class CustomType
    {
        public string Filename { get; }
        public JtTokenType BaseType { get; }
        public string Id { get; }

        public JObject Object { get; set; }

        public CustomType(string id, string filename)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Filename = filename ?? throw new ArgumentNullException(nameof(filename));


            try
            {
                Object = JObject.Parse(File.ReadAllText(Filename));
            }
            catch (IOException ex)
            {
                throw new Exception("STOP CODE: JTF_CUSTOMTYPE_FILE_OPEN_EXCEPTION", ex);
            }


            if (Object?["type"]?.ToString() != "Type")
            {
                throw new InvalidJtfFileTypeException(filename, "Type", (string?)Object?["type"]);
            }

            BaseType = JtTokenType.GetByName((string?)Object["baseType"]);
        }
        public override string ToString() => "@" + Id;
    }
}
