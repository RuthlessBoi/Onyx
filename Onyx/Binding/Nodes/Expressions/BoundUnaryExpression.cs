using Onyx.Binding.Symbols;
using Onyx.Syntax;

namespace Onyx.Binding.Nodes.Expressions
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public override BoundNodeType Type => BoundNodeType.UnaryExpression;
        public override TypeSymbol ValueType => Op.ValueType;
        public BoundUnaryOperator Op { get; }
        public BoundExpression Operand { get; }
        public override BoundConstant? ConstantValue { get; }

        public BoundUnaryExpression(SyntaxNode syntax, BoundUnaryOperator op, BoundExpression operand) : base(syntax)
        {
            Op = op;
            Operand = operand;
            ConstantValue = ConstantFolding.Fold(op, operand);
        }
    }
}
