using System.Collections.Generic;

namespace Onyx.Syntax.Nodes
{
    public sealed class NameExpressionSyntax : IdentifiableExpressionSyntax
    {
        public override SyntaxType Type => SyntaxType.NameExpression;

        public ModifierSyntax? Modifier { get; }

        internal NameExpressionSyntax(SyntaxTree syntaxTree, SyntaxToken identifierToken, ModifierSyntax? modifier) : base(syntaxTree, identifierToken, null) => Modifier = modifier;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return IdentifierToken;

            if (Modifier != null)
                yield return Modifier;
        }
    }
}
