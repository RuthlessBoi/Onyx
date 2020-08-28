using Onyx.Binding.Symbols;
using Onyx.Syntax;

namespace Onyx.Binding.Nodes
{
    internal sealed class BoundBinaryOperator
    {
        private static readonly BoundBinaryOperator[] operators =
        {
            new BoundBinaryOperator(SyntaxType.PlusToken, BoundBinaryOperatorType.Addition, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxType.MinusToken, BoundBinaryOperatorType.Subtraction, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxType.StarToken, BoundBinaryOperatorType.Multiplication, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxType.SlashToken, BoundBinaryOperatorType.Division, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxType.AmpersandToken, BoundBinaryOperatorType.BitwiseAnd, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxType.PipeToken, BoundBinaryOperatorType.BitwiseOr, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxType.HatToken, BoundBinaryOperatorType.BitwiseXor, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxType.EqualsEqualsToken, BoundBinaryOperatorType.Equals, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.BangEqualsToken, BoundBinaryOperatorType.NotEquals, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.LessToken, BoundBinaryOperatorType.Less, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.LessOrEqualsToken, BoundBinaryOperatorType.LessOrEquals, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.GreaterToken, BoundBinaryOperatorType.Greater, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.GreaterOrEqualsToken, BoundBinaryOperatorType.GreaterOrEquals, TypeSymbol.Int, TypeSymbol.Bool),

            new BoundBinaryOperator(SyntaxType.AmpersandToken, BoundBinaryOperatorType.BitwiseAnd, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.AmpersandAmpersandToken, BoundBinaryOperatorType.LogicalAnd, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.PipeToken, BoundBinaryOperatorType.BitwiseOr, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.PipePipeToken, BoundBinaryOperatorType.LogicalOr, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.HatToken, BoundBinaryOperatorType.BitwiseXor, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.EqualsEqualsToken, BoundBinaryOperatorType.Equals, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.BangEqualsToken, BoundBinaryOperatorType.NotEquals, TypeSymbol.Bool),

            new BoundBinaryOperator(SyntaxType.PlusToken, BoundBinaryOperatorType.Addition, TypeSymbol.String),
            new BoundBinaryOperator(SyntaxType.EqualsEqualsToken, BoundBinaryOperatorType.Equals, TypeSymbol.String, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.BangEqualsToken, BoundBinaryOperatorType.NotEquals, TypeSymbol.String, TypeSymbol.Bool),

            new BoundBinaryOperator(SyntaxType.EqualsEqualsToken, BoundBinaryOperatorType.Equals, TypeSymbol.Any),
            new BoundBinaryOperator(SyntaxType.BangEqualsToken, BoundBinaryOperatorType.NotEquals, TypeSymbol.Any),
            new BoundBinaryOperator(SyntaxType.IsKeyword, BoundBinaryOperatorType.Is, TypeSymbol.Any, TypeSymbol.InternalType, TypeSymbol.Bool),
        };

        public SyntaxType SyntaxType { get; }
        public BoundBinaryOperatorType Type { get; }
        public TypeSymbol LeftType { get; }
        public TypeSymbol RightType { get; }
        public TypeSymbol ValueType { get; }

        private BoundBinaryOperator(SyntaxType syntaxType, BoundBinaryOperatorType type, TypeSymbol valueType) : this(syntaxType, type, valueType, valueType, valueType) { }
        private BoundBinaryOperator(SyntaxType syntaxType, BoundBinaryOperatorType type, TypeSymbol operandType, TypeSymbol resultType) : this(syntaxType, type, operandType, operandType, resultType) { }
        private BoundBinaryOperator(SyntaxType syntaxType, BoundBinaryOperatorType type, TypeSymbol leftType, TypeSymbol rightType, TypeSymbol resultType)
        {
            SyntaxType = syntaxType;
            Type = type;
            LeftType = leftType;
            RightType = rightType;
            ValueType = resultType;
        }

        public static BoundBinaryOperator? Bind(SyntaxType syntaxType, TypeSymbol leftType, TypeSymbol rightType)
        {
            foreach (var op in operators)
            {
                if (op.SyntaxType == syntaxType && op.LeftType == leftType && op.RightType == rightType)
                    return op;
            }

            return null;
        }
    }
}
