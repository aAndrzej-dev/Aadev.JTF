using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Aadev.JTF
{
    public class CustomValue
    {
        public string Id { get; }
        public int Version { get; }
        private readonly JObject obj;
        private readonly JTemplate template;


        private CustomValue(JObject obj, JTemplate template)
        {
            this.obj = obj;
            this.template = template;
            Id = (string?)obj["id"] ?? throw new Exception("Invalid id");


            if (!int.TryParse((string?)obj["version"], out int ver))
            {

                throw new Exception($"Parameter 'version' must by integer type.");
            }


            Version = ver;





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


        public object GetInstance()
        {
            switch (((string?)obj["valueType"])?.ToLower())
            {
                case "nodecollection":
                case "tokencollection": // Backwards compatibility
                    {
                        if (!(obj["content"] is JArray array))
                            throw new Exception("Content is null");
                        JtNode[] tokens = new JtNode[array.Count];
                        BlankIdentifiersManager? im = new BlankIdentifiersManager();
                        for (int i = 0; i < array.Count; i++)
                        {
                            tokens[i] = JtNode.Create((JObject)array[i], template, im);
                        }
                        return tokens;
                    }
                    break;
                case "node":
                case "token": // Backwards compatibility
                    {
                        if (!(obj["content"] is JObject val))
                            throw new Exception("Content is null");
                        return JtNode.Create(val, template, new BlankIdentifiersManager());

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
                            JObject? o = (JObject)array[i];
                            tokens[i] = new Types.JtEnum.EnumValue((string?)o["name"], (string?)o["displayName"]);
                        }
                        return tokens;


                    }
                    break;
                default:
                    throw new Exception("Invalid value");
            }
        }
    }
    public enum CustomValueType
    {
        NodeCollection,
        Node,
        EnumValuesCollection,
    }
}