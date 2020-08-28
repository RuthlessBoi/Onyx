using System.Collections.Generic;

namespace Onyx.Syntax.Nodes
{
    public sealed class TypeDeclarationSyntax : SyntaxNode
    {
        public override SyntaxType Type => SyntaxType.TypeDelcarationNode;
        public SyntaxToken ColonToken { get; }
        public SyntaxToken Identifier { get; }
        public SyntaxToken? LeftBracketToken { get; }
        public SyntaxToken? RightBracketToken { get; }
        public bool IsArray => LeftBracketToken != null && RightBracketToken != null;

        internal TypeDeclarationSyntax(SyntaxTree syntaxTree, SyntaxToken colonToken, SyntaxToken identifier, SyntaxToken? leftBracketToken, SyntaxToken? rightBracketToken) : base(syntaxTree)
        {
            ColonToken = colonToken;
            Identifier = identifier;
            LeftBracketToken = leftBracketToken;
            RightBracketToken = rightBracketToken;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return ColonToken;
            yield return Identifier;

            if (LeftBracketToken != null)
                yield return LeftBracketToken;

            if (RightBracketToken != null)
                yield return RightBracketToken;
        }
    }
}
