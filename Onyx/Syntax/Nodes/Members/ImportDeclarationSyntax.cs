using System.Collections.Generic;

namespace Onyx.Syntax.Nodes.Members
{
    public sealed class ImportDeclarationSyntax : MemberSyntax
    {
        public override SyntaxType Type => SyntaxType.FunctionDeclarationNode;
        public SyntaxToken ImportKeyword { get; }
        public NamespaceSyntax Identifier { get; }

        internal ImportDeclarationSyntax(SyntaxTree syntaxTree, SyntaxToken importKeyword, NamespaceSyntax identifier) : base(syntaxTree)
        {
            ImportKeyword = importKeyword;
            Identifier = identifier;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return ImportKeyword;
            yield return Identifier;
        }
    }
}
