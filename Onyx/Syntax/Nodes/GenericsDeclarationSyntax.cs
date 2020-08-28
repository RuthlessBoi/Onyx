using System.Collections.Generic;

namespace Onyx.Syntax.Nodes
{
    public sealed class GenericsDeclarationSyntax : ModifierSyntax
    {
        public override SyntaxType Type => SyntaxType.IndexerModifierExpression;
        public SyntaxToken LessThanToken { get; }
        public SeparatedSyntaxList<GenericsParameterSyntax> Parameters { get; }
        public SyntaxToken GreaterThanToken { get; }

        internal GenericsDeclarationSyntax(SyntaxTree syntaxTree, SyntaxToken lessThanToken, SeparatedSyntaxList<GenericsParameterSyntax> parameters, SyntaxToken greaterThanToken) : base(syntaxTree)
        {
            LessThanToken = lessThanToken;
            Parameters = parameters;
            GreaterThanToken = greaterThanToken;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return LessThanToken;

            foreach (var parameter in Parameters)
                yield return parameter;

            yield return GreaterThanToken;
        }

        public bool ContainsParameter(string name)
        {
            foreach (var parameter in Parameters)
            {
                if (parameter.Identifier.Text == name)
                    return true;
            }

            return false;
        }
    }
}
