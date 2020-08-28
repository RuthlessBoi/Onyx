using Onyx.Binding.Symbols;
using Onyx.Syntax;

namespace Onyx.Binding.Nodes.Expressions
{
    internal sealed class BoundVariableExpression : BoundExpression
    {
        public override BoundNodeType Type => BoundNodeType.VariableExpression;
        public override TypeSymbol ValueType => Variable.ValueType;
        public VariableSymbol Variable { get; }
        public override BoundConstant? ConstantValue => Variable.Constant;

        public BoundVariableExpression(SyntaxNode syntax, VariableSymbol variable) : base(syntax) => Variable = variable;
    }
}
