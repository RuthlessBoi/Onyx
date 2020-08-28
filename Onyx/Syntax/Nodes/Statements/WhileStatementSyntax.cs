using System.Collections.Generic;

namespace Onyx.Syntax.Nodes.Statements
{
    public sealed class WhileStatementSyntax : StatementSyntax
    {
        public override SyntaxType Type => SyntaxType.WhileStatement;
        public SyntaxToken WhileKeyword { get; }
        public ExpressionSyntax Condition { get; }
        public StatementSyntax Body { get; }

        internal WhileStatementSyntax(SyntaxTree syntaxTree, SyntaxToken whileKeyword, ExpressionSyntax condition, StatementSyntax body) : base(syntaxTree)
        {
            WhileKeyword = whileKeyword;
            Condition = condition;
            Body = body;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return WhileKeyword;
            yield return Condition;
            yield return Body;
        }
    }
}
