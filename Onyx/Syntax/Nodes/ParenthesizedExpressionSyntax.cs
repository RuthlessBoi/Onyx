using System.Collections.Generic;

namespace Onyx.Syntax.Nodes
{
    public sealed partial class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxType Type => SyntaxType.ParenthesizedExpression;
        public SyntaxToken OpenParenthesisToken { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken CloseParenthesisToken { get; }

        internal ParenthesizedExpressionSyntax(SyntaxTree syntaxTree, SyntaxToken openParenthesisToken, ExpressionSyntax expression, SyntaxToken closeParenthesisToken) : base(syntaxTree)
        {
            OpenParenthesisToken = openParenthesisToken;
            Expression = expression;
            CloseParenthesisToken = closeParenthesisToken;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpenParenthesisToken;
            yield return Expression;
            yield return CloseParenthesisToken;
        }
    }
}
