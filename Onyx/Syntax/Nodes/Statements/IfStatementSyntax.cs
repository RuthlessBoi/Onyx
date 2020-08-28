using System.Collections.Generic;

namespace Onyx.Syntax.Nodes.Statements
{
    public sealed class IfStatementSyntax : StatementSyntax
    {
        public override SyntaxType Type => SyntaxType.IfStatement;
        public SyntaxToken IfKeyword { get; }
        public ExpressionSyntax Condition { get; }
        public StatementSyntax ThenStatement { get; }
        public ElseDeclarationSyntax? ElseDeclaration { get; }

        internal IfStatementSyntax(SyntaxTree syntaxTree, SyntaxToken ifKeyword, ExpressionSyntax condition, StatementSyntax thenStatement, ElseDeclarationSyntax? elseDeclaration) : base(syntaxTree)
        {
            IfKeyword = ifKeyword;
            Condition = condition;
            ThenStatement = thenStatement;
            ElseDeclaration = elseDeclaration;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return IfKeyword;
            yield return Condition;
            yield return ThenStatement;

            if (ElseDeclaration != null)
                yield return ElseDeclaration;
        }
    }
}
