using System.Collections.Immutable;

namespace Onyx.Syntax
{
    public abstract class SeparatedSyntaxList
    {
        private protected SeparatedSyntaxList() { }

        public abstract ImmutableArray<SyntaxNode> GetWithSeparators();
    }
}
