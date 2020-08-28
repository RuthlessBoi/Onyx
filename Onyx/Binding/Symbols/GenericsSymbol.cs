using System;
using System.Collections.Generic;

namespace Onyx.Binding.Symbols
{
    public sealed class GenericsSymbol : TypeSymbol
    {
        public Symbol Owner { get; }

        public GenericsSymbol(Symbol owner, string name, object defaultValue = null, Type internalType = null, Dictionary<string, Symbol> symbols = null) : base(name, defaultValue, internalType, symbols) => Owner = owner;

        public override bool Equals(object obj) => obj is GenericsSymbol gs && gs.Name == Name && gs.Owner.Name == Owner.Name;
        public override int GetHashCode() => Owner.GetHashCode() * Name.GetHashCode();
    }
}
