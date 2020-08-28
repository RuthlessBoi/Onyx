using System.Collections.Generic;

namespace Onyx.Syntax.Nodes.Statements
{
    internal sealed class BreakStatementSyntax : StatementSyntax
    {
        public override SyntaxType Type => SyntaxType.BreakStatement;
        public SyntaxToken Keyword { get; }

        internal BreakStatementSyntax(SyntaxTree syntaxTree, SyntaxToken keyword) : base(syntaxTree) => Keyword = keyword;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Keyword;
        }
    }
}
