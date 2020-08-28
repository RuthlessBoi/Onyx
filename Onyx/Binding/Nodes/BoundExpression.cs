using Onyx.Binding.Symbols;
using Onyx.Syntax;

namespace Onyx.Binding.Nodes
{
    internal abstract class BoundExpression : BoundNode
    {
        public abstract TypeSymbol ValueType { get; }
        public virtual BoundConstant? ConstantValue => null;

        protected BoundExpression(SyntaxNode syntax) : base(syntax) { }
    }
}
