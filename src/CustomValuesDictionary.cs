using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aadev.JTF
{
    internal class CustomValuesDictionary
    {
        private readonly CustomValue[] CustomValues;
        public CustomValuesDictionary(string filename, JTemplate template)
        {
            JObject valuesDictionaryRoot = JObject.Parse(File.ReadAllText(filename));

            if (!((string?)valuesDictionaryRoot["type"]).Compare("valuesdictionary", true))
                throw new InvalidJtfFileTypeException(filename, "valuesdictionary", (string?)valuesDictionaryRoot["type"]);


            List<CustomValue> customValues = new List<CustomValue>();

            foreach (JValue item in valuesDictionaryRoot["values"]!)
            {
                string? source = (string?)item.Value;

                if (source is null)
                    continue;

                source = Path.GetFullPath(source.ToString(), Path.GetDirectoryName(filename)!);

                if (!File.Exists(source))
                    throw new FileNotFoundException(source);





                customValues.Add(CustomValue.LoadFormFile(source.ToString(), template));
            }
            CustomValues = customValues.ToArray();
        }
        private CustomValuesDictionary()
        {
            CustomValues = Array.Empty<CustomValue>();
        }

        public static readonly CustomValuesDictionary Empty = new CustomValuesDictionary();

        public CustomValue GetCustomValueById(string id) => CustomValues.Where(x => x.Id == id).Single();
    }
}
