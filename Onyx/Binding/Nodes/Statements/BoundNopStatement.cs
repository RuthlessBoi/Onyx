using Onyx.Syntax;

namespace Onyx.Binding.Nodes.Statements
{
    internal sealed class BoundNopStatement : BoundStatement
    {
        public override BoundNodeType Type => BoundNodeType.NopStatement;

        public BoundNopStatement(SyntaxNode syntax) : base(syntax) { }
    }
}
