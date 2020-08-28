using Onyx.Syntax;

namespace Onyx.Binding.Nodes
{
    internal abstract class BoundStatement : BoundNode
    {
        protected BoundStatement(SyntaxNode syntax) : base(syntax) { }
    }
}
