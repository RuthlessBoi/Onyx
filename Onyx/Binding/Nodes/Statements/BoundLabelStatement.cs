using Onyx.Syntax;

namespace Onyx.Binding.Nodes.Statements
{
    internal sealed class BoundLabelStatement : BoundStatement
    {
        public override BoundNodeType Type => BoundNodeType.LabelStatement;
        public BoundLabel Label { get; }

        public BoundLabelStatement(SyntaxNode syntax, BoundLabel label) : base(syntax) => Label = label;
    }
}
