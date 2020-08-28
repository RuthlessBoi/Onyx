using Onyx.Syntax;

namespace Onyx.Binding.Nodes.Statements
{
    internal sealed class BoundReturnStatement : BoundStatement
    {
        public override BoundNodeType Type => BoundNodeType.ReturnStatement;
        public BoundExpression? Expression { get; }

        public BoundReturnStatement(SyntaxNode syntax, BoundExpression? expression) : base(syntax) => Expression = expression;
    }
}
