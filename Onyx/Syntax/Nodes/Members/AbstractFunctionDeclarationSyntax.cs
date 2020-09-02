using System.Collections.Generic;

namespace Onyx.Syntax.Nodes.Members
{
    public sealed class AbstractFunctionDeclarationSyntax : AbstractDeclarationSyntax
    {
        public override SyntaxType Type => SyntaxType.AbstractFunctionDeclarationNode;
        public SyntaxToken FunctionKeyword { get; }
        public SyntaxToken Identifier { get; }
        public GenericsDeclarationSyntax? GenericsDeclaration { get; }
        public SyntaxToken OpenParenthesisToken { get; }
        public SeparatedSyntaxList<ParameterSyntax> Parameters { get; }
        public SyntaxToken CloseParenthesisToken { get; }
        public TypeDeclarationSyntax? TypeDeclaration { get; }

        internal AbstractFunctionDeclarationSyntax(SyntaxTree syntaxTree, SyntaxToken abstractKeyword, SyntaxToken functionKeyword, SyntaxToken identifier, GenericsDeclarationSyntax? genericsDeclaration, SyntaxToken openParenthesisToken, SeparatedSyntaxList<ParameterSyntax> parameters, SyntaxToken closeParenthesisToken, TypeDeclarationSyntax? typeDeclaration) : base(syntaxTree, abstractKeyword)
        {
            FunctionKeyword = functionKeyword;
            Identifier = identifier;
            GenericsDeclaration = genericsDeclaration;
            OpenParenthesisToken = openParenthesisToken;
            Parameters = parameters;
            CloseParenthesisToken = closeParenthesisToken;
            TypeDeclaration = typeDeclaration;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return AbstractKeyword;
            yield return FunctionKeyword;
            yield return Identifier;

            if (GenericsDeclaration != null)
                yield return GenericsDeclaration;

            yield return OpenParenthesisToken;

            foreach (var child in Parameters.GetWithSeparators())
                yield return child;

            yield return CloseParenthesisToken;

            if (TypeDeclaration != null)
                yield return TypeDeclaration;
        }
    }
}
