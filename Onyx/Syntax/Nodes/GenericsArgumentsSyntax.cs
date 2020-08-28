using System.Collections.Generic;

namespace Onyx.Syntax.Nodes
{
    public sealed class GenericsArgumentsSyntax : SyntaxNode
    {
        public override SyntaxType Type => SyntaxType.GenericsNode;
        public SyntaxToken LessThanToken { get; }
        public SeparatedSyntaxList<TypeSyntax> GenericsArguments { get; }
        public SyntaxToken GreaterThanToken { get; }

        internal GenericsArgumentsSyntax(SyntaxTree syntaxTree, SyntaxToken lessThanToken, SeparatedSyntaxList<TypeSyntax> genericsArguments, SyntaxToken greaterThanToken) : base(syntaxTree)
        {
            LessThanToken = lessThanToken;
            GenericsArguments = genericsArguments;
            GreaterThanToken = greaterThanToken;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return LessThanToken;

            foreach (var argument in GenericsArguments)
                yield return argument;

            yield return GreaterThanToken;
        }
    }
}
