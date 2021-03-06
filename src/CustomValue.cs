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




            switch (((string?)obj["valueType"])?.ToLower())
            {
                case "nodecollection":
                case "tokencollection": // Backwards compatibility
                    {
                        if (!(obj["content"] is JArray array))
                            throw new Exception("Content is null");
                        CustomValueType = CustomValueType.NodeCollection;
                        JtNode[] tokens = new JtNode[array.Count];

                        for (int i = 0; i < array.Count; i++)
                        {
                            tokens[i] = JtNode.Create((JObject)array[i], template);
                        }
                        Value = tokens;
                    }
                    break;
                case "node":
                case "token": // Backwards compatibility
                    {
                        if (!(obj["content"] is JObject val))
                            throw new Exception("Content is null");
                        Value = JtNode.Create(val, template);

                        CustomValueType = CustomValueType.Node;
                    }
                    break;
                case "enumvaluecollection":
                case "enumvaluescollection": // Backwards compatibility
                    {
                        if (!(obj["content"] is JArray array))
                            throw new Exception("Content is null");

                        Types.JtEnum.EnumValue[] tokens = new Types.JtEnum.EnumValue[array.Count];

                        for (int i = 0; i < array.Count; i++)
                        {
                            JObject? o = ((JObject)array[i]);
                            tokens[i] = new Types.JtEnum.EnumValue((string?)o["name"], (string?)o["displayName"]);
                        }
                        Value = tokens;


                        CustomValueType = CustomValueType.EnumValuesCollection;
                    }
                    break;
                default:
                    throw new Exception("Invalid value");
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
        NodeCollection,
        Node,
        EnumValuesCollection,
    }
}
