using System.Collections.Generic;

namespace Onyx.Syntax.Nodes
{
    public sealed class TypeofExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxType Type => SyntaxType.TypeofExpression;
        public SyntaxToken TypeofKeyword { get; }
        public TypeSyntax InternalType { get; }

        internal TypeofExpressionSyntax(SyntaxTree syntaxTree, SyntaxToken typeofKeyword, TypeSyntax internalType) : base(syntaxTree)
        {
            TypeofKeyword = typeofKeyword;
            InternalType = internalType;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return TypeofKeyword;
            yield return InternalType;
        }
    }
}
