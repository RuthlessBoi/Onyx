using System.Collections.Generic;
using System.Collections.Immutable;

namespace Onyx.Syntax.Nodes
{
    public abstract class ExpressionSyntax : SyntaxNode
    {
        private protected ExpressionSyntax(SyntaxTree syntaxTree) : base(syntaxTree) { }
    }
}
