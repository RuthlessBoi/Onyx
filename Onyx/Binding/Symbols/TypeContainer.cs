using System;

namespace Onyx.Binding.Symbols
{
    public class TypeContainer
    {
        /*public TypeSymbol InternalType { get; }

        public TypeContainer(TypeSymbol type) => InternalType = type;*/

        public Type InternalType { get; }
        public TypeSymbol Type { get; }

        public TypeContainer(Type internalType, TypeSymbol type)
        {
            InternalType = internalType;
            Type = type;
        }
    }
}
