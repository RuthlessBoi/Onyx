using System.Collections.Generic;
using System.Collections.Immutable;

namespace Onyx.Syntax.Nodes.Statements
{
    public sealed class BlockStatementSyntax : BlockSyntax
    {
        public override SyntaxType Type => SyntaxType.BlockStatement;
        public SyntaxToken OpenBraceToken { get; }
        public ImmutableArray<StatementSyntax> Statements { get; }
        public SyntaxToken CloseBraceToken { get; }

        internal BlockStatementSyntax(SyntaxTree syntaxTree, SyntaxToken openBraceToken, ImmutableArray<StatementSyntax> statements, SyntaxToken closeBraceToken) : base(syntaxTree)
        {
            OpenBraceToken = openBraceToken;
            Statements = statements;
            CloseBraceToken = closeBraceToken;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpenBraceToken;

            foreach (var statement in Statements)
                yield return statement;

            yield return CloseBraceToken;
        }
    }
}
