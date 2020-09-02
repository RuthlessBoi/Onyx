using Onyx.Binding.Nodes;
using Onyx.Binding.Symbols;
using System;

namespace Onyx.Binding
{
    internal static class ConstantFolding
    {
        public static BoundConstant? Fold(BoundUnaryOperator op, BoundExpression operand)
        {
            if (operand.ConstantValue != null)
            {
                switch (op.Type)
                {
                    case BoundUnaryOperatorType.Identity:
                        return new BoundConstant((int)operand.ConstantValue.Value, TypeSymbol.Int);
                    case BoundUnaryOperatorType.Negation:
                        return new BoundConstant(-(int)operand.ConstantValue.Value, TypeSymbol.Int);
                    case BoundUnaryOperatorType.LogicalNegation:
                        return new BoundConstant(!(bool)operand.ConstantValue.Value, TypeSymbol.Int);
                    case BoundUnaryOperatorType.OnesComplement:
                        return new BoundConstant(~(int)operand.ConstantValue.Value, TypeSymbol.Int);
                    default:
                        throw new Exception($"Unexpected unary operator {op.Type}");
                }
            }

            return null;
        }

        public static BoundConstant? Fold(BoundExpression left, BoundBinaryOperator op, BoundExpression right)
        {
            var leftConstant = left.ConstantValue;
            var rightConstant = right.ConstantValue;

            // Special case && and || because there are cases where only one
            // side needs to be known.

            if (op.Type == BoundBinaryOperatorType.LogicalAnd)
            {
                if (leftConstant != null && !(bool)leftConstant.Value ||
                    rightConstant != null && !(bool)rightConstant.Value)
                {
                    return new BoundConstant(false, TypeSymbol.Bool);
                }
            }

            if (op.Type == BoundBinaryOperatorType.LogicalOr)
            {
                if (leftConstant != null && (bool)leftConstant.Value ||
                    rightConstant != null && (bool)rightConstant.Value)
                {
                    return new BoundConstant(true, TypeSymbol.Bool);
                }
            }

            if (leftConstant == null || rightConstant == null)
                return null;

            var l = leftConstant.Value;
            var r = rightConstant.Value;

            switch (op.Type)
            {
                case BoundBinaryOperatorType.Addition:
                    if (left.ValueType == TypeSymbol.Int)
                        return new BoundConstant((int)l + (int)r, TypeSymbol.Int);
                    else
                        return new BoundConstant((string)l + (string)r, TypeSymbol.String);
                case BoundBinaryOperatorType.Subtraction:
                    return new BoundConstant((int)l - (int)r, TypeSymbol.Int);
                case BoundBinaryOperatorType.Multiplication:
                    return new BoundConstant((int)l * (int)r, TypeSymbol.Int);
                case BoundBinaryOperatorType.Division:
                    return new BoundConstant((int)l / (int)r, TypeSymbol.Int);
                case BoundBinaryOperatorType.BitwiseAnd:
                    if (left.ValueType == TypeSymbol.Int)
                        return new BoundConstant((int)l & (int)r, TypeSymbol.Int);
                    else
                        return new BoundConstant((bool)l & (bool)r, TypeSymbol.Bool);
                case BoundBinaryOperatorType.BitwiseOr:
                    if (left.ValueType == TypeSymbol.Int)
                        return new BoundConstant((int)l | (int)r, TypeSymbol.Int);
                    else
                        return new BoundConstant((bool)l | (bool)r, TypeSymbol.Bool);
                case BoundBinaryOperatorType.BitwiseXor:
                    if (left.ValueType == TypeSymbol.Int)
                        return new BoundConstant((int)l ^ (int)r, TypeSymbol.Int);
                    else
                        return new BoundConstant((bool)l ^ (bool)r, TypeSymbol.Bool);
                case BoundBinaryOperatorType.LogicalAnd:
                    return new BoundConstant((bool)l && (bool)r, TypeSymbol.Bool);
                case BoundBinaryOperatorType.LogicalOr:
                    return new BoundConstant((bool)l || (bool)r, TypeSymbol.Bool);
                case BoundBinaryOperatorType.Equals:
                    return new BoundConstant(Equals(l, r), TypeSymbol.Bool);
                case BoundBinaryOperatorType.NotEquals:
                    return new BoundConstant(!Equals(l, r), TypeSymbol.Bool);
                case BoundBinaryOperatorType.Less:
                    return new BoundConstant((int)l < (int)r, TypeSymbol.Int);
                case BoundBinaryOperatorType.LessOrEquals:
                    return new BoundConstant((int)l <= (int)r, TypeSymbol.Int);
                case BoundBinaryOperatorType.Greater:
                    return new BoundConstant((int)l > (int)r, TypeSymbol.Int);
                case BoundBinaryOperatorType.GreaterOrEquals:
                    return new BoundConstant((int)l >= (int)r, TypeSymbol.Int);
                default:
                    throw new Exception($"Unexpected binary operator {op.Type}");
            }
        }
    }
}
