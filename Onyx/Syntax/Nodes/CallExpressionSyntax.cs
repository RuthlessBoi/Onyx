using System.Collections.Generic;

namespace Onyx.Syntax.Nodes
{
    public sealed class CallExpressionSyntax : IdentifiableExpressionSyntax
    {
        public override SyntaxType Type => SyntaxType.CallExpression;
        public GenericsArgumentsSyntax? GenericArguments { get; }
        public SyntaxToken OpenParenthesisToken { get; }
        public SeparatedSyntaxList<ExpressionSyntax> Arguments { get; }
        public SyntaxToken CloseParenthesisToken { get; }

        internal CallExpressionSyntax(SyntaxTree syntaxTree, SyntaxToken identifierToken, GenericsArgumentsSyntax? genericArguments, SyntaxToken openParenthesisToken, SeparatedSyntaxList<ExpressionSyntax> arguments, SyntaxToken closeParenthesisToken, IdentifiableExpressionSyntax? child) : base(syntaxTree, identifierToken, child)
        {
            GenericArguments = genericArguments;
            OpenParenthesisToken = openParenthesisToken;
            Arguments = arguments;
            CloseParenthesisToken = closeParenthesisToken;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return IdentifierToken;

            if (GenericArguments != null)
                yield return GenericArguments;

            yield return OpenParenthesisToken;

            foreach (var argument in Arguments)
                yield return argument;

            yield return CloseParenthesisToken;
        }
    }

    public sealed class CallSyntax : ExpressionSyntax
    {
        public override SyntaxType Type => SyntaxType.CallExpression;
        public SyntaxToken LeftParenthesisToken { get; }
        public SeparatedSyntaxList<ExpressionSyntax> Arguments { get; }
        public SyntaxToken RightParenthesisToken { get; }

        internal CallSyntax(SyntaxTree syntaxTree, SyntaxToken leftParenthesisToken, SeparatedSyntaxList<ExpressionSyntax> arguments, SyntaxToken rightParenthesisToken) : base(syntaxTree)
        {
            LeftParenthesisToken = leftParenthesisToken;
            Arguments = arguments;
            RightParenthesisToken = rightParenthesisToken;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return LeftParenthesisToken;

            foreach (var argument in Arguments)
                yield return argument;

            yield return RightParenthesisToken;
        }
    }
}
