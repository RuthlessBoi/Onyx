using System.Collections.Generic;

namespace Onyx.Syntax.Nodes.Statements
{
    public sealed class ReturnStatementSyntax : StatementSyntax
    {
        public override SyntaxType Type => SyntaxType.ReturnStatement;
        public SyntaxToken ReturnKeyword { get; }
        public ExpressionSyntax? Expression { get; }

        internal ReturnStatementSyntax(SyntaxTree syntaxTree, SyntaxToken returnKeyword, ExpressionSyntax? expression) : base(syntaxTree)
        {
            ReturnKeyword = returnKeyword;
            Expression = expression;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return ReturnKeyword;
            yield return Expression;
        }
    }
}
