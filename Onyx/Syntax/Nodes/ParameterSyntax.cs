using System.Collections.Generic;

namespace Onyx.Syntax.Nodes
{
    public sealed partial class ParameterSyntax : SyntaxNode
    {
        public override SyntaxType Type => SyntaxType.ParameterDelcarationNode;
        public SyntaxToken Identifier { get; }
        public TypeDeclarationSyntax TypeDeclaration { get; }

        internal ParameterSyntax(SyntaxTree syntaxTree, SyntaxToken identifier, TypeDeclarationSyntax typeDeclaration) : base(syntaxTree)
        {
            Identifier = identifier;
            TypeDeclaration = typeDeclaration;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Identifier;
            yield return TypeDeclaration;
        }
    }
}
