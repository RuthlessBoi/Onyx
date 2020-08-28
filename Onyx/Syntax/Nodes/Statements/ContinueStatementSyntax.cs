using System.Collections.Generic;

namespace Onyx.Syntax.Nodes.Statements
{
    internal sealed class ContinueStatementSyntax : StatementSyntax
    {
        public override SyntaxType Type => SyntaxType.ContinueStatement;
        public SyntaxToken Keyword { get; }

        internal ContinueStatementSyntax(SyntaxTree syntaxTree, SyntaxToken keyword) : base(syntaxTree) => Keyword = keyword;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Keyword;
        }
    }
}
