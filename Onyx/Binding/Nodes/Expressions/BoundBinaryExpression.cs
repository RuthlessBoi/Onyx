using Onyx.Binding.Symbols;
using Onyx.Syntax;

namespace Onyx.Binding.Nodes.Expressions
{
    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public override BoundNodeType Type => BoundNodeType.BinaryExpression;
        public override TypeSymbol ValueType => Op.ValueType;
        public BoundExpression Left { get; }
        public BoundBinaryOperator Op { get; }
        public BoundExpression Right { get; }
        public override BoundConstant? ConstantValue { get; }

        public BoundBinaryExpression(SyntaxNode syntax, BoundExpression left, BoundBinaryOperator op, BoundExpression right) : base(syntax)
        {
            Left = left;
            Op = op;
            Right = right;
            ConstantValue = ConstantFolding.Fold(left, op, right);
        }
    }
}
