using Onyx.Binding.Symbols;
using Onyx.Syntax;

namespace Onyx.Binding.Nodes.Expressions
{
    internal sealed class BoundCompoundAssignmentExpression : BoundExpression
    {
        public BoundCompoundAssignmentExpression(SyntaxNode syntax, VariableSymbol variable, BoundBinaryOperator op, BoundExpression expression) : base(syntax)
        {
            Variable = variable;
            Op = op;
            Expression = expression;
        }

        public override BoundNodeType Type => BoundNodeType.CompoundAssignmentExpression;
        public override TypeSymbol ValueType => Expression.ValueType;
        public VariableSymbol Variable { get; }
        public BoundBinaryOperator Op { get; }
        public BoundExpression Expression { get; }
    }
}
