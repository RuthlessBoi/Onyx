namespace Onyx.Binding.Symbols
{
    public sealed class ParameterSymbol : LocalVariableSymbol
    {
        public override SymbolType Type => SymbolType.Parameter;
        public int Ordinal { get; }

        public ParameterSymbol(string name, TypeSymbol type, int ordinal) : base(name, isReadOnly: true, type, null) => Ordinal = ordinal;
    }
}
