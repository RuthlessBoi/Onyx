using System.Collections.Generic;

namespace Onyx.Syntax.Nodes.Members
{
    public sealed class AnnotationDeclarationSyntax : MemberSyntax
    {
        public override SyntaxType Type => SyntaxType.AnnotationDeclarationNode;
        public SyntaxToken AtKeyword { get; }
        public SyntaxToken Identifier { get; }
        public CallSyntax? Call { get; }
        public MemberSyntax AnnotatedMember { get; }

        internal AnnotationDeclarationSyntax(SyntaxTree syntaxTree, SyntaxToken atKeyword, SyntaxToken identifier, CallSyntax? call, MemberSyntax annotatedMember) : base(syntaxTree)
        {
            AtKeyword = atKeyword;
            Identifier = identifier;
            Call = call;
            AnnotatedMember = annotatedMember;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return AtKeyword;
            yield return Identifier;

            if (Call != null)
                yield return Call;

            yield return AnnotatedMember;
        }
    }
}
