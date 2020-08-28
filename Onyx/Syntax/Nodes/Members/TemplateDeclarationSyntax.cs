using System.Collections.Generic;

namespace Onyx.Syntax.Nodes.Members
{
    public sealed class TemplateDeclarationSyntax : MemberSyntax
    {
        public override SyntaxType Type => SyntaxType.FunctionDeclarationNode;
        public SyntaxToken TemplateKeyword { get; }
        public SyntaxToken Identifier { get; }
        public GenericsDeclarationSyntax? GenericsDeclaration { get; }
        public SyntaxToken OpenBraceToken { get; }
        public SeparatedSyntaxList<TemplateParameterSyntax> Declarations { get; }
        public SyntaxToken CloseBraceToken { get; }
        public bool IsGeneric => GenericsDeclaration != null;

        internal TemplateDeclarationSyntax(SyntaxTree syntaxTree, SyntaxToken modelKeyword, SyntaxToken identifier, GenericsDeclarationSyntax? genericsDeclaration, SyntaxToken openBraceToken, SeparatedSyntaxList<TemplateParameterSyntax> modelDeclarations, SyntaxToken closeBraceToken) : base(syntaxTree)
        {
            TemplateKeyword = modelKeyword;
            Identifier = identifier;
            GenericsDeclaration = genericsDeclaration;
            OpenBraceToken = openBraceToken;
            Declarations = modelDeclarations;
            CloseBraceToken = closeBraceToken;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return TemplateKeyword;
            yield return Identifier;

            if (GenericsDeclaration != null)
                yield return GenericsDeclaration;

            yield return OpenBraceToken;

            foreach (var child in Declarations.GetWithSeparators())
                yield return child;

            yield return CloseBraceToken;
        }
    }
}
