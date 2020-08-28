using System.Collections.Generic;

namespace Onyx.Syntax.Nodes
{
    public sealed class TypeSyntax : SyntaxNode
    {
        public override SyntaxType Type => SyntaxType.TypeNode;
        public SyntaxToken Identifier { get; }
        public GenericsArgumentsSyntax? GenericsArguments { get; }
        public SyntaxToken? LeftBracketToken { get; }
        public SyntaxToken? SizeToken { get; }
        public SyntaxToken? RightBracketToken { get; }
        public bool IsGeneric => GenericsArguments != null;
        public bool IsArray => LeftBracketToken != null && RightBracketToken != null;

        internal TypeSyntax(SyntaxTree syntaxTree, SyntaxToken identifier, GenericsArgumentsSyntax? genericsArguments, SyntaxToken? leftBracketToken, SyntaxToken? sizeToken, SyntaxToken? rightBracketToken) : base(syntaxTree)
        {
            Identifier = identifier;
            GenericsArguments = genericsArguments;
            Identifier = identifier;
            LeftBracketToken = leftBracketToken;
            SizeToken = sizeToken;
            RightBracketToken = rightBracketToken;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Identifier;

            if (GenericsArguments != null)
                yield return GenericsArguments;

            if (LeftBracketToken != null)
                yield return LeftBracketToken;

            if (SizeToken != null)
                yield return SizeToken;

            if (RightBracketToken != null)
                yield return RightBracketToken;
        }
    }
}
