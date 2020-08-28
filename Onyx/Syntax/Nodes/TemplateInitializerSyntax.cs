using System.Collections.Generic;

namespace Onyx.Syntax.Nodes
{
    public sealed class TemplateInitializerSyntax : InitializerSyntax
    {
        public override SyntaxType Type => SyntaxType.ModelInitializerDeclarationNode;
        public override InitializerSyntaxType InitializerType => InitializerSyntaxType.Model;
        public SyntaxToken LeftBrace { get; }
        public SeparatedSyntaxList<TemplateParameterInitializerSyntax> Parameters { get; }
        public SyntaxToken RightBrace { get; }

        internal TemplateInitializerSyntax(SyntaxTree syntaxTree, SyntaxToken leftBrace, SeparatedSyntaxList<TemplateParameterInitializerSyntax> parameters, SyntaxToken rightBrace) : base(syntaxTree)
        {
            LeftBrace = leftBrace;
            Parameters = parameters;
            RightBrace = rightBrace;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return LeftBrace;

            foreach (var parameter in Parameters)
                yield return parameter;

            yield return RightBrace;
        }
    }

    public abstract class InitializerSyntax : ExpressionSyntax
    {
        public abstract InitializerSyntaxType InitializerType { get; }

        internal InitializerSyntax(SyntaxTree syntaxTree) : base(syntaxTree) { }
    }

    public enum InitializerSyntaxType
    {
        Class,
        Model,
        Array,
    }
}
