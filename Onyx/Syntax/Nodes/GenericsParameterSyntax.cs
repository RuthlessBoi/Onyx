using System.Collections.Generic;

namespace Onyx.Syntax.Nodes
{
    public sealed class GenericsParameterSyntax : ExpressionSyntax
    {
        public override SyntaxType Type => SyntaxType.IndexerModifierExpression;
        public SyntaxToken Identifier { get; }
        public SyntaxToken? ColonToken { get; }
        public SyntaxToken? ParentIdentifier { get; }

        internal GenericsParameterSyntax(SyntaxTree syntaxTree, SyntaxToken identifier, SyntaxToken? colonToken, SyntaxToken? parentIdentifier) : base(syntaxTree)
        {
            Identifier = identifier;
            ColonToken = colonToken;
            ParentIdentifier = parentIdentifier;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Identifier;
            yield return ColonToken;
            yield return ParentIdentifier;
        }

        public override string ToString() => Identifier.Text;
    }
}
