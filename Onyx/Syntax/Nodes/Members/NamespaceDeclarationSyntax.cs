using System.Collections.Generic;
using System.Collections.Immutable;

namespace Onyx.Syntax.Nodes.Members
{
    public sealed class NamespaceDeclarationSyntax : MemberSyntax
    {
        public override SyntaxType Type => SyntaxType.NamespaceDeclarationNode;
        public SyntaxToken NamespaceKeyword { get; }
        public NamespaceSyntax Identifier { get; }
        public SyntaxToken LeftBraceToken { get; }
        public ImmutableArray<MemberSyntax> Members { get; }
        public SyntaxToken RightBraceToken { get; }

        internal NamespaceDeclarationSyntax(SyntaxTree syntaxTree, SyntaxToken namespaceKeyword, NamespaceSyntax identifier, SyntaxToken leftBraceToken, ImmutableArray<MemberSyntax> members, SyntaxToken rightBraceToken) : base(syntaxTree)
        {
            NamespaceKeyword = namespaceKeyword;
            Identifier = identifier;
            LeftBraceToken = leftBraceToken;
            Members = members;
            RightBraceToken = rightBraceToken;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return NamespaceKeyword;
            yield return Identifier;
            yield return LeftBraceToken;

            foreach (var member in Members)
                yield return member;

            yield return RightBraceToken;
        }
    }
}
