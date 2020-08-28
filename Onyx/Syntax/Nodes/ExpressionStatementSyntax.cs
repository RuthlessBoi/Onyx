using System.Collections.Generic;

namespace Onyx.Syntax.Nodes
{
    public sealed partial class ExpressionStatementSyntax : StatementSyntax
    {
        public override SyntaxType Type => SyntaxType.ExpressionStatement;
        public ExpressionSyntax Expression { get; }

        internal ExpressionStatementSyntax(SyntaxTree syntaxTree, ExpressionSyntax expression) : base(syntaxTree) => Expression = expression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Expression;
        }
    }
}
