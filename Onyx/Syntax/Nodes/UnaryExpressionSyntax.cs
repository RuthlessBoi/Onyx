using System.Collections.Generic;

namespace Onyx.Syntax.Nodes
{
    public sealed partial class UnaryExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxType Type => SyntaxType.UnaryExpression;
        public SyntaxToken OperatorToken { get; }
        public ExpressionSyntax Operand { get; }

        internal UnaryExpressionSyntax(SyntaxTree syntaxTree, SyntaxToken operatorToken, ExpressionSyntax operand) : base(syntaxTree)
        {
            OperatorToken = operatorToken;
            Operand = operand;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OperatorToken;
            yield return Operand;
        }
    }
}
