using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public abstract class CustomSource : IJtCustomSourceParent
    {
        private readonly IJtCustomSourceDeclaration? declaration;
        private IdentifiersManager? identifiersManager;
        [Browsable(false)]
        public IJtCustomSourceParent? Parent { get; }
        [Browsable(false)] public IJtCustomSourceDeclaration Declaration => (declaration ?? Parent?.Declaration)!;
        [Browsable(false)] public ICustomSourceProvider SourceProvider => Declaration.SourceProvider;
        private protected CustomSource(IJtCustomSourceParent parent)
        {
            if (parent is IJtCustomSourceDeclaration declaration)
            {
                if (declaration.IsDeclaringSource)
                    throw new UnreachableException();

                this.declaration = declaration;
            }
            else if (parent is CustomSource customSource)
                Parent = customSource;
        }

        [Browsable(false)] public bool IsDeclared => declaration is not null;

        public virtual IJtCustomSourceDeclaration? Base => IsDeclared ? Declaration : null;

        [Browsable(false)] public abstract bool IsExternal { get; }
        [Browsable(false)] public bool IsRoot => IsDeclared;
        [MemberNotNull(nameof(identifiersManager))]
        [DefaultValue(false)]
        public IIdentifiersManager IdentifiersManager => identifiersManager ??= new IdentifiersManager(null);


        internal virtual void BuildJson(StringBuilder sb)
        {
            if (IsDeclared)
                Declaration.BuildJson(sb);
            else
                BuildJsonDeclaration(sb);
        }
        internal abstract void BuildJsonDeclaration(StringBuilder sb);
    }
}
