using Aadev.JTF.AbstractStructure;
using Aadev.JTF.CollectionBuilders;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtNodeCollectionSource : CustomSource, IJtNodeCollectionSourceChild, IJtStructureCollectionElement, IJtNodeSourceParent, IList<IJtNodeCollectionSourceChild>
    {
        private IJtCollectionBuilder<IJtNodeCollectionSourceChild>? childrenBuilder;
        private readonly JtNodeCollectionSource? @base;
        private List<IJtNodeCollectionSourceChild>? children;

        [MemberNotNull(nameof(children))]
        internal List<IJtNodeCollectionSourceChild> Children
        {
            get
            {
                if (children is null)
                {
                    children = childrenBuilder!.Build();
                    childrenBuilder = null;
                }
                return children;
            }
        }


        public override bool IsExternal => @base?.IsDeclared ?? IsDeclared;
        public override IJtCustomSourceDeclaration? Base => base.Base ?? @base?.Declaration;

        public bool HasExternalChildrenSource => IsExternal;

        public IJtStructureCollectionElement ChildrenCollection => this;

        public JtNodeSource? Owner => ((IJtNodeSourceParent?)Parent)?.Owner;


        public int Count => ((ICollection<IJtNodeCollectionSourceChild>)Children).Count;

        public bool IsReadOnly => ((ICollection<IJtNodeCollectionSourceChild>)Children).IsReadOnly;

        public IJtNodeCollectionSourceChild this[int index] { get => ((IList<IJtNodeCollectionSourceChild>)Children)[index]; set => ((IList<IJtNodeCollectionSourceChild>)Children)[index] = value; }

        private JtNodeCollectionSource(IJtNodeSourceParent parent) : base(parent)
        {
            childrenBuilder = JtCollectionBuilder.CreateEmpty<IJtNodeCollectionSourceChild>();
        }

        private JtNodeCollectionSource(JtNodeCollection instance) : base(new CustomSourceFormInstanceDeclaration(instance.Owner))
        {
            childrenBuilder = JtCollectionBuilder.CreateJtNodeSourceCollection(instance);
        }

        private JtNodeCollectionSource(IJtNodeSourceParent parent, JtNodeCollectionSource @base, JArray? @override) : base(parent)
        {
            childrenBuilder = JtCollectionBuilder.CreateJtNodeSourceCollection(this, @base, @override);
            this.@base = @base;
        }

        private JtNodeCollectionSource(IJtNodeSourceParent parent, JArray? jArray) : base(parent)
        {
            childrenBuilder = JtCollectionBuilder.CreateJtNodeSourceCollection(this, jArray);
        }

        internal override void BuildJsonDeclaration(StringBuilder sb)
        {
            sb.Append('[');
            bool isFirst = true;

#if NET5_0_OR_GREATER
            Span<IJtNodeCollectionSourceChild> listSpan = CollectionsMarshal.AsSpan(Children);
            for (int i = 0; i < listSpan.Length; i++)
            {
                if (!isFirst)
                    sb.Append(',');
                else
                    isFirst = false;
                listSpan[i].BuildJson(sb);
            }
#else
            for (int i = 0; i < Children.Count; i++)
            {
                if (!isFirst)
                    sb.Append(',');
                else
                    isFirst = false;
                 Children[i].BuildJson(sb);
            }
#endif
            sb.Append(']');
        }
        internal static JtNodeCollectionSource Create(IJtNodeSourceParent parent) => new JtNodeCollectionSource(parent);
        internal static JtNodeCollectionSource Create(IJtNodeSourceParent parent, JToken? source)
        {
            if (source?.Type is JTokenType.String)
            {
                return parent.SourceProvider.GetCustomSource<JtNodeCollectionSource>((string?)source) ?? new JtNodeCollectionSource(parent);
            }
            return new JtNodeCollectionSource(parent, (JArray?)source);

        }

        internal static JtNodeCollectionSource Create(JtNodeCollection instance) => new JtNodeCollectionSource(instance);
        internal JtNodeCollectionSource CreateOverride(IJtNodeSourceParent parent, JArray? @override) => new JtNodeCollectionSource(parent, this, @override);
        IJtNodeCollectionChild IJtNodeCollectionSourceChild.CreateInstance(IJtNodeParent parent, JToken? @override) => CreateInstance(parent, @override);
        public JtNodeCollection CreateInstance(IJtNodeParent parent, JToken? @override) => new JtNodeCollection(parent, this, @override as JArray);
        IJtNodeCollectionSourceChild IJtNodeCollectionSourceChild.CreateOverride(IJtNodeSourceParent parent, JToken? @override) => CreateOverride(parent, (JArray?)@override);
        void IJtNodeCollectionSourceChild.BuildJson(StringBuilder sb) => BuildJson(sb);
        public void Add(IJtStructureInnerElement item) => Children.Add((IJtNodeCollectionSourceChild)item);
        public IEnumerable<IJtStructureInnerElement> GetStructureChildren() => Children;

        public int IndexOf(IJtNodeCollectionSourceChild item) => ((IList<IJtNodeCollectionSourceChild>)Children).IndexOf(item);
        public void Insert(int index, IJtNodeCollectionSourceChild item) => ((IList<IJtNodeCollectionSourceChild>)Children).Insert(index, item);
        public void RemoveAt(int index) => ((IList<IJtNodeCollectionSourceChild>)Children).RemoveAt(index);
        public void Add(IJtNodeCollectionSourceChild item) => ((ICollection<IJtNodeCollectionSourceChild>)Children).Add(item);
        public void Clear() => ((ICollection<IJtNodeCollectionSourceChild>)Children).Clear();
        public bool Contains(IJtNodeCollectionSourceChild item) => ((ICollection<IJtNodeCollectionSourceChild>)Children).Contains(item);
        public void CopyTo(IJtNodeCollectionSourceChild[] array, int arrayIndex) => ((ICollection<IJtNodeCollectionSourceChild>)Children).CopyTo(array, arrayIndex);
        public bool Remove(IJtNodeCollectionSourceChild item) => ((ICollection<IJtNodeCollectionSourceChild>)Children).Remove(item);
        public IEnumerator<IJtNodeCollectionSourceChild> GetEnumerator() => ((IEnumerable<IJtNodeCollectionSourceChild>)Children).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Children).GetEnumerator();
    }
}
