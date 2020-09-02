using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Onyx.Syntax
{
    public abstract class SeparatedSyntaxList
    {
        private protected SeparatedSyntaxList() { }

        public abstract ImmutableArray<SyntaxNode> GetWithSeparators();
    }

    public sealed class SeparatedSyntaxList<T> : SeparatedSyntaxList, IEnumerable<T> where T : SyntaxNode
    {
        private readonly ImmutableArray<SyntaxNode> nodes;

        internal SeparatedSyntaxList(ImmutableArray<SyntaxNode> nodes) => this.nodes = nodes;

        public int Count => (nodes.Length + 1) / 2;

        public T this[int index]
        {
            get
            {
                var pos = index * 2;

                if (pos > nodes.Length)
                    return (T)nodes[0];

                return (T)nodes[index * 2];
            }
        }

        public SyntaxToken GetSeparator(int index)
        {
            if (index < 0 || index >= Count - 1)
                throw new ArgumentOutOfRangeException(nameof(index));

            return (SyntaxToken)nodes[index * 2 + 1];
        }

        public override ImmutableArray<SyntaxNode> GetWithSeparators() => nodes;

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
                yield return this[i];
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
