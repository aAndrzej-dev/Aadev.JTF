using Aadev.JTF.CustomSources;
using Aadev.JTF.JtEnumerable;
using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aadev.JTF
{
    public sealed class JtNodeCollection : IJtNodeCollectionChild, IJtNodeParent, IList<IJtNodeCollectionChild>
    {
        private readonly JtCustomResourceIdentifier id;
        internal readonly IJtEnumerable<IJtNodeCollectionChild> nodeEnumerable;
        private readonly JtNodeCollectionSource? @base;
        private List<JtNode>? nodes;
        private List<IJtNodeCollectionChild>? children;
        private IIdentifiersManager? childrenManager;

        public IJtNodeParent Parent { get; }
        public IIdentifiersManager IdentifiersManager => Parent.IdentifiersManager;

        public JtContainer Owner => Parent.Owner!;


        public JTemplate Template => Parent.Template;
        public bool IsMainCollection => Parent.Owner == Parent;

        public int Count => Children.Count;

        public bool IsReadOnly => false;

        public IJtNodeCollectionChild this[int index] { get => Children[index]; set => Children[index] = value; }


        private JtNodeCollection(IJtNodeParent parent)
        {
            Parent = parent;
            nodeEnumerable = JtEnumerable.JtEnumerable.CreateEmpty<IJtNodeCollectionChild>();
        }

        internal JtNodeCollection(IJtNodeParent parent, JtNodeCollectionSource? source, JtCustomResourceIdentifier id, ICustomSourceProvider sourceProvider)
        {
            Parent = parent;
            @base = source;
            this.id = id;
            nodeEnumerable = JtEnumerable.JtEnumerable.CreatJtNodeCollection(this, source, null, sourceProvider);
        }
        private JtNodeCollection(IJtNodeParent parent, JArray source, ICustomSourceProvider sourceProvider)
        {
            Parent = parent;
            nodeEnumerable = JtEnumerable.JtEnumerable.CreatJtNodeCollection(this, source, sourceProvider);
        }
        internal JtNodeCollection(IJtNodeParent parent, JtNodeCollectionSource source, JArray? @override, ICustomSourceProvider sourceProvider)
        {
            Parent = parent;
            @base = source;
            nodeEnumerable = JtEnumerable.JtEnumerable.CreatJtNodeCollection(this, source, @override, sourceProvider);
        }

        public static JtNodeCollection Create(IJtNodeParent parent) => new JtNodeCollection(parent);
        public static JtNodeCollection Create(IJtNodeParent parent, JToken? source, ICustomSourceProvider sourceProvider)
        {
            if (source?.Type is JTokenType.String)
            {
                JtCustomResourceIdentifier id = (string?)source;
                if (id.Type is JtCustomResourceIdentifierType.None)
                    return new JtNodeCollection(parent);
                if (id.Type is JtCustomResourceIdentifierType.Dynamic)
                    return new JtNodeCollection(parent, null, id, sourceProvider);
                if (id.Type is JtCustomResourceIdentifierType.External)
                    return sourceProvider.GetCustomSource<JtNodeCollectionSource>(id)?.CreateInstance(parent, id) ?? new JtNodeCollection(parent, null, id, sourceProvider);
                if (id.Type is JtCustomResourceIdentifierType.Direct)
                {
                    JtContainerNodeSource? element = sourceProvider.GetCustomSource<JtContainerNodeSource>(id);
                    if (element is null)
                        return new JtNodeCollection(parent, null, id, sourceProvider);
                    return element.Children.CreateInstance(parent, id);

                }
                throw new InternalException();
            }
            if (source?.Type is JTokenType.Array)
            {
                return new JtNodeCollection(parent, (JArray)source,  sourceProvider);
            }
            if (source?.Type is JTokenType.Object)
            {
                JToken? @base = source["base"];
                if (@base is null)
                    return new JtNodeCollection(parent);
                
                if(new CustomSourceBaseDeclaration(@base, sourceProvider).Value is JtNodeCollectionSource ncs)
                    return new JtNodeCollection(parent, ncs, (JArray?)source["_"], sourceProvider);
                throw new InternalException();

            }
            return new JtNodeCollection(parent);
        }

        public void BuildJson(StringBuilder sb)
        {
            if (@base != null)
            {
                bool isAnyChildOverriden = Children.Any(x => x.IsOverriden());


                if (!isAnyChildOverriden && !@base.IsDeclarated)
                    return;

                if(!isAnyChildOverriden)
                {
                    @base.BuildJson(sb);
                    return;
                }    


                sb.Append('{');
                if (@base.IsDeclarated)
                {
                    sb.Append("\"base\": ");
                    @base.BuildJson(sb);
                }
                if (isAnyChildOverriden)
                {
                    sb.Append(", \"_\": [");
                    for (int i = 0; i < Children.Count; i++)
                    {
                        if (i > 0)
                            sb.Append(',');
                        IJtNodeCollectionChild item = Children[i];
                        if (item.IsOverriden())
                            item.BuildJson(sb);
                        else
                        {
                            sb.Append("{}");
                        }
                    }
                    sb.Append(']');
                }
                sb.Append('}');
            }
            else
            {
                sb.Append('[');
                bool isFirst = true;
                foreach (IJtNodeCollectionChild item in nodeEnumerable)
                {
                    if (!isFirst)
                    {
                        sb.Append(',');
                    }
                    else
                        isFirst = false;
                    item.BuildJson(sb);
                }
                sb.Append(']');
            }
        }
        internal JtNodeCollectionSource CreateSource(ICustomSourceProvider sourceProvider) => JtNodeCollectionSource.Create(this, sourceProvider);

        public List<JtNode>? Nodes => IsMainCollection ? nodes ??= nodeEnumerable.SelectMany(x => x.GetNodes()).ToList() : null;
        private List<IJtNodeCollectionChild> Children => children ??= nodeEnumerable.Enumerate();

        public bool HasExternalChildren => (@base != null && @base.IsDeclarated) || Children.Any(x => x is JtNodeCollection n && n.HasExternalChildren);

        public bool IsOverriden() => IsMainCollection ? Nodes!.Any(x => x.IsOverriden()) : nodeEnumerable.Enumerate().Any(x => x.IsOverriden());
        public int IndexOf(IJtNodeCollectionChild item) => Children.IndexOf(item);
        public void Insert(int index, IJtNodeCollectionChild item) => Children.Insert(index, item);
        public void RemoveAt(int index) => Children.RemoveAt(index);
        public void Add(IJtNodeCollectionChild item) => Children.Add(item);
        public void Clear() => Children.Clear();
        public bool Contains(IJtNodeCollectionChild item) => Children.Contains(item);
        public void CopyTo(IJtNodeCollectionChild[] array, int arrayIndex) => Children.CopyTo(array, arrayIndex);
        public bool Remove(IJtNodeCollectionChild item) => Children.Remove(item);
        public IEnumerator<IJtNodeCollectionChild> GetEnumerator() => Children.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IEnumerable<JtNode> IJtNodeCollectionChild.GetNodes() => nodes ?? nodeEnumerable.SelectMany(x => x.GetNodes());




        public IIdentifiersManager GetIdentifiersManagerForChild()
        {
            if (Owner.ContainerDisplayType is JtContainerType.Array)
                return new IdentifiersManager(IdentifiersManager);
            if ((@base is null || !@base.IsDeclarated)&& id.IsEmpty)
                return IdentifiersManager;
            return childrenManager ??= new IdentifiersManager(IdentifiersManager);
        }
    }
}
