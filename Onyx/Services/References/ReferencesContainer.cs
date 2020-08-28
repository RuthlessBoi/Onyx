using Onyx.Binding.Symbols;
using Onyx.Syntax;
using System.Collections;
using System.Collections.Generic;

namespace Onyx.Services.References
{
    public sealed class ReferencesContainer : IEnumerable<Reference>
    {
        public int TotalReferences => references.Count;

        private readonly List<Reference> references;
        private readonly VariableSymbol symbol;

        public ReferencesContainer(VariableSymbol symbol)
        {
            references = new List<Reference>();
            this.symbol = symbol;
        }

        public void AddReference(SyntaxToken referrer, ReferenceType type) => references.Add(new Reference(symbol, referrer, type));

        public IEnumerator<Reference> GetEnumerator() => references.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public sealed class Reference
    {
        public VariableSymbol Symbol { get; }
        public SyntaxToken Referrer { get; }
        public ReferenceType Type { get; }

        internal Reference(VariableSymbol symbol, SyntaxToken referrer, ReferenceType type)
        {
            Symbol = symbol;
            Referrer = referrer;
            Type = type;
        }
    }

    public enum ReferenceType
    {
        Variable,
        Function,
        Model
    }
}
