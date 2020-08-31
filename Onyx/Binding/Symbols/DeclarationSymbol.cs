using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Onyx.Binding.Symbols
{
    public abstract class DeclarationSymbol<TReturn, TArgument> : TypeSymbol
    {
        public DeclarationSymbol(string name) : base(name) { }

        internal abstract TReturn New(ImmutableArray<TArgument> arguments, Dictionary<string, TypeSymbol> types);
    }
}
