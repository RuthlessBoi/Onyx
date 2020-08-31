using System.Collections.Generic;

namespace Onyx.Syntax.Nodes
{
    public sealed class TypeDeclarationSyntax : SyntaxNode
    {
        public override SyntaxType Type => SyntaxType.TypeDelcarationNode;
        public SyntaxToken ColonToken { get; }
        public TypeSyntax InternalType { get; }

        internal TypeDeclarationSyntax(SyntaxTree syntaxTree, SyntaxToken colonToken, TypeSyntax internalType) : base(syntaxTree)
        {
            ColonToken = colonToken;
            InternalType = internalType;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return ColonToken;
            yield return InternalType;
        }
    }
}
