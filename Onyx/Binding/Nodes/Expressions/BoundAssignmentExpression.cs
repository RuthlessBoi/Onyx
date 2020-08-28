using Onyx.Binding.Symbols;
using Onyx.Syntax;

namespace Onyx.Binding.Nodes.Expressions
{
    internal sealed class BoundAssignmentExpression : BoundExpression
    {
        public override BoundNodeType Type => BoundNodeType.AssignmentExpression;
        public override TypeSymbol ValueType => Expression.ValueType;
        public VariableSymbol Variable { get; }
        public BoundExpression Expression { get; }

        public BoundAssignmentExpression(SyntaxNode syntax, VariableSymbol variable, BoundExpression expression) : base(syntax)
        {
            Variable = variable;
            Expression = expression;
        }
    }
}
