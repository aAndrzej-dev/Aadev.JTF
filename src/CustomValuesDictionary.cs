using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Aadev.JTF
{
    internal class CustomValuesDictionary
    {
        private readonly CustomValue[] customValues;
        public CustomValuesDictionary(string filename, JTemplate template)
        {
            JObject valuesDictionaryRoot = JObject.Parse(File.ReadAllText(filename));

            if (!((string?)valuesDictionaryRoot["type"]).Compare("valuesdictionary", true))
            {
                throw new InvalidJtfFileTypeException(filename, "valuesdictionary", (string?)valuesDictionaryRoot["type"]);
            }

            List<CustomValue> customValues = new List<CustomValue>();

            foreach (JValue item in valuesDictionaryRoot["values"]!)
            {
                string? source = (string?)item.Value;

                if (source is null)
                {
                    continue;
                }

                source = Path.GetFullPath(source.ToString(), Path.GetDirectoryName(filename)!);

                if (!File.Exists(source))
                {
                    throw new FileNotFoundException(source);
                }

                customValues.Add(CustomValue.LoadFormFile(source.ToString(), template));
            }
            this.customValues = customValues.ToArray();
        }
        private CustomValuesDictionary()
        {
            customValues = Array.Empty<CustomValue>();
        }

        private static readonly CustomValuesDictionary empty = new CustomValuesDictionary();
        public static CustomValuesDictionary Empty => empty;

        public CustomValue? GetCustomValueById(string id) => customValues.Where(x => x.Id == id).SingleOrDefault();
    }
}