using System.Collections.Generic;

namespace Onyx.Syntax.Nodes.Members
{
    public sealed class GlobalStatementSyntax : MemberSyntax
    {
        public override SyntaxType Type => SyntaxType.GlobalStatementDelcarationNode;
        public StatementSyntax Statement { get; }

        internal GlobalStatementSyntax(SyntaxTree syntaxTree, StatementSyntax statement) : base(syntaxTree) => Statement = statement;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Statement;
        }
    }
}
