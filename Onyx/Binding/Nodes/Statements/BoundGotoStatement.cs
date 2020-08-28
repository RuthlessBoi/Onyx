using Onyx.Syntax;

namespace Onyx.Binding.Nodes.Statements
{
    internal sealed class BoundGotoStatement : BoundStatement
    {
        public override BoundNodeType Type => BoundNodeType.GotoStatement;
        public BoundLabel Label { get; }

        public BoundGotoStatement(SyntaxNode syntax, BoundLabel label) : base(syntax) => Label = label;
    }
}
