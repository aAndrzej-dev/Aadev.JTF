using System.Text;

namespace Aadev.JTF.CustomSources
{
    public abstract class CustomSource : ICustomSourceParent
    {
        private readonly ICustomSourceDeclaration? declaration;

        public CustomSource? Parent { get; }
        public ICustomSourceDeclaration Declaration => (declaration ?? Parent?.Declaration)!;
        public ICustomSourceProvider? SourceProvider { get; }
        protected internal CustomSource(ICustomSourceParent parent, ICustomSourceProvider? sourceProvider)
        {
            SourceProvider = sourceProvider;
            if (parent is ICustomSourceDeclaration declaration)
            {
                if (declaration.IsDeclaratingSource)
                    throw new InternalException("Declaration cannot declarate multiple custom sources");
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
