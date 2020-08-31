using System.Collections.Generic;
using System.Collections.Immutable;

namespace Onyx.Syntax.Nodes.Statements
{
    public sealed class NamespaceBlockSyntax : StatementSyntax
    {
        public override SyntaxType Type => SyntaxType.NamespaceBlockStatement;
        public SyntaxToken LeftBraceToken { get; }
        public ImmutableArray<MemberSyntax> Members { get; }
        public SyntaxToken RightBraceToken { get; }

        internal NamespaceBlockSyntax(SyntaxTree syntaxTree, SyntaxToken leftBraceToken, ImmutableArray<MemberSyntax> members, SyntaxToken closeBraceToken) : base(syntaxTree)
        {
            LeftBraceToken = leftBraceToken;
            Members = members;
            RightBraceToken = closeBraceToken;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return LeftBraceToken;

            foreach (var member in Members)
                yield return member;

            yield return RightBraceToken;
        }
    }
}
