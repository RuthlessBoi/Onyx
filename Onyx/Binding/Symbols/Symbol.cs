namespace Onyx.Binding.Symbols
{
    public abstract class Symbol
    {
        public abstract SymbolType Type { get; }
        public string Name { get; }

        private protected Symbol(string name) => Name = name;

        public abstract string GetUniqueName();
    }
}
