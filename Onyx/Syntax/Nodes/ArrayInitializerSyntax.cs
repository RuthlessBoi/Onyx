using System.Collections.Generic;

namespace Onyx.Syntax.Nodes
{
    public sealed class ArrayInitializerSyntax : InitializerSyntax
    {
        public override SyntaxType Type => SyntaxType.ArrayInitializerDeclarationNode;
        public override InitializerSyntaxType InitializerType => InitializerSyntaxType.Array;
        public SyntaxToken LeftBrace { get; }
        public SeparatedSyntaxList<ExpressionSyntax> Arguments { get; }
        public SyntaxToken RightBrace { get; }

        internal ArrayInitializerSyntax(SyntaxTree syntaxTree, SyntaxToken leftBrace, SeparatedSyntaxList<ExpressionSyntax> arguments, SyntaxToken rightBrace) : base(syntaxTree)
        {
            LeftBrace = leftBrace;
            Arguments = arguments;
            RightBrace = rightBrace;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return LeftBrace;

            foreach (var argument in Arguments)
                yield return argument;

            yield return RightBrace;
        }
    }
}
