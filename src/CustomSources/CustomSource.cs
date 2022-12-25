using System.Diagnostics;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public abstract class CustomSource : ICustomSourceParent
    {
        private readonly ICustomSourceDeclaration? declaration;

        public CustomSource? Parent { get; }
        public ICustomSourceDeclaration Declaration => (declaration ?? Parent?.Declaration)!;
        public ICustomSourceProvider SourceProvider => Declaration.SourceProvider;
        private protected CustomSource(ICustomSourceParent parent)
        {
            //SourceProvider = sourceProvider;
            if (parent is ICustomSourceDeclaration declaration)
            {
                if (declaration.IsDeclaratingSource)
#if NET7_0_OR_GREATER
                    throw new UnreachableException();
#else
                    throw new InternalException("Declaration cannot declarate multiple custom sources");
#endif
                this.declaration = declaration;
            }
            else if (parent is CustomSource customSource)
                Parent = customSource;
        }
        
        public bool IsDeclarated => declaration != null;

        internal virtual void BuildJson(StringBuilder sb)
        {
            if (IsDeclarated)
                Declaration.BuildJson(sb);
            else
                BuildJsonDeclaration(sb);
        }
        internal abstract void BuildJsonDeclaration(StringBuilder sb);
    }
}
