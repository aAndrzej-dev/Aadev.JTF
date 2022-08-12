using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Aadev.JTF
{
    public class CustomValue
    {
        private object? value;
        private readonly JObject obj;

        public CustomValueType CustomValueType { get; private set; }
        public string Id { get; }
        public int Version { get; }
        public JTemplate Template { get; }
        public IIdentifiersManager IdentifiersManager { get; }
        public object Value
        {
            get
            {
                if (value != null)
                    return value;
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
                            tokens[i] = JtNode.Create((JObject)array[i], Template, IdentifiersManager);
                        }
                        value = tokens;
                    }
                    break;
                    case "node":
                    case "token": // Backwards compatibility
                    {
                        if (!(obj["content"] is JObject val))
                            throw new Exception("Content is null");
                        CustomValueType = CustomValueType.Node;
                        value = JtNode.Create(val, Template, IdentifiersManager);

                    }
                    break;
                    case "enumvaluecollection": // Backwards compatibility
                    case "enumvaluescollection": // Backwards compatibility
                    case "suggestioncollection":
                    {
                        if (!(obj["content"] is JArray array))
                            throw new Exception("Content is null");

                        string? type = (string?)obj["suggestionType"] ?? "string";

                        if (type is null)
                            throw new Exception();

                        CustomValueType = CustomValueType.SuggestionCollection;
                        switch (type)
                        {
                            case "string":
                            {
                                JtSuggestion<string>[] tokens = new JtSuggestion<string>[array.Count];

                                for (int i = 0; i < array.Count; i++)
                                {
                                    JObject? o = (JObject)array[i];
                                    tokens[i] = new JtSuggestion<string>(o);
                                }
                                value = tokens;
                                break;
                            }
                            default:
                                throw new Exception();
                        }



                    }
                    break;
                    default:
                        throw new Exception("Invalid value");
                }
                return value;
            }
        }
        private CustomValue(JObject obj, JTemplate template)
        {
            this.obj = obj;
            Template = template;
            Id = (string?)obj["id"] ?? throw new Exception("Invalid id");


            if (!int.TryParse((string?)obj["version"], out int ver))
            {

                throw new Exception($"Parameter 'version' must by integer type.");
            }


            Version = ver;
            IdentifiersManager = new BlankIdentifiersManager();


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
        SuggestionCollection,
    }
}