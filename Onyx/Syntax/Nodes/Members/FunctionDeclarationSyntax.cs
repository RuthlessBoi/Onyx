using Onyx.Syntax.Nodes.Statements;
using System.Collections.Generic;

namespace Onyx.Syntax.Nodes.Members
{
    public sealed class FunctionDeclarationSyntax : MemberSyntax
    {
        public override SyntaxType Type => SyntaxType.FunctionDeclarationNode;
        public SyntaxToken FunctionKeyword { get; }
        public SyntaxToken Identifier { get; }
        public GenericsDeclarationSyntax? GenericsDeclaration { get; }
        public SyntaxToken OpenParenthesisToken { get; }
        public SeparatedSyntaxList<ParameterSyntax> Parameters { get; }
        public SyntaxToken CloseParenthesisToken { get; }
        public TypeDeclarationSyntax? TypeDeclaration { get; }
        public BlockSyntax Body { get; }

        internal FunctionDeclarationSyntax(SyntaxTree syntaxTree, SyntaxToken functionKeyword, SyntaxToken identifier, GenericsDeclarationSyntax? genericsDeclaration, SyntaxToken openParenthesisToken, SeparatedSyntaxList<ParameterSyntax> parameters, SyntaxToken closeParenthesisToken, TypeDeclarationSyntax? typeDeclaration, BlockSyntax body) : base(syntaxTree)
        {
            FunctionKeyword = functionKeyword;
            Identifier = identifier;
            GenericsDeclaration = genericsDeclaration;
            OpenParenthesisToken = openParenthesisToken;
            Parameters = parameters;
            CloseParenthesisToken = closeParenthesisToken;
            TypeDeclaration = typeDeclaration;
            Body = body;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
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

            yield return Body;
        }
    }
}
