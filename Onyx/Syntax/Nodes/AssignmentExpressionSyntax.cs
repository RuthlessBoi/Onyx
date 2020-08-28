using System.Collections.Generic;

namespace Onyx.Syntax.Nodes
{
    public sealed partial class AssignmentExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxType Type => SyntaxType.AssignmentExpression;
        public SyntaxToken Identifier { get; }
        public SyntaxToken AssignmentToken { get; }
        public ExpressionSyntax Expression { get; }

        public AssignmentExpressionSyntax(SyntaxTree syntaxTree, SyntaxToken identifier, SyntaxToken assignmentToken, ExpressionSyntax expression) : base(syntaxTree)
        {
            Identifier = identifier;
            AssignmentToken = assignmentToken;
            Expression = expression;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Identifier;
            yield return AssignmentToken;
            yield return Expression;
        }
    }
}
