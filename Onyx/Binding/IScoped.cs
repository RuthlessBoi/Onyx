using Onyx.Binding.Symbols;
using System.Collections.Immutable;

namespace Onyx.Binding
{
    public interface IScoped
    {
        public bool TryDeclareSymbol<TSymbol>(TSymbol symbol) where TSymbol : Symbol;
        public Symbol? TryLookupSymbol(string name);
        public ImmutableArray<TSymbol> GetDeclaredSymbols<TSymbol>() where TSymbol : Symbol;
    }
}
