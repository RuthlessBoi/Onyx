using System.Collections.Generic;

namespace Onyx.Syntax.Nodes
{
    public sealed partial class LiteralExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxType Type => SyntaxType.LiteralExpression;
        public SyntaxToken LiteralToken { get; }
        public object Value { get; }

        internal LiteralExpressionSyntax(SyntaxTree syntaxTree, SyntaxToken literalToken) : this(syntaxTree, literalToken, literalToken.Value!) { }
        internal LiteralExpressionSyntax(SyntaxTree syntaxTree, SyntaxToken literalToken, object value) : base(syntaxTree)
        {
            LiteralToken = literalToken;
            Value = value;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return LiteralToken;
        }
    }
}
