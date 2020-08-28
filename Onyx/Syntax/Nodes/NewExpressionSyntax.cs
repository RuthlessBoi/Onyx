using System.Collections.Generic;

namespace Onyx.Syntax.Nodes
{
    public sealed class NewExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxType Type => SyntaxType.NewExpression;
        public SyntaxToken NewKeyword { get; }
        public TypeSyntax InternalType { get; }
        public InitializerSyntax? Initializer { get; }

        internal NewExpressionSyntax(SyntaxTree syntaxTree, SyntaxToken newKeyword, TypeSyntax internalType, InitializerSyntax? initializer) : base(syntaxTree)
        {
            NewKeyword = newKeyword;
            InternalType = internalType;
            Initializer = initializer;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return NewKeyword;
            yield return InternalType;
            yield return Initializer;
        }
    }
}
