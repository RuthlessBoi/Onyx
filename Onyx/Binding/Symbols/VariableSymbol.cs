using Onyx.Services.References;
using System;

namespace Onyx.Binding.Symbols
{
    public abstract class VariableSymbol : TypedSymbol
    {
        public bool IsReadOnly { get; }
        public ReferencesContainer References { get; }
        public Guid Signature { get; }
        internal BoundConstant? Constant { get; }

        internal VariableSymbol(string name, bool isReadOnly, TypeSymbol type, BoundConstant? constant, Guid? signature = null) : base(name, type)
        {
            IsReadOnly = isReadOnly;
            Constant = constant;
            References = new ReferencesContainer(this);
            Signature = signature ?? Guid.NewGuid();
        }

        public override string GetUniqueName() => Name;

        public override bool Equals(object obj) => obj is VariableSymbol symbol && symbol.Name == Name && symbol.Signature == Signature;
        public override int GetHashCode() => Name.GetHashCode() * Signature.GetHashCode();
        public override string ToString() => $"{Name}-{Signature}";
    }
}
