using Onyx.Syntax;

namespace Onyx.Binding.Nodes.Statements
{
    internal sealed class BoundDoWhileStatement : BoundLoopStatement
    {
        public override BoundNodeType Type => BoundNodeType.DoWhileStatement;
        public BoundStatement Body { get; }
        public BoundExpression Condition { get; }

        public BoundDoWhileStatement(SyntaxNode syntax, BoundStatement body, BoundExpression condition, BoundLabel breakLabel, BoundLabel continueLabel) : base(syntax, breakLabel, continueLabel)
        {
            Body = body;
            Condition = condition;
        }
    }
}
