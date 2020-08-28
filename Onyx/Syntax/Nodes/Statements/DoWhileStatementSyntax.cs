using System.Collections.Generic;

namespace Onyx.Syntax.Nodes.Statements
{
    public sealed class DoWhileStatementSyntax : StatementSyntax
    {
        public override SyntaxType Type => SyntaxType.DoWhileStatement;
        public SyntaxToken DoKeyword { get; }
        public StatementSyntax Body { get; }
        public SyntaxToken WhileKeyword { get; }
        public ExpressionSyntax Condition { get; }

        internal DoWhileStatementSyntax(SyntaxTree syntaxTree, SyntaxToken doKeyword, StatementSyntax body, SyntaxToken whileKeyword, ExpressionSyntax condition) : base(syntaxTree)
        {
            DoKeyword = doKeyword;
            Body = body;
            WhileKeyword = whileKeyword;
            Condition = condition;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return DoKeyword;
            yield return Body;
            yield return WhileKeyword;
            yield return Condition;
        }
    }
}
