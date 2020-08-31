using System;

namespace Onyx.Binding.Symbols
{
    public class TypeContainer
    {
        public Type InternalType { get; }
        public TypeSymbol Type { get; }

        public TypeContainer(Type internalType, TypeSymbol type)
        {
            InternalType = internalType;
            Type = type;
        }
    }
}
