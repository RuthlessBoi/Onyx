using System.Collections.Generic;

namespace Onyx.Syntax.Nodes
{
    public sealed partial class BinaryExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxType Type => SyntaxType.BinaryExpression;
        public ExpressionSyntax Left { get; }
        public SyntaxToken OperatorToken { get; }
        public ExpressionSyntax Right { get; }

        internal BinaryExpressionSyntax(SyntaxTree syntaxTree, ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right) : base(syntaxTree)
        {
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            yield return OperatorToken;
            yield return Right;
        }
    }
}
