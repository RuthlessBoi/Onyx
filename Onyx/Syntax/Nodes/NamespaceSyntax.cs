using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Onyx.Syntax.Nodes
{
    public sealed class NamespaceSyntax : ExpressionSyntax, IEnumerator<SyntaxToken>
    {
        public override SyntaxType Type => SyntaxType.NamespaceNode;

        public SeparatedSyntaxList<SyntaxToken> Identifiers { get; }
        public SyntaxToken Current => Identifiers[index];
        public bool IsFirst => index == 0;
        public bool HasNext => Identifiers.Count < index + 1;
        object IEnumerator.Current => Current;

        private int index = 0;

        internal NamespaceSyntax(SyntaxTree syntaxTree, SeparatedSyntaxList<SyntaxToken> identifiers) : base(syntaxTree) => Identifiers = identifiers;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            foreach (var node in Identifiers.GetWithSeparators())
                yield return node;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            foreach (var node in Identifiers.GetWithSeparators())
                builder.Append((node as SyntaxToken).Text);

            return builder.ToString();
        }

        public bool MoveNext()
        {
            if (index + 1 > Identifiers.Count)
                return false;

            index++;
            return true;
        }
        public void Reset() => index = 0;
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
