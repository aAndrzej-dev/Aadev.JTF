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

        public JObject Object { get; }

        public CustomType(string id, string filename)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Filename = filename ?? throw new ArgumentNullException(nameof(filename));


            try
            {
                Object = JObject.Parse(File.ReadAllText(Filename));
            }
            catch (IOException)
            {
                throw;
            }


            if ((string?)Object["type"] != "Type")
            {
                throw new InvalidJtfFileTypeException(filename, "Type", (string?)Object["type"]);
            }

            BaseType = JtTokenType.GetByName((string?)Object["baseType"]);
        }
        public override string ToString() => "@" + Id;
    }
}
