using Onyx.Syntax;

namespace Onyx.Binding.Nodes.Statements
{
    internal sealed class BoundExpressionStatement : BoundStatement
    {
        public override BoundNodeType Type => BoundNodeType.ExpressionStatement;
        public BoundExpression Expression { get; }

        public BoundExpressionStatement(SyntaxNode syntax, BoundExpression expression) : base(syntax) => Expression = expression;
    }
}
