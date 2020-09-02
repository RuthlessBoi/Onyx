using System.Collections.Generic;

namespace Onyx.Syntax.Nodes.Members
{
    public sealed class AbstractPropertyDeclarationSyntax : MemberSyntax
    {
        public override SyntaxType Type => SyntaxType.AbstractPropertyDeclarationNode;
        public SyntaxToken AbstractKeyword { get; }
        public SyntaxToken? ReadOnlyKeyword { get; }
        public SyntaxToken VarKeyword { get; }
        public SyntaxToken Identifier { get; }
        public TypeDeclarationSyntax InternalType { get; }
        public bool IsReadOnly => ReadOnlyKeyword != null;

        internal AbstractPropertyDeclarationSyntax(SyntaxTree syntaxTree, SyntaxToken? abstractKeyword, SyntaxToken? readOnlyKeyword, SyntaxToken varKeyword, SyntaxToken identifier, TypeDeclarationSyntax internalType) : base(syntaxTree)
        {
            AbstractKeyword = abstractKeyword;
            ReadOnlyKeyword = readOnlyKeyword;
            VarKeyword = varKeyword;
            Identifier = identifier;
            InternalType = internalType;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return AbstractKeyword;

            if (ReadOnlyKeyword != null)
                yield return ReadOnlyKeyword;

            yield return VarKeyword;
            yield return Identifier;
            yield return InternalType;
        }
    }
}
