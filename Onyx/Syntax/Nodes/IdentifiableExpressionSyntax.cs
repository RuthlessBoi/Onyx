namespace Onyx.Syntax.Nodes
{
    public abstract class IdentifiableExpressionSyntax : ExpressionSyntax
    {
        public IdentifiableExpressionSyntax? Child { get; }
        public SyntaxToken IdentifierToken { get; }

        public IdentifiableExpressionSyntax(SyntaxTree syntaxTree, SyntaxToken identifierToken, IdentifiableExpressionSyntax? child) : base(syntaxTree)
        {
            Child = child;
            IdentifierToken = identifierToken;
        }
    }
}
