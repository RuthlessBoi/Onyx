using Onyx.Binding.Symbols;
using Onyx.Syntax;

namespace Onyx.Binding.Nodes.Expressions
{
    internal sealed class BoundErrorExpression : BoundExpression
    {
        public override BoundNodeType Type => BoundNodeType.ErrorExpression;
        public override TypeSymbol ValueType => TypeSymbol.Error;

        public BoundErrorExpression(SyntaxNode syntax) : base(syntax) { }
    }
}
