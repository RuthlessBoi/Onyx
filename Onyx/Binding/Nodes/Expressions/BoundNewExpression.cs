using Onyx.Binding.Symbols;
using Onyx.Syntax;

namespace Onyx.Binding.Nodes.Expressions
{
    internal sealed class BoundNewExpression : BoundExpression
    {
        public override BoundNodeType Type => BoundNodeType.NewExpression;
        public override TypeSymbol ValueType { get; }
        public override BoundConstant ConstantValue { get; }
        public BoundInitializerExpression Initializer { get; }

        public BoundNewExpression(SyntaxNode syntax, BoundInitializerExpression initializer, TypeSymbol valueType) : base(syntax)
        {
            Initializer = initializer;
            ValueType = valueType;
        }
    }
}
