using System.Collections.Generic;

namespace Onyx.Syntax.Nodes.Statements
{
    public sealed class LambdaStatementSyntax : BlockSyntax
    {
        public override SyntaxType Type => SyntaxType.BlockStatement;
        public SyntaxToken EqualsGreaterToken { get; }
        public StatementSyntax Statement { get; }

        internal LambdaStatementSyntax(SyntaxTree syntaxTree, SyntaxToken equalsGreaterToken, StatementSyntax statement) : base(syntaxTree)
        {
            EqualsGreaterToken = equalsGreaterToken;
            Statement = statement;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return EqualsGreaterToken;
            yield return Statement;
        }
    }
}
