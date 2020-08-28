using Onyx.Binding.Symbols;
using Onyx.Syntax;

namespace Onyx.Binding.Nodes
{
    internal sealed class BoundUnaryOperator
    {
        private static readonly BoundUnaryOperator[] operators =
        {
            new BoundUnaryOperator(SyntaxType.BangToken, BoundUnaryOperatorType.LogicalNegation, TypeSymbol.Bool),

            new BoundUnaryOperator(SyntaxType.PlusToken, BoundUnaryOperatorType.Identity, TypeSymbol.Int),
            new BoundUnaryOperator(SyntaxType.MinusToken, BoundUnaryOperatorType.Negation, TypeSymbol.Int),
            new BoundUnaryOperator(SyntaxType.TildeToken, BoundUnaryOperatorType.OnesComplement, TypeSymbol.Int),
        };

        public SyntaxType SyntaxType { get; }
        public BoundUnaryOperatorType Type { get; }
        public TypeSymbol OperandType { get; }
        public TypeSymbol ValueType { get; }

        private BoundUnaryOperator(SyntaxType syntaxType, BoundUnaryOperatorType type, TypeSymbol operandType) : this(syntaxType, type, operandType, operandType) { }
        private BoundUnaryOperator(SyntaxType syntaxType, BoundUnaryOperatorType type, TypeSymbol operandType, TypeSymbol resultType)
        {
            SyntaxType = syntaxType;
            Type = type;
            OperandType = operandType;
            ValueType = resultType;
        }

        public static BoundUnaryOperator? Bind(SyntaxType syntaxKind, TypeSymbol operandType)
        {
            foreach (var op in operators)
            {
                if (op.SyntaxType == syntaxKind && op.OperandType == operandType)
                    return op;
            }

            return null;
        }
    }
}
