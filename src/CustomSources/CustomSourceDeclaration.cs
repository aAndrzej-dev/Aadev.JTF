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
using System.Text;

namespace Aadev.JTF.CustomSources
{
    [DebuggerDisplay("ID: {Id}; Global ID:{GlobalGuid}")]
    public sealed class CustomSourceDeclaration : IJtCustomSourceDeclaration, IJtFile
    {
        private static readonly ConcurrentDictionary<Guid, CustomSourceDeclaration> globalDeclarations = new ConcurrentDictionary<Guid, CustomSourceDeclaration>();
        private IdentifiersManager? identifiersManager;
        private int version;

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

        public int Version { get => version; set => version = Math.Clamp(value, 0, JTemplate.JTF_VERSION); }

        public JtFileType FileType => JtFileType.CustomSource;

        public IJtStructureCollectionElement CreateCollectionElement(IJtStructureParentElement parent) => JtNodeCollectionSource.Create((IJtNodeSourceParent)parent);
        private CustomSourceDeclaration(JObject root, string filename, bool readOnly, ICustomSourceProvider sourceProvider)
        {
            Id = (string)root["id"]!;
            Filename = filename;
            SourceProvider = sourceProvider;
            if (root["globalId"] is not null)
            {
                IsGlobal = true;
                GlobalGuid = Guid.Parse((string)root["globalId"]!);
            }

            string? valueType = (string?)root["valueType"];



            FileType.ThrowIfInvalidType((string?)root["type"], this);
            if (!int.TryParse((string?)root["version"], out version))
            {
                throw new JtfException($"Parameter 'version' in file `{filename}` must by integer type.", this);
            }
            JTemplate.ThrowIfNotSupportedVersion(version, this);

            if (valueType is null)
            {
                throw new JtfException($"Value type is not specified in file: {filename}", this);
            }
            else if (valueType.Length == 4 && valueType.Equals("node", StringComparison.OrdinalIgnoreCase))
            {
                Type = CustomSourceType.Node;
                Value = JtNodeSource.Create(this, root["content"]!);
            }
            else if (valueType.Length == 14 && valueType.Equals("nodecollection", StringComparison.OrdinalIgnoreCase))
            {
                Type = CustomSourceType.NodeCollection;
                Value = JtNodeCollectionSource.Create(this, root["content"]);
            }
            else if (valueType.Length == 20 && valueType.Equals("suggestioncollection", StringComparison.OrdinalIgnoreCase))
            {
                Type = CustomSourceType.SuggestionCollection;
                if (root["content"] is not JArray)
                    throw new JtfException("Content is null", this);
                string? suggestionType = (string?)root["suggestionType"] ?? "string";

                switch (suggestionType)
                {
                    case "byte":
                    {
                        Value = JtSuggestionCollectionSource<byte>.Create(this, root["content"]);
                        break;
                    }
                    case "short":
                    {
                        Value = JtSuggestionCollectionSource<short>.Create(this, root["content"]);
                        break;
                    }
                    case "int":
                    {
                        Value = JtSuggestionCollectionSource<int>.Create(this, root["content"]);
                        break;
                    }
                    case "long":
                    {
                        Value = JtSuggestionCollectionSource<long>.Create(this, root["content"]);
                        break;
                    }
                    case "float":
                    {
                        Value = JtSuggestionCollectionSource<float>.Create(this, root["content"]);
                        break;
                    }
                    case "double":
                    {
                        Value = JtSuggestionCollectionSource<double>.Create(this, root["content"]);
                        break;
                    }
                    case "string":
                    {
                        Value = JtSuggestionCollectionSource<string>.Create(this, root["content"]);
                        break;
                    }
                    default:
                        throw new JtfException($"Invalid suggestion type: {suggestionType}", this);
                }
            }
            else
            {
                throw new JtfException($"Invalid value type: \"{valueType}\" in file {filename}", this);
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
        public IJtStructureNodeElement CreateNodeElement(IJtStructureParentElement parent, JtNodeType type)
        {
            if (parent is null)
                throw new ArgumentNullException(nameof(parent));
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            return JtNodeSource.Create((IJtNodeSourceParent)parent, type);
        }

        public IEnumerable<IJtStructureInnerElement> GetStructureChildren()
        {
            yield return (IJtStructureInnerElement)Value;
        }
    }
}