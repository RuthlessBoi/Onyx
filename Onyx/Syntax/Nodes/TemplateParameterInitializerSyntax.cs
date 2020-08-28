using System.Collections.Generic;

namespace Onyx.Syntax.Nodes
{
    public sealed class TemplateParameterInitializerSyntax : ExpressionSyntax
    {
        public override SyntaxType Type => SyntaxType.ModelParameterInitializerDeclarationNode;
        public SyntaxToken Identifier { get; }
        public SyntaxToken EqualsToken { get; }
        public ExpressionSyntax Value { get; }

        internal TemplateParameterInitializerSyntax(SyntaxTree syntaxTree, SyntaxToken identifier, SyntaxToken equalsToken, ExpressionSyntax value) : base(syntaxTree)
        {
            Identifier = identifier;
            EqualsToken = equalsToken;
            Value = value;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Identifier;
            yield return EqualsToken;
            yield return Value;
        }
    }
}
