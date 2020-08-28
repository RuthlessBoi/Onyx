using System.Collections.Generic;

namespace Onyx.Syntax.Nodes
{
    public sealed class DotExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxType Type => SyntaxType.DotExpression;
        public SyntaxToken LeftToken { get; }
        public SyntaxToken DotToken { get; }
        public ExpressionSyntax RightExpression { get; }

        internal DotExpressionSyntax(SyntaxTree syntaxTree, SyntaxToken leftToken, SyntaxToken dotToken, ExpressionSyntax rightExpression) : base(syntaxTree)
        {
            LeftToken = leftToken;
            DotToken = dotToken;
            RightExpression = rightExpression;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return LeftToken;
            yield return DotToken;
            yield return RightExpression;
        }
    }
}
