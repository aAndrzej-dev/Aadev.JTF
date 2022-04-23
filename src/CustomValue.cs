using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Aadev.JTF
{
    public class CustomValue
    {
        public CustomValueType CustomValueType { get; }
        public string Id { get; }
        public int Version { get; }
        public object Value { get; }



        private CustomValue(JObject obj, JTemplate template)
        {
            Id = (string?)obj["id"] ?? throw new Exception("Invalid id");


            if (!int.TryParse((string?)obj["version"], out int ver))
            {

                throw new Exception($"Parameter 'version' must by integer type.");
            }


            Version = ver;



            if (!Enum.TryParse((string?)obj["valueType"], true, out CustomValueType customValueType))
                throw new Exception("Invalid value type");
            CustomValueType = customValueType;


            switch (CustomValueType)
            {
                case CustomValueType.TokenCollection:
                    {
                        if (!(obj["content"] is JArray array))
                            throw new Exception("Content is null");

                        JtToken[] tokens = new JtToken[array.Count];

                        for (int i = 0; i < array.Count; i++)
                        {
                            tokens[i] = JtToken.Create((JObject)array[i], template);
                        }
                        Value = tokens;
                    }
                    break;
                case CustomValueType.Token:
                    {
                        if (!(obj["content"] is JObject val))
                            throw new Exception("Content is null");
                        Value = JtToken.Create(val, template);

                    }
                    break;
                case CustomValueType.EnumValuesCollection:
                    {
                        if (!(obj["content"] is JArray array))
                            throw new Exception("Content is null");

                        string?[] tokens = new string[array.Count];

                        for (int i = 0; i < array.Count; i++)
                        {
                            tokens[i] = (string?)((JObject)array[i])["name"];
                        }
                        Value = tokens;
                    }
                    break;
                default:
                    throw new Exception("Invalid enum value");
            }

        }
        public static CustomValue LoadFormFile(string filename, JTemplate template)
        {
            JObject obj = JObject.Parse(File.ReadAllText(filename));
            if (!((string?)obj["type"]).Compare("cusotmvalue", true))
            {
                throw new InvalidJtfFileTypeException(filename, "cusotmvalue", (string?)obj["type"]);
            }

            return new CustomValue(obj, template);
        }


    }
    public enum CustomValueType
    {
        TokenCollection,
        Token,
        EnumValuesCollection,
    }
}
