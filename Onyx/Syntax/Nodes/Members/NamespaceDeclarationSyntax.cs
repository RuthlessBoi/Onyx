using Onyx.Syntax.Nodes.Statements;
using System.Collections.Generic;

namespace Onyx.Syntax.Nodes.Members
{
    public sealed class NamespaceDeclarationSyntax : MemberSyntax
    {
        public override SyntaxType Type => SyntaxType.NamespaceDeclarationNode;
        public SyntaxToken NamespaceKeyword { get; }
        public NamespaceSyntax Identifier { get; }
        public NamespaceBlockSyntax NamespaceBlock { get; }

        internal NamespaceDeclarationSyntax(SyntaxTree syntaxTree, SyntaxToken namespaceKeyword, NamespaceSyntax identifier, NamespaceBlockSyntax namespaceBlock) : base(syntaxTree)
        {
            NamespaceKeyword = namespaceKeyword;
            Identifier = identifier;
            NamespaceBlock = namespaceBlock;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return NamespaceKeyword;
            yield return Identifier;
            yield return NamespaceBlock;
        }
    }
}
