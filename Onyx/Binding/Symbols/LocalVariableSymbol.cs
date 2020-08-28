using System;

namespace Onyx.Binding.Symbols
{
    public class LocalVariableSymbol : VariableSymbol
    {
        public override SymbolType Type => SymbolType.LocalVariable;

        internal LocalVariableSymbol(string name, bool isReadOnly, TypeSymbol type, BoundConstant? constant, Guid? signature = null) : base(name, isReadOnly, type, constant, signature) { }
    }
}
