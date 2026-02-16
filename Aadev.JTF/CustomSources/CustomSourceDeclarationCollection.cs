using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using Aadev.JTF.CustomSources.Declarations;
using Aadev.JTF.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.CustomSources;

public class CustomSourceDeclarationCollection : IList<CustomSourceDeclaration>, ICustomSourceProvider, IJtFile, IJsonBuildable
{
    private static readonly ConcurrentDictionary<Guid, CustomSourceDeclarationCollection> globalDeclarations = new ConcurrentDictionary<Guid, CustomSourceDeclarationCollection>();
    private List<CustomSourceDeclaration>? list;
    private readonly JTemplate template;
    private int version;

    public CustomSourceDeclaration this[int index] { get => List[index]; set => List[index] = value; }

    public int Count => List.Count;

    public bool IsReadOnly => false;

    public int Version { get => version; set => version = Math.Clamp(value, 0, JTemplate.JTF_VERSION); }

    public JtFileType FileType => JtFileType.CustomValueDictionary;

    public string Filename { get; }
    public Guid GlobalGuid { get; set; }
    [Browsable(false)] public bool IsGlobal { get; }

    private List<CustomSourceDeclaration> List => list ??= new List<CustomSourceDeclaration>();

    private CustomSourceDeclarationCollection(JTemplate template, string filename, string? workingDirectory, bool readOnly)
    {
        if (string.IsNullOrEmpty(filename))
            throw new ArgumentNullException(nameof(filename), $"'{nameof(filename)}' cannot be null or empty.");
        this.template = template ?? throw new ArgumentNullException(nameof(template));
        Filename = filename;

        if (workingDirectory is not null)
        {
            if (Path.GetRelativePath(workingDirectory, filename).StartsWith("..", StringComparison.Ordinal))
            {
                throw new OutOfWorkingDirectoryException($"File is outside working directory!\nFile name: \"{filename}\"\nWorking directory: \"{workingDirectory}\"");
            }
        }

        if (!File.Exists(filename))
            throw new FileNotFoundException(filename);



        list = new List<CustomSourceDeclaration>();
        using StreamReader sr = new StreamReader(filename);
        using JsonReader jr = new JsonTextReader(sr);


        JObject root = JObject.Load(jr, JTemplate.jsonLoadSettings);

        jr.Close();

        FileType.ThrowIfInvalidType((string?)root["type"], this);
        if (!int.TryParse((string?)root["version"], out version))
        {
            throw new JtfException($"Parameter 'version' in file `{filename}` must by integer type.", this);
        }

        JTemplate.ThrowIfNotSupportedVersion(version, this);

        foreach (JToken item in root["values"]!)
        {
            string? source = (string?)item;

            if (source is null)
            {
                continue;
            }

            source = Path.GetFullPath(source, Path.GetDirectoryName(filename)!);

            if (workingDirectory is not null)
            {
                if (Path.GetRelativePath(workingDirectory, source).StartsWith("..", StringComparison.Ordinal))
                {
                    throw new OutOfWorkingDirectoryException($"File is outside working directory!\nFile name: \"{filename}\"\nWorking directory: \"{workingDirectory}\"");
                }
            }

            if (!File.Exists(source))
            {
                throw new FileNotFoundException(source);
            }

            list.Add(CustomSourceDeclaration.Create(source, readOnly, this));
        }
    }
    private CustomSourceDeclarationCollection(JTemplate template)
    {
        this.template = template;
        Filename = template.Filename;
    }

    internal void BuildJson(JsonBuilder jb)
    {
        if (Filename is not null)
        {
            jb.AddValue(Path.GetRelativePath(Path.GetDirectoryName(template.Filename)!, Filename).Replace("\\", "/", StringComparison.Ordinal));
        }
        else
        {
            jb.StartArray();
#if NET5_0_OR_GREATER
            Span<CustomSourceDeclaration> listSpan = CollectionsMarshal.AsSpan(List);
            for (int i = 0; i < listSpan.Length; i++)
            {
                jb.AddValue(listSpan[i]);

            }
#else
            for (int i = 0; i < List.Count; i++)
            {
                jb.AddValue(List[i]);
            }
#endif
            jb.EndArray();
        }
    }

    public static CustomSourceDeclarationCollection LoadFormFile(JTemplate template, string filename, string? workingDirectory, bool readOnly)
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

                    if (globalDeclarations.TryGetValue(id, out CustomSourceDeclarationCollection? value))
                        return value;
                    jr.Close();


                    using StreamReader sr3 = new StreamReader(filename);
                    using JsonReader jr3 = new JsonTextReader(sr3);


                    JObject o = JObject.Load(jr3, JTemplate.jsonLoadSettings);

                    jr3.Close();

                    CustomSourceDeclarationCollection element = new CustomSourceDeclarationCollection(template, filename, workingDirectory, readOnly);
                    globalDeclarations.TryAdd(id, element);
                    return element;
                }
            }

            if (jr.Depth == 2)
                break;
        }

        return new CustomSourceDeclarationCollection(template, filename, workingDirectory, readOnly);
    }

    public void Add(CustomSourceDeclaration item) => List.Add(item);
    public void Clear() => List.Clear();
    public bool Contains(CustomSourceDeclaration item) => List.Contains(item);
    public void CopyTo(CustomSourceDeclaration[] array, int arrayIndex) => List.CopyTo(array, arrayIndex);
    public IEnumerator<CustomSourceDeclaration> GetEnumerator() => List.GetEnumerator();
    public int IndexOf(CustomSourceDeclaration item) => List.IndexOf(item);
    public void Insert(int index, CustomSourceDeclaration item) => List.Insert(index, item);
    public bool Remove(CustomSourceDeclaration item) => List.Remove(item);
    public void RemoveAt(int index) => List.RemoveAt(index);
    IEnumerator IEnumerable.GetEnumerator() => List.GetEnumerator();
    public T? GetCustomSource<T>(JtSourceReference identifier) where T : CustomSource
    {
        if (identifier.Type is JtSourceReferenceType.External)
        {
#if NET5_0_OR_GREATER
            Span<CustomSourceDeclaration> listSpan = CollectionsMarshal.AsSpan(List);
            for (int i = 0; i < listSpan.Length; i++)
            {
                CustomSourceDeclaration item = listSpan[i];
                if (item.Id == identifier.Identifier)
                    return (T?)item.Value;

            }
#else
            for (int i = 0; i < List.Count; i++)
            {
                CustomSourceDeclaration item = List[i];
                    if (item.Id == identifier.Identifier)
                        return (T?)item.Value;
             }
#endif
        }

        return null;
    }

    public CustomSource? GetCustomSource(JtSourceReference identifier)
    {
        if (identifier.Type is JtSourceReferenceType.External)
        {
#if NET5_0_OR_GREATER
            Span<CustomSourceDeclaration> listSpan = CollectionsMarshal.AsSpan(List);
            for (int i = 0; i < listSpan.Length; i++)
            {
                CustomSourceDeclaration item = listSpan[i];


                if (item.Id == identifier.Identifier)
                    return item.Value;

            }
#else
            for (int i = 0; i < List.Count; i++)
            {
                 CustomSourceDeclaration item = List[i];


                    if (item.Id == identifier.Identifier)
                        return item.Value;
             }
#endif
        }

        return null;
    }

    internal static CustomSourceDeclarationCollection CreateEmpty(JTemplate template) => new CustomSourceDeclarationCollection(template);
    internal static void ClearGlobalCache() => globalDeclarations.Clear();
    public IEnumerable<IJtCustomSourceDeclaration> EnumerateCustomSources() => List;
    void IJsonBuildable.BuildJson(JsonBuilder jb) => BuildJson(jb);
}
