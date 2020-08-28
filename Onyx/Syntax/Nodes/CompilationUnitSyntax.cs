using System.Collections.Generic;
using System.Collections.Immutable;

namespace Onyx.Syntax.Nodes
{
    public sealed partial class CompilationUnitSyntax : SyntaxNode
    {
        public override SyntaxType Type => SyntaxType.CompilationNode;
        public ImmutableArray<MemberSyntax> Members { get; }
        public SyntaxToken EndOfFileToken { get; }

        internal CompilationUnitSyntax(SyntaxTree syntaxTree, ImmutableArray<MemberSyntax> members, SyntaxToken endOfFileToken) : base(syntaxTree)
        {
            Members = members;
            EndOfFileToken = endOfFileToken;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            foreach (var member in Members)
                yield return member;

            yield return EndOfFileToken;
        }
    }
}
