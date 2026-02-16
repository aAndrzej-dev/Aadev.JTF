using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Aadev.JTF.Common;
using Aadev.JTF.CustomSources;
using Aadev.JTF.CustomSources.Declarations;
using Aadev.JTF.Nodes;
using Aadev.JTF.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF;

public sealed class JTemplate : IJtFile, IJtNodeParent, IJtCommonRoot, ICustomSourceProvider, IJsonBuildable
{
    private string name;
    private int version;
    private string? description;


    internal static readonly JsonLoadSettings jsonLoadSettings = new JsonLoadSettings()
    {
        CommentHandling = CommentHandling.Ignore,
        LineInfoHandling = LineInfoHandling.Ignore
    };


    internal static void ThrowIfNotSupportedVersion(int version, IJtFile jtFile)
    {
        if (version > JTF_VERSION)
            throw new JtfException($"JTF file is not supported. Las supported version is: {JTF_VERSION} and file version is: {version}.", jtFile);
    }

    public static void ClearCustomSourceCache()
    {
        CustomSourceDeclaration.ClearGlobalCache();
        CustomSourceDeclarationCollection.ClearGlobalCache();
    }


    public const int JTF_VERSION = 2;
    /// <summary>
    /// Name of template. Default is file name
    /// </summary>
    [Category("General")] public string Name { get => name; set { ThrowIfReadOnly(); name = value; } }
    /// <summary>
    /// Absolute path to jtf file
    /// </summary>
    [Category("General")] public string Filename { get; }
    /// <summary>
    /// If <see cref="false"/> the jtf template is loaded in edit mode, otherwise the template is loaded only needed components to create and edit json files with this template.
    /// </summary>
    [Browsable(false)] public bool IsReadOnly { get; }

    /// <summary>
    /// Version of template
    /// </summary>
    [Category("General")] public int Version { get => version; set { ThrowIfReadOnly(); version = MathExtensions.Clamp(value, 0, JTemplate.JTF_VERSION); } }
    /// <summary>
    /// Description of template
    /// </summary>
    [Category("General")] public string? Description { get => description; set { ThrowIfReadOnly(); description = value; } }
    /// <summary>
    /// Root elements in template
    /// </summary>
    [Browsable(false)] public JtNodeCollection Roots { get; }




    [Browsable(false)]
    public JtFileType FileType => JtFileType.Template;


    [Browsable(false)] public CustomSourceDeclarationCollection? CustomSources { get; set; }

    JtContainerNode? IJtNodeParent.Owner => null;

    [Browsable(false)]
    public IdentifiersManager IdentifiersManager { get; }

    JTemplate IJtNodeParent.Template => this;


    ICustomSourceProvider IHaveCustomSourceProvider.SourceProvider => this;

    bool IJtCommonParent.HasExternalChildrenSource => Roots.IsExternal;

    JtNodeCollection IJtNodeParent.OwnersMainCollection => Roots;


    public static JTemplate Create(string filename, int version = JTF_VERSION, string? name = null, string? description = null, string? customTypeFilename = null)
    {
        JObject obj = new JObject
        {
            ["version"] = version,
            ["type"] = "main"
        };
        if (name is not null)
            obj["name"] = name;
        if (description is not null)
            obj["description"] = description;
        if (customTypeFilename is not null)
            obj["typesFile"] = customTypeFilename;



        DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(filename)!);
        if (!di.Exists)
            di.Create();

        if (File.Exists(filename))
            throw new IOException($"File already exist: '{filename}'");

        File.WriteAllText(filename, obj.ToString(Newtonsoft.Json.Formatting.None));

        return new JTemplate(filename, readOnly: false);


    }
    internal void ThrowIfReadOnly()
    {
        if (IsReadOnly)
            throw new ReadOnlyException("Cannot change items in read-only template.");
    }

    public string GetJson()
    {
        JsonBuilder jb = new JsonBuilder();
        BuildJson(jb);
        return jb.ToString();

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="workingDirectory"></param>
    /// <param name="readOnly"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="JtfException"></exception>
    /// <exception cref="Exception"></exception>
    public static JTemplate Load(string filename, string? workingDirectory = null, bool readOnly = true) => new JTemplate(filename, workingDirectory, readOnly);

    private JTemplate(string filename, string? workingDirectory = null, bool readOnly = true)
    {
        Filename = filename ?? throw new ArgumentNullException(nameof(filename));
        IsReadOnly = readOnly;
        if (!File.Exists(filename))
            throw new FileNotFoundException(filename);
        if (workingDirectory is not null)
        {
            if (Path.GetRelativePath(workingDirectory, filename).StartsWith("..", StringComparison.Ordinal))
            {
                throw new OutOfWorkingDirectoryException($"File is outside working directory!\nFile name: \"{filename}\"\nWorking directory: \"{workingDirectory}\"");
            }
        }

        JObject root;
        try
        {
            using StreamReader sr = new StreamReader(filename);
            using JsonReader jr = new JsonTextReader(sr);


            root = JObject.Load(jr, jsonLoadSettings);

            jr.Close();
        }
        catch (Exception ex)
        {
            throw new JtfException($"Cannot convert file `{filename}` to json", ex, this);
        }

        JtFileType.Template.ThrowIfInvalidType((string?)root["type"], this);

        name = (string?)root["name"] ?? Path.GetFileNameWithoutExtension(Filename);
        description = (string?)root["description"];

        IdentifiersManager = new IdentifiersManager(null);

        if (!int.TryParse((string?)root["version"], out version))
        {
            throw new JtfException($"Parameter 'version' in file `{filename}` must by integer type.", this);
        }


        ThrowIfNotSupportedVersion(version, this);

        string? customSourcesDictionaryFile = (string?)root["customSources"] ?? (string?)root["valuesDictionaryFile"];


        if (!string.IsNullOrEmpty(customSourcesDictionaryFile))
        {
            string? absoluteTypeFilename = Path.GetFullPath(customSourcesDictionaryFile, Path.GetDirectoryName(Filename)!);
            CustomSources = CustomSourceDeclarationCollection.LoadFormFile(this, absoluteTypeFilename, workingDirectory, true);
        }
        //else
        //{
        //    //CustomSources = CustomSourceDeclarationCollection.CreateEmpty(this);
        //}


        if (root["root"] is JToken rootToken)
        {
            Roots = JtNodeCollection.Create(this);
            Roots.Add(JtNode.Create(this, rootToken));
        }
        else if (root["roots"] is JToken rootsToken)
        {
            Roots = JtNodeCollection.Create(this, rootsToken);
        }
        else
        {
            Roots = JtNodeCollection.Create(this);
        }
    }


    public T? GetCustomSource<T>(JtSourceReference identifier) where T : CustomSource
    {
        return identifier.Type switch
        {
            JtSourceReferenceType.None => null,
            JtSourceReferenceType.Dynamic => null,
            JtSourceReferenceType.Direct => IdentifiersManager.GetNodeById(identifier.Identifier)?.CreateSource() is T value ? value : null,
            _ => CustomSources?.GetCustomSource<T>(identifier),
        };
    }


    public CustomSource? GetCustomSource(JtSourceReference identifier)
    {
        return identifier.Type switch
        {
            JtSourceReferenceType.None => null,
            JtSourceReferenceType.Dynamic => null,
            JtSourceReferenceType.Direct => IdentifiersManager.GetNodeById(identifier.Identifier)?.CreateSource(),
            _ => CustomSources?.GetCustomSource(identifier),
        };
    }

    public IdentifiersManager GetIdentifiersManagerForChild() => IdentifiersManager;
    internal void BuildJson(JsonBuilder jb)
    {
        jb.StartBlock();
        jb.AddProperty("name", Name);
        jb.AddProperty("type", "main");
        jb.AddProperty("version", Version);

        if (Description is not null)
            jb.AddProperty("description", Description);
        if (CustomSources is not null)
        {
            jb.AddProperty("customSources", CustomSources);
        }

        if (Roots.Count == 1)
        {
            jb.AddProperty("root", Roots[0]);
        }
        else if (Roots.Count > 1)
        {
            jb.AddProperty("roots");
            jb.StartArray();
            for (int i = 0; i < Roots.Count; i++)
            {
                jb.AddValue(Roots[i]);
            }

            jb.EndArray();
        }

        jb.EndBlock();
    }
    IEnumerable<IJtCommonContentElement> IJtCommonParent.EnumerateChildrenElements() => ((IJtCommonParent)Roots).EnumerateChildrenElements();
    IJtCommonNodeCollection IJtCommonParent.GetChildrenElementsCollection() => Roots;
    IJtCommonNode IJtCommonRoot.CreateNodeElement(IJtCommonParent parent, JtNodeType type) => type.CreateEmptyInstance((IJtNodeParent)parent);
    IJtCommonNodeCollection IJtCommonRoot.CreateCollectionElement(IJtCommonParent parent) => JtNodeCollection.Create((IJtNodeParent)parent);
    public IEnumerable<IJtCustomSourceDeclaration> EnumerateCustomSources()
    {
        return Enumerable.Concat(CustomSources?.EnumerateCustomSources() ?? Enumerable.Empty<IJtCustomSourceDeclaration>(), IdentifiersManager.EnumerateRegisteredNodes().Select(x => x.CreateSource().Declaration));
    }

    void IJsonBuildable.BuildJson(JsonBuilder jsonBuilder) => throw new NotImplementedException();
}