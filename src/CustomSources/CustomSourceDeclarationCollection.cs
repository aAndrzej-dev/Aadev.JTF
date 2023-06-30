using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Aadev.JTF.CustomSources.Declarations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.CustomSources;

public class CustomSourceDeclarationCollection : IList<CustomSourceDeclaration>, ICustomSourceProvider, IJtFile
{
    private static readonly ConcurrentDictionary<Guid, CustomSourceDeclarationCollection> globalDeclarations = new ConcurrentDictionary<Guid, CustomSourceDeclarationCollection>();
    private readonly List<CustomSourceDeclaration> list;
    private readonly JTemplate template;
    private int version;

    public CustomSourceDeclaration this[int index] { get => list[index]; set => list[index] = value; }

    public int Count => list.Count;

    public bool IsReadOnly => false;

    public int Version { get => version; set => version = Math.Clamp(value, 0, JTemplate.JTF_VERSION); }

    public JtFileType FileType => JtFileType.CustomValueDictionary;

    public string Filename { get; }
    public Guid GlobalGuid { get; set; }
    [Browsable(false)] public bool IsGlobal { get; }

    private CustomSourceDeclarationCollection(JTemplate template, string filename, string? workingDirectory, bool readOnly)
    {
        if (string.IsNullOrEmpty(filename))
            throw new ArgumentException($"'{nameof(filename)}' cannot be null or empty.", nameof(filename));
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
        list = new List<CustomSourceDeclaration>();
        this.template = template;
        Filename = template.Filename;
    }

    internal void BuildJson(StringBuilder sb)
    {
        if (Filename is not null)
        {
            sb.Append($"\"{Path.GetRelativePath(Path.GetDirectoryName(template.Filename)!, Filename).Replace("\\", "/", StringComparison.Ordinal)}\"");
        }
        else
        {
            sb.Append('[');
#if NET5_0_OR_GREATER
            Span<CustomSourceDeclaration> listSpan = CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < listSpan.Length; i++)
            {
                ((IJtCustomSourceDeclaration)listSpan[i]).BuildJson(sb);
            }
#else
            for (int i = 0; i < list.Count; i++)
            {
                ((IJtCustomSourceDeclaration)list[i]).BuildJson(sb);
            }
#endif
            sb.Append(']');
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

    public void Add(CustomSourceDeclaration item) => list.Add(item);
    public void Clear() => list.Clear();
    public bool Contains(CustomSourceDeclaration item) => list.Contains(item);
    public void CopyTo(CustomSourceDeclaration[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);
    public IEnumerator<CustomSourceDeclaration> GetEnumerator() => list.GetEnumerator();
    public int IndexOf(CustomSourceDeclaration item) => list.IndexOf(item);
    public void Insert(int index, CustomSourceDeclaration item) => list.Insert(index, item);
    public bool Remove(CustomSourceDeclaration item) => list.Remove(item);
    public void RemoveAt(int index) => list.RemoveAt(index);
    IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
    public T? GetCustomSource<T>(JtSourceReference identifier) where T : CustomSource
    {
        if (identifier.Type is JtSourceReferenceType.External)
        {
#if NET5_0_OR_GREATER
            Span<CustomSourceDeclaration> listSpan = CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < listSpan.Length; i++)
            {
                CustomSourceDeclaration item = listSpan[i];
                if (item.Id == identifier.Identifier)
                    return (T?)item.Value;

            }
#else
            for (int i = 0; i < list.Count; i++)
            {
                CustomSourceDeclaration item = list[i];
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
            Span<CustomSourceDeclaration> listSpan = CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < listSpan.Length; i++)
            {
                CustomSourceDeclaration item = listSpan[i];


                if (item.Id == identifier.Identifier)
                    return item.Value;

            }
#else
            for (int i = 0; i < list.Count; i++)
            {
                 CustomSourceDeclaration item = list[i];


                    if (item.Id == identifier.Identifier)
                        return item.Value;
             }
#endif
        }

        return null;
    }

    internal static CustomSourceDeclarationCollection CreateEmpty(JTemplate template) => new CustomSourceDeclarationCollection(template);
    internal static void ClearGlobalCache() => globalDeclarations.Clear();
    public IEnumerable<IJtCustomSourceDeclaration> EnumerateCustomSources() => list;
}
