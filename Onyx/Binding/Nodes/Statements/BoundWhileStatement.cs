using Onyx.Syntax;

namespace Onyx.Binding.Nodes.Statements
{
    internal sealed class BoundWhileStatement : BoundLoopStatement
    {
        public override BoundNodeType Type => BoundNodeType.WhileStatement;
        public BoundExpression Condition { get; }
        public BoundStatement Body { get; }

        public BoundWhileStatement(SyntaxNode syntax, BoundExpression condition, BoundStatement body, BoundLabel breakLabel, BoundLabel continueLabel) : base(syntax, breakLabel, continueLabel)
        {
            Condition = condition;
            Body = body;
        }
    }
}
