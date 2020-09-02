using System.Collections.Generic;

namespace Onyx.Syntax.Nodes.Members
{
    public sealed class PropertyDeclarationSyntax : MemberSyntax
    {
        public override SyntaxType Type => SyntaxType.PropertyDeclarationNode;
        public SyntaxToken? ReadOnlyKeyword { get; }
        public SyntaxToken VarKeyword { get; }
        public SyntaxToken Identifier { get; }
        public TypeDeclarationSyntax InternalType { get; }
        public bool IsReadOnly => ReadOnlyKeyword != null;

        internal PropertyDeclarationSyntax(SyntaxTree syntaxTree, SyntaxToken? readOnlyKeyword, SyntaxToken varKeyword, SyntaxToken identifier, TypeDeclarationSyntax internalType) : base(syntaxTree)
        {
            ReadOnlyKeyword = readOnlyKeyword;
            VarKeyword = varKeyword;
            Identifier = identifier;
            InternalType = internalType;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            if (ReadOnlyKeyword != null)
                yield return ReadOnlyKeyword;

            yield return VarKeyword;
            yield return Identifier;
            yield return InternalType;
        }
    }
}
