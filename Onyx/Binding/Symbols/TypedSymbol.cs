namespace Onyx.Binding.Symbols
{
    public abstract class TypedSymbol : Symbol
    {
        public TypeSymbol ValueType { get; }

        public TypedSymbol(string name, TypeSymbol valueType) : base(name) => ValueType = valueType;
    }
}
