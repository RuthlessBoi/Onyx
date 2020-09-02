using Onyx.Syntax.Nodes.Statements;
using System.Collections.Generic;

namespace Onyx.Syntax.Nodes.Members
{
    public sealed class ConstructorDeclarationSyntax : MemberSyntax
    {
        public override SyntaxType Type => SyntaxType.ConstructorDeclarationNode;
        public SyntaxToken ConstructorKeyword { get; }
        public SyntaxToken LeftParenthesisToken { get; }
        public SeparatedSyntaxList<ParameterSyntax> Parameters { get; }
        public SyntaxToken RightParenthesisToken { get; }
        public BlockSyntax Body { get; }

        internal ConstructorDeclarationSyntax(SyntaxTree syntaxTree, SyntaxToken constructorKeyword, SyntaxToken leftParenthesisToken, SeparatedSyntaxList<ParameterSyntax> parameters, SyntaxToken rightParenthesisToken, BlockSyntax body) : base(syntaxTree)
        {
            ConstructorKeyword = constructorKeyword;
            LeftParenthesisToken = leftParenthesisToken;
            Parameters = parameters;
            RightParenthesisToken = rightParenthesisToken;
            Body = body;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return ConstructorKeyword;

            yield return LeftParenthesisToken;

            foreach (var child in Parameters.GetWithSeparators())
                yield return child;

            yield return RightParenthesisToken;
            yield return Body;
        }
    }
}
