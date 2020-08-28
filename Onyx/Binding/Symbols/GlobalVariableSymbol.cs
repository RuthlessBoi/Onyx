namespace Onyx.Binding.Symbols
{
    public class GlobalVariableSymbol : VariableSymbol
    {
        public override SymbolType Type => SymbolType.GlobalVariable;

        internal GlobalVariableSymbol(string name, bool isReadOnly, TypeSymbol type, BoundConstant? constant) : base(name, isReadOnly, type, constant) { }
    }
}
