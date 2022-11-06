using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    [DebuggerDisplay("ID: {Id}; Global ID:{GlobalGuid}")]
    public sealed class CustomSourceDeclaration : ICustomSourceDeclaration
    {
        private static readonly ConcurrentDictionary<Guid, CustomSourceDeclaration> globalDeclarations = new ConcurrentDictionary<Guid, CustomSourceDeclaration>();

        public JtIdentifier Id { get; }
        public Guid GlobalGuid { get; }
        public bool IsGlobal { get; }
        public CustomSource Value { get; }
        public CustomSourceType Type { get; }
        public string Filename { get; }


        public bool IsDeclaratingSource => Value != null;

        private CustomSourceDeclaration(JObject obj, string filename, ICustomSourceProvider sourceProvider)
        {
            Id = (string)obj["id"]!;
            Filename = filename;
            if (obj["globalId"] != null)
            {
                IsGlobal = true;
                GlobalGuid = Guid.Parse((string)obj["globalId"]!);
            }
           
            string valueType = ((string)obj["valueType"])!.ToLowerInvariant();

            switch (valueType)
            {
                case "node":
                    Type = CustomSourceType.Node;
                    Value = JtNodeSource.Create(this, obj["content"]!, sourceProvider);
                    break;
                case "nodecollection":
                    Type = CustomSourceType.NodeCollection;
                    Value = JtNodeCollectionSource.Create(this, obj["content"], sourceProvider);
                    break;
                case "suggestioncollection":
                {
                    Type = CustomSourceType.SuggestionCollection;
                    if (!(obj["content"] is JArray))
                        throw new JtfException("Content is null");
                    string? suggestionType = (string?)obj["suggestionType"] ?? "string";

                    switch (suggestionType)
                    {
                        case "byte":
                        {
                            Value = JtSuggestionCollectionSource<byte>.Create(this, obj["content"], sourceProvider);
                            break;
                        }
                        case "short":
                        {
                            Value = JtSuggestionCollectionSource<short>.Create(this, obj["content"], sourceProvider);
                            break;
                        }
                        case "int":
                        {
                            Value = JtSuggestionCollectionSource<int>.Create(this, obj["content"], sourceProvider);
                            break;
                        }
                        case "long":
                        {
                            Value = JtSuggestionCollectionSource<long>.Create(this, obj["content"], sourceProvider);
                            break;
                        }
                        case "float":
                        {
                            Value = JtSuggestionCollectionSource<float>.Create(this, obj["content"], sourceProvider);
                            break;
                        }
                        case "double":
                        {
                            Value = JtSuggestionCollectionSource<double>.Create(this, obj["content"], sourceProvider);
                            break;
                        }
                        case "string":
                        {
                            Value = JtSuggestionCollectionSource<string>.Create(this, obj["content"], sourceProvider);
                            break;
                        }
                        default:
                            throw new JtfException($"Invalid suggestion type: {suggestionType}");
                    }
                }

                break;
                default:
                    throw new JtfException($"Invalid value type: {valueType}");
            }
        }


        public static CustomSourceDeclaration Create(string filename, ICustomSourceProvider sourceProvider)
        {
            using TextReader tr = File.OpenText(filename);
            using JsonTextReader jr = new JsonTextReader(tr);

            while (jr.Read())
            {
                if (jr.TokenType is JsonToken.PropertyName)
                {
                    if (jr.Value is string s && s == "globalId")
                    {
                        jr.Read();
                        Guid id = Guid.Parse((string?)jr.Value!);
                        if (globalDeclarations.ContainsKey(id))
                            return globalDeclarations[id];
                        jr.Close();
                        JObject o = JObject.Parse(File.ReadAllText(filename));
                        CustomSourceDeclaration element = new CustomSourceDeclaration(o, filename, sourceProvider);
                        globalDeclarations.TryAdd(id, element);
                        return element;

                    }
                }
                if (jr.Depth == 2)
                    break;
            }

            JObject obj = JObject.Parse(File.ReadAllText(filename));
            JtFileType.CustomSource.ThorwIfInvalidType((string?)obj["type"], filename);

            return new CustomSourceDeclaration(obj, filename, sourceProvider);
        }
        public string GetJson()
        {
            StringBuilder sb = new StringBuilder();
           
            sb.Append('{');
            sb.Append("\"type\": \"CustomSource\"");
            sb.Append($", \"version\": {JTemplate.JTFVERSION}");
            if (IsGlobal)
                sb.Append( $", \"globalId\": \"{GlobalGuid}\"");
            sb.Append( $", \"id\": \"{Id}\"");
            sb.Append( $", \"valueType\": \"{Type}\"");
            if (Type is CustomSourceType.SuggestionCollection)
            {


                string suggestionType = "";
                Type element = Value.GetType().GetGenericArguments()[0];
                if (element == typeof(byte))
                    suggestionType = "byte";
                else if (element == typeof(short))
                    suggestionType = "short";
                else if (element == typeof(int))
                    suggestionType = "int";
                else if (element == typeof(long))
                    suggestionType = "long";
                else if (element == typeof(float))
                    suggestionType = "float";
                else if (element == typeof(double))
                    suggestionType = "double";
                else if (element == typeof(string))
                    suggestionType = "string";
                sb.Append( $", \"suggestionType\": \"{suggestionType}\"");

            }
            sb.Append($", \"content\": ");
            Value.BuildJsonDeclaration(sb);
            sb.Append('}');
            return sb.ToString();
        }

        internal static void RemoveGlobalCache() => globalDeclarations.Clear();
        void ICustomSourceDeclaration.BuildJson(StringBuilder sb)
        {
            sb.Append( $"\"@{Id}\"");
        }
    }
}