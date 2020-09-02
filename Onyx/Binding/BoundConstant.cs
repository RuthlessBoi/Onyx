using Onyx.Binding.Symbols;

namespace Onyx.Binding
{
    internal sealed class BoundConstant
    {
        public object Value { get; }
        public TypeSymbol Type { get; }
        public OnyxValue OnyxValue { get; }

        public BoundConstant(object value, TypeSymbol type)
        {
            Value = value;
            Type = type;
            OnyxValue = new OnyxValue(value, type);
        }
    }
}
