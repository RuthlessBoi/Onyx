using System.Collections.Generic;

namespace Onyx.Syntax.Nodes
{
    public sealed class TemplateParameterSyntax : ExpressionSyntax
    {
        public override SyntaxType Type => SyntaxType.ModelParameterDeclarationNode;
        public SyntaxToken? ReadOnlyToken { get; }
        public SyntaxToken Identifier { get; }
        public TypeDeclarationSyntax TypeDeclaration { get; }

        internal TemplateParameterSyntax(SyntaxTree syntaxTree, SyntaxToken? readOnlyToken, SyntaxToken identifier, TypeDeclarationSyntax typeDeclaration) : base(syntaxTree)
        {
            ReadOnlyToken = readOnlyToken;
            Identifier = identifier;
            TypeDeclaration = typeDeclaration;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            if (ReadOnlyToken != null)
                yield return ReadOnlyToken;

            yield return Identifier;
            yield return TypeDeclaration;
        }
    }
}
