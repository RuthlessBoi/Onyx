using Onyx.Binding.Symbols;
using Onyx.Syntax;

namespace Onyx.Binding.Nodes.Expressions
{
    internal sealed class BoundConversionExpression : BoundExpression
    {
        public override BoundNodeType Type => BoundNodeType.ConversionExpression;
        public override TypeSymbol ValueType { get; }
        public BoundExpression Expression { get; }

        public BoundConversionExpression(SyntaxNode syntax, TypeSymbol type, BoundExpression expression) : base(syntax)
        {
            ValueType = type;
            Expression = expression;
        }
    }
}
