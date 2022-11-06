using Aadev.JTF.JtEnumerable;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtNodeCollectionSource : CustomSource, IJtNodeCollectionSourceChild
    {
        internal readonly IJtEnumerable<IJtNodeCollectionSourceChild> nodeEnumerable;

        private JtNodeCollectionSource(ICustomSourceParent parent, ICustomSourceProvider sourceProvider) : base(parent, sourceProvider)
        {
            nodeEnumerable = JtEnumerable.JtEnumerable.CreateEmpty<IJtNodeCollectionSourceChild>();
        }

        private JtNodeCollectionSource(JtNodeCollection instance, ICustomSourceProvider sourceProvider) : base(new CustomSourceFormInstanceDeclaration(instance.Owner), sourceProvider)
        {
            nodeEnumerable = JtEnumerable.JtEnumerable.CreateJtNodeSourceCollection(instance, sourceProvider);
        }

        private JtNodeCollectionSource(ICustomSourceParent parent, JtNodeCollectionSource @base, JArray? @override, ICustomSourceProvider sourceProvider) : base(parent, sourceProvider)
        {
            nodeEnumerable = JtEnumerable.JtEnumerable.CreateJtNodeSourceCollection(this, @base, @override, sourceProvider);
        }

        private JtNodeCollectionSource(ICustomSourceParent parent, JArray jArray, ICustomSourceProvider sourceProvider) : base(parent, sourceProvider)
        {
            nodeEnumerable = JtEnumerable.JtEnumerable.CreateJtNodeSourceCollection(this, jArray, sourceProvider);
        }

        internal override void BuildJsonDeclaration(StringBuilder sb)
        {
            sb.Append('[');
            bool isFirst = true;
            foreach (IJtNodeCollectionSourceChild item in nodeEnumerable.Enumerate())
            {
                if (!isFirst)
                    sb.Append(',');
                else
                    isFirst = false;
                item.BuildJson(sb);
            }
            sb.Append(']');
        }

        internal static JtNodeCollectionSource Create(ICustomSourceParent parent, JToken? source, ICustomSourceProvider sourceProvider)
        {
            if (source?.Type is JTokenType.String)
            {
                return sourceProvider.GetCustomSource<JtNodeCollectionSource>((string)source!) ?? new JtNodeCollectionSource(parent, sourceProvider);
            }
            return new JtNodeCollectionSource(parent, (JArray)source!, sourceProvider);

        }

        internal static JtNodeCollectionSource Create(JtNodeCollection instance, ICustomSourceProvider sourceProvider) => new JtNodeCollectionSource(instance, sourceProvider);
        internal JtNodeCollectionSource CreateOverride(ICustomSourceParent parent, JArray? @override) => new JtNodeCollectionSource(parent, this, @override, SourceProvider!);
        IJtNodeCollectionChild IJtNodeCollectionSourceChild.CreateInstance(IJtNodeParent parent, JToken? @override) => CreateInstance(parent, @override);
        public JtNodeCollection CreateInstance(IJtNodeParent parent, JToken? @override) => new JtNodeCollection(parent, this, @override as JArray, SourceProvider!);
        internal JtNodeCollection CreateInstance(IJtNodeParent parent, JtCustomResourceIdentifier id) => new JtNodeCollection(parent, this, id, SourceProvider!);
        void IJtNodeCollectionSourceChild.BuildJson(StringBuilder sb) => BuildJson(sb);
    }
}
