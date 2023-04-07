using Aadev.JTF.AbstractStructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    [DebuggerDisplay("ID: {Id}; Global ID:{GlobalGuid}")]
    public sealed class CustomSourceDeclaration : IJtCustomSourceDeclaration
    {
        private static readonly ConcurrentDictionary<Guid, CustomSourceDeclaration> globalDeclarations = new ConcurrentDictionary<Guid, CustomSourceDeclaration>();
        private IdentifiersManager? identifiersManager;

        public JtIdentifier Id { get; set; }
        public Guid GlobalGuid { get; set; }
        [Browsable(false)] public bool IsGlobal { get; }
        public CustomSource Value { get; set; }
        [Browsable(false)] public CustomSourceType Type { get; private set; }
        public string Filename { get; }
        [Browsable(false)] public ICustomSourceProvider SourceProvider { get; }
        [Browsable(false)] public bool ReadOnly { get; }

        [MemberNotNullWhen(true, nameof(Value))]
        public bool IsDeclaringSource => Value is not null;

        IJtCustomSourceDeclaration IJtCustomSourceParent.Declaration => this;



        [MemberNotNull(nameof(identifiersManager))]
        [Browsable(false)]
        public IIdentifiersManager IdentifiersManager => identifiersManager ??= new IdentifiersManager(null);

        [Browsable(false)] public string Name => $"@{Id}";

        JtNodeSource? IJtNodeSourceParent.Owner => null;
        public IJtStructureCollectionElement CreateCollectionElement(IJtStructureParentElement parent) => JtNodeCollectionSource.Create((IJtNodeSourceParent)parent);
        private CustomSourceDeclaration(JObject obj, string filename, bool readOnly, ICustomSourceProvider sourceProvider)
        {
            Id = (string)obj["id"]!;
            Filename = filename;
            SourceProvider = sourceProvider;
            if (obj["globalId"] is not null)
            {
                IsGlobal = true;
                GlobalGuid = Guid.Parse((string)obj["globalId"]!);
            }

            string valueType = ((string)obj["valueType"])!.ToLowerInvariant();

            switch (valueType)
            {
                case "node":
                    Type = CustomSourceType.Node;
                    Value = JtNodeSource.Create(this, obj["content"]!);
                    break;
                case "nodecollection":
                    Type = CustomSourceType.NodeCollection;
                    Value = JtNodeCollectionSource.Create(this, obj["content"]);
                    break;
                case "suggestioncollection":
                {
                    Type = CustomSourceType.SuggestionCollection;
                    if (obj["content"] is not JArray)
                        throw new JtfException("Content is null");
                    string? suggestionType = (string?)obj["suggestionType"] ?? "string";

                    switch (suggestionType)
                    {
                        case "byte":
                        {
                            Value = JtSuggestionCollectionSource<byte>.Create(this, obj["content"]);
                            break;
                        }
                        case "short":
                        {
                            Value = JtSuggestionCollectionSource<short>.Create(this, obj["content"]);
                            break;
                        }
                        case "int":
                        {
                            Value = JtSuggestionCollectionSource<int>.Create(this, obj["content"]);
                            break;
                        }
                        case "long":
                        {
                            Value = JtSuggestionCollectionSource<long>.Create(this, obj["content"]);
                            break;
                        }
                        case "float":
                        {
                            Value = JtSuggestionCollectionSource<float>.Create(this, obj["content"]);
                            break;
                        }
                        case "double":
                        {
                            Value = JtSuggestionCollectionSource<double>.Create(this, obj["content"]);
                            break;
                        }
                        case "string":
                        {
                            Value = JtSuggestionCollectionSource<string>.Create(this, obj["content"]);
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
            ReadOnly = readOnly;
        }


        public static CustomSourceDeclaration Create(string filename, bool readOnly, ICustomSourceProvider sourceProvider)
        {
            using TextReader sr = File.OpenText(filename);
            using JsonTextReader jr = new JsonTextReader(sr);

            while (jr.Read())
            {
                if (jr.TokenType is JsonToken.PropertyName)
                {
                    if (jr.Value is string s && s == "globalId")
                    {
                        jr.Read();
                        Guid id = Guid.Parse((string?)jr.Value!);

                        if (globalDeclarations.TryGetValue(id, out CustomSourceDeclaration? value))
                            return value;
                        jr.Close();


                        using StreamReader sr3 = new StreamReader(filename);
                        using JsonReader jr3 = new JsonTextReader(sr3);


                        JObject o = JObject.Load(jr3, JTemplate.jsonLoadSettings);

                        jr3.Close();

                        CustomSourceDeclaration element = new CustomSourceDeclaration(o, filename, readOnly, sourceProvider);
                        globalDeclarations.TryAdd(id, element);
                        return element;
                    }
                }
                if (jr.Depth == 2)
                    break;
            }
            using StreamReader sr2 = new StreamReader(filename);
            using JsonReader jr2 = new JsonTextReader(sr2);

            JObject obj = JObject.Load(jr2, JTemplate.jsonLoadSettings);

            jr2.Close();

            JtFileType.CustomSource.ThrowIfInvalidType((string?)obj["type"], filename);

            return new CustomSourceDeclaration(obj, filename, readOnly, sourceProvider);
        }
        public string GetJson()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('{');
            sb.Append("\"type\": \"CustomSource\"");
            sb.Append(
#if NET6_0_OR_GREATER
                System.Globalization.CultureInfo.InvariantCulture,
#endif
                $", \"version\": {JTemplate.JTF_VERSION}");
            if (IsGlobal)
                sb.Append(
#if NET6_0_OR_GREATER
                System.Globalization.CultureInfo.InvariantCulture,
#endif
                    $", \"globalId\": \"{GlobalGuid}\"");
            sb.Append(
#if NET6_0_OR_GREATER
                System.Globalization.CultureInfo.InvariantCulture,
#endif
                $", \"id\": \"{Id}\"");
            sb.Append(
#if NET6_0_OR_GREATER
                System.Globalization.CultureInfo.InvariantCulture,
#endif
                $", \"valueType\": \"{Type}\"");
            if (Type is CustomSourceType.SuggestionCollection)
            {
                if (Value is not IJtSuggestionCollectionSource suggestionCollection)
                    throw new UnreachableException();

                ReadOnlySpan<char> suggestionType;
                Type element = suggestionCollection.SuggestionType;
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
                else
                    throw new UnreachableException();


#if NET6_0_OR_GREATER
                sb.Append(System.Globalization.CultureInfo.InvariantCulture, $", \"suggestionType\": \"{suggestionType}\"");
#else
                sb.Append($", \"suggestionType\": \"{suggestionType.ToString()}\"");
#endif


            }
            sb.Append($", \"content\": ");
            Value.BuildJsonDeclaration(sb);
            sb.Append('}');
            return sb.ToString();
        }

        internal static void RemoveGlobalCache() => globalDeclarations.Clear();
        void IJtCustomSourceDeclaration.BuildJson(StringBuilder sb)
        {
            sb.Append($"\"{Name}\"");
        }

        public override string ToString() => Name;
        public IJtStructureNodeElement CreateNodeElement(IJtStructureParentElement parent, JtNodeType type) => JtNodeSource.Create((IJtNodeSourceParent)parent, type);
        public IEnumerable<IJtStructureInnerElement> GetStructureChildren()
        {
            yield return (IJtStructureInnerElement)Value;
        }
    }
}