namespace Onyx.Syntax.Nodes.Members
{
    public abstract class AbstractDeclarationSyntax : MemberSyntax
    {
        public SyntaxToken AbstractKeyword { get; }

        internal AbstractDeclarationSyntax(SyntaxTree syntaxTree, SyntaxToken abstractKeyword) : base(syntaxTree) => AbstractKeyword = abstractKeyword;
    }
}
