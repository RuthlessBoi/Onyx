using Onyx.Syntax;

namespace Onyx.Binding.Nodes.Statements
{
    internal sealed class BoundConditionalGotoStatement : BoundStatement
    {
        public override BoundNodeType Type => BoundNodeType.ConditionalGotoStatement;
        public BoundLabel Label { get; }
        public BoundExpression Condition { get; }
        public bool JumpIfTrue { get; }

        public BoundConditionalGotoStatement(SyntaxNode syntax, BoundLabel label, BoundExpression condition, bool jumpIfTrue = true) : base(syntax)
        {
            Label = label;
            Condition = condition;
            JumpIfTrue = jumpIfTrue;
        }
    }
}
