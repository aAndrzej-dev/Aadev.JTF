using Aadev.JTF.JtEnumerable;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtNodeCollectionSource : CustomSource, IJtNodeCollectionSourceChild
    {
        internal readonly IJtEnumerable<IJtNodeCollectionSourceChild> nodeEnumerable;

        private JtNodeCollectionSource(ICustomSourceParent parent) : base(parent)
        {
            nodeEnumerable = JtEnumerable.JtEnumerable.CreateEmpty<IJtNodeCollectionSourceChild>();
        }

        private JtNodeCollectionSource(JtNodeCollection instance) : base(new CustomSourceFormInstanceDeclaration(instance.Owner))
        {
            nodeEnumerable = JtEnumerable.JtEnumerable.CreateJtNodeSourceCollection(instance);
        }

        private JtNodeCollectionSource(ICustomSourceParent parent, JtNodeCollectionSource @base, JArray? @override) : base(parent)
        {
            nodeEnumerable = JtEnumerable.JtEnumerable.CreateJtNodeSourceCollection(this, @base, @override);
        }

        private JtNodeCollectionSource(ICustomSourceParent parent, JArray jArray) : base(parent)
        {
            nodeEnumerable = JtEnumerable.JtEnumerable.CreateJtNodeSourceCollection(this, jArray);
        }

        internal override void BuildJsonDeclaration(StringBuilder sb)
        {
            sb.Append('[');
            bool isFirst = true;
            List<IJtNodeCollectionSourceChild> list = nodeEnumerable.Enumerate();

#if NET5_0_OR_GREATER
            Span<IJtNodeCollectionSourceChild> listSpan = CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < listSpan.Length; i++)
            {
                if (!isFirst)
                    sb.Append(',');
                else
                    isFirst = false;
                listSpan[i].BuildJson(sb);
            }
#else
            for (int i = 0; i < list.Count; i++)
            {
                if (!isFirst)
                    sb.Append(',');
                else
                    isFirst = false;
                 list[i].BuildJson(sb);
            }
#endif
            sb.Append(']');
        }
        internal static JtNodeCollectionSource Create(ICustomSourceParent parent) => new JtNodeCollectionSource(parent);
        internal static JtNodeCollectionSource Create(ICustomSourceParent parent, JToken? source)
        {
            if (source?.Type is JTokenType.String)
            {
                return parent.SourceProvider.GetCustomSource<JtNodeCollectionSource>((string)source!) ?? new JtNodeCollectionSource(parent);
            }
            return new JtNodeCollectionSource(parent, (JArray)source!);

        }

        internal static JtNodeCollectionSource Create(JtNodeCollection instance) => new JtNodeCollectionSource(instance);
        internal JtNodeCollectionSource CreateOverride(ICustomSourceParent parent, JArray? @override) => new JtNodeCollectionSource(parent, this, @override);
        IJtNodeCollectionChild IJtNodeCollectionSourceChild.CreateInstance(IJtNodeParent parent, JToken? @override) => CreateInstance(parent, @override);
        public JtNodeCollection CreateInstance(IJtNodeParent parent, JToken? @override) => new JtNodeCollection(parent, this, @override as JArray);
        IJtNodeCollectionSourceChild IJtNodeCollectionSourceChild.CreateOverride(ICustomSourceParent parent, JToken? @override) => CreateOverride(parent, (JArray?)@override);
        //internal JtNodeCollection CreateInstance(IJtNodeParent parent, JtCustomResourceIdentifier id) => new JtNodeCollection(parent, this, id);
        void IJtNodeCollectionSourceChild.BuildJson(StringBuilder sb) => BuildJson(sb);
    }
}
