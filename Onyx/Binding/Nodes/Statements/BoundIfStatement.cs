using Onyx.Syntax;

namespace Onyx.Binding.Nodes.Statements
{
    internal sealed class BoundIfStatement : BoundStatement
    {
        public override BoundNodeType Type => BoundNodeType.IfStatement;
        public BoundExpression Condition { get; }
        public BoundStatement ThenStatement { get; }
        public BoundStatement? ElseStatement { get; }

        public BoundIfStatement(SyntaxNode syntax, BoundExpression condition, BoundStatement thenStatement, BoundStatement? elseStatement) : base(syntax)
        {
            Condition = condition;
            ThenStatement = thenStatement;
            ElseStatement = elseStatement;
        }
    }
}
