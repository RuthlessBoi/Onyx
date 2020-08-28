using System.Collections.Generic;

namespace Onyx.Syntax.Nodes
{
    public sealed partial class VariableDeclarationSyntax : StatementSyntax
    {
        public override SyntaxType Type => SyntaxType.VariableDeclarationStatement;
        public SyntaxToken Keyword { get; }
        public SyntaxToken Identifier { get; }
        public TypeDeclarationSyntax? TypeDeclaration { get; }
        public SyntaxToken EqualsToken { get; }
        public ExpressionSyntax Initializer { get; }

        internal VariableDeclarationSyntax(SyntaxTree syntaxTree, SyntaxToken keyword, SyntaxToken identifier, TypeDeclarationSyntax? typeDeclaration, SyntaxToken equalsToken, ExpressionSyntax initializer) : base(syntaxTree)
        {
            Keyword = keyword;
            Identifier = identifier;
            TypeDeclaration = typeDeclaration;
            EqualsToken = equalsToken;
            Initializer = initializer;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Keyword;
            yield return Identifier;

            if (TypeDeclaration != null)
                yield return TypeDeclaration;

            yield return EqualsToken;
            yield return Initializer;
        }
    }
}
