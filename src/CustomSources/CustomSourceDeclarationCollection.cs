using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public class CustomSourceDeclarationCollection : IList<CustomSourceDeclaration>, ICustomSourceProvider
    {
        private readonly List<CustomSourceDeclaration> list;
        private readonly JTemplate template;
        private readonly string? filename;

        public CustomSourceDeclaration this[int index] { get => ((IList<CustomSourceDeclaration>)list)[index]; set => ((IList<CustomSourceDeclaration>)list)[index] = value; }

        public int Count => ((ICollection<CustomSourceDeclaration>)list).Count;

        public bool IsReadOnly => ((ICollection<CustomSourceDeclaration>)list).IsReadOnly;



        private CustomSourceDeclarationCollection(JTemplate template, string filename, string? workingDirectory, bool readOnly)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException($"'{nameof(filename)}' cannot be null or empty.", nameof(filename));
            this.template = template ?? throw new ArgumentNullException(nameof(template));
            this.filename = filename;

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
        }

        internal void BuildJson(StringBuilder sb)
        {
            if (filename is not null)
            {
                sb.Append($"\"{Path.GetRelativePath(Path.GetDirectoryName(template.Filename)!, filename).Replace("\\", "/", StringComparison.Ordinal)}\"");
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

        public static CustomSourceDeclarationCollection LoadFormFile(JTemplate template, string filename, string? workingDirectory, bool readOnly) => new CustomSourceDeclarationCollection(template, filename, workingDirectory, readOnly);


        public void Add(CustomSourceDeclaration item) => ((ICollection<CustomSourceDeclaration>)list).Add(item);
        public void Clear() => ((ICollection<CustomSourceDeclaration>)list).Clear();
        public bool Contains(CustomSourceDeclaration item) => ((ICollection<CustomSourceDeclaration>)list).Contains(item);
        public void CopyTo(CustomSourceDeclaration[] array, int arrayIndex) => ((ICollection<CustomSourceDeclaration>)list).CopyTo(array, arrayIndex);
        public IEnumerator<CustomSourceDeclaration> GetEnumerator() => ((IEnumerable<CustomSourceDeclaration>)list).GetEnumerator();
        public int IndexOf(CustomSourceDeclaration item) => ((IList<CustomSourceDeclaration>)list).IndexOf(item);
        public void Insert(int index, CustomSourceDeclaration item) => ((IList<CustomSourceDeclaration>)list).Insert(index, item);
        public bool Remove(CustomSourceDeclaration item) => ((ICollection<CustomSourceDeclaration>)list).Remove(item);
        public void RemoveAt(int index) => ((IList<CustomSourceDeclaration>)list).RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)list).GetEnumerator();
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
    }
}
