using System.Collections.Generic;

namespace Onyx.Syntax.Nodes
{
    public sealed partial class ElseDeclarationSyntax : SyntaxNode
    {
        public override SyntaxType Type => SyntaxType.ElseDelcarationNode;
        public SyntaxToken ElseKeyword { get; }
        public StatementSyntax ElseStatement { get; }

        internal ElseDeclarationSyntax(SyntaxTree syntaxTree, SyntaxToken elseKeyword, StatementSyntax elseStatement) : base(syntaxTree)
        {
            ElseKeyword = elseKeyword;
            ElseStatement = elseStatement;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return ElseKeyword;
            yield return ElseStatement;
        }
    }
}
