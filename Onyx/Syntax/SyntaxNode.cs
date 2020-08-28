using Onyx.Text;
using System.Collections.Generic;
using System.Linq;

namespace Onyx.Syntax
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxType Type { get; }
        public SyntaxTree SyntaxTree { get; }
        public SyntaxNode? Parent => SyntaxTree.GetParent(this);
        public virtual TextSpan Span
        {
            get
            {
                var children = GetChildren();

                var firstChild = children.First().Span;
                var lastChild = children.First().Span;

                return TextSpan.FromBounds(firstChild.Start, lastChild.End);
            }
        }
        public virtual TextSpan FullSpan
        {
            get
            {
                var children = GetChildren();

                var firstChild = children.First().FullSpan;
                var lastChild = children.First().FullSpan;

                return TextSpan.FromBounds(firstChild.Start, lastChild.End);
            }
        }
        public TextLocation Location => new TextLocation(SyntaxTree.Text, Span);

        private protected SyntaxNode(SyntaxTree syntaxTree) => SyntaxTree = syntaxTree;

        public abstract IEnumerable<SyntaxNode> GetChildren();

        public IEnumerable<SyntaxNode> AncestorsAndSelf()
        {
            var node = this;

            while (node != null)
            {
                yield return node;
                node = node.Parent;
            }
        }
        public IEnumerable<SyntaxNode> Ancestors() => AncestorsAndSelf().Skip(1);

        public SyntaxToken GetLastToken()
        {
            if (this is SyntaxToken token)
                return token;

            return GetChildren().Last().GetLastToken();
        }
    }
}
