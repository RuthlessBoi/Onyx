using System.Collections.Generic;

namespace Onyx.Syntax.Nodes
{
    public sealed class IndexerModifierSyntax : ModifierSyntax
    {
        public override SyntaxType Type => SyntaxType.IndexerModifierExpression;
        public SyntaxToken LeftBracketToken { get; }
        public SyntaxToken IndexToken { get; }
        public SyntaxToken RightBracketToken { get; }

        internal IndexerModifierSyntax(SyntaxTree syntaxTree, SyntaxToken leftBracketToken, SyntaxToken indexToken, SyntaxToken rightBracketToken) : base(syntaxTree)
        {
            LeftBracketToken = leftBracketToken;
            IndexToken = indexToken;
            RightBracketToken = rightBracketToken;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return LeftBracketToken;
            yield return IndexToken;
            yield return RightBracketToken;
        }
    }
}
