using System.Collections.Generic;
using System.Collections.Immutable;

namespace Onyx.Syntax.Nodes.Members
{
    public sealed class ClassDeclarationSyntax : MemberSyntax
    {
        public override SyntaxType Type => SyntaxType.ClassDeclarationNode;
        public SyntaxToken? AbstractKeyword { get; }
        public SyntaxToken ClassKeyword { get; }
        public SyntaxToken Identifier { get; }
        public GenericsDeclarationSyntax? GenericsDeclaration { get; }
        public SyntaxToken LeftBraceToken { get; }
        public ImmutableArray<MemberSyntax> Members { get; }
        public SyntaxToken RightBraceToken { get; }
        public bool IsAbstract => AbstractKeyword != null;
        public bool IsGeneric => GenericsDeclaration != null;

        internal ClassDeclarationSyntax(SyntaxTree syntaxTree, SyntaxToken? abstractKeyword, SyntaxToken classKeyword, SyntaxToken identifier, GenericsDeclarationSyntax? genericsDeclaration, SyntaxToken leftBraceToken, ImmutableArray<MemberSyntax> members, SyntaxToken rightBraceToken) : base(syntaxTree)
        {
            AbstractKeyword = abstractKeyword;
            ClassKeyword = classKeyword;
            Identifier = identifier;
            GenericsDeclaration = genericsDeclaration;
            LeftBraceToken = leftBraceToken;
            Members = members;
            RightBraceToken = rightBraceToken;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            if (AbstractKeyword != null)
                yield return AbstractKeyword;

            yield return ClassKeyword;
            yield return Identifier;

            if (GenericsDeclaration != null)
                yield return GenericsDeclaration;

            yield return LeftBraceToken;

            foreach (var member in Members)
                yield return member;

            yield return RightBraceToken;
        }
    }
}
