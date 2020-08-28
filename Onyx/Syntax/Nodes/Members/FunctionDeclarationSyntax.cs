using Onyx.Syntax.Nodes.Statements;
using System.Collections.Generic;

namespace Onyx.Syntax.Nodes.Members
{
    public sealed class FunctionDeclarationSyntax : MemberSyntax
    {
        public override SyntaxType Type => SyntaxType.FunctionDeclarationNode;
        public SyntaxToken FunctionKeyword { get; }
        public SyntaxToken Identifier { get; }
        public SyntaxToken OpenParenthesisToken { get; }
        public SeparatedSyntaxList<ParameterSyntax> Parameters { get; }
        public SyntaxToken CloseParenthesisToken { get; }
        public TypeDeclarationSyntax? TypeDeclaration { get; }
        public BlockStatementSyntax Body { get; }

        internal FunctionDeclarationSyntax(SyntaxTree syntaxTree, SyntaxToken functionKeyword, SyntaxToken identifier, SyntaxToken openParenthesisToken, SeparatedSyntaxList<ParameterSyntax> parameters, SyntaxToken closeParenthesisToken, TypeDeclarationSyntax? typeDeclaration, BlockStatementSyntax body) : base(syntaxTree)
        {
            FunctionKeyword = functionKeyword;
            Identifier = identifier;
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
