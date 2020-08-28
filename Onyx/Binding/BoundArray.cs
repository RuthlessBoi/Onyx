using Onyx.Binding.Symbols;
using System.Collections.Generic;
using System.Linq;

namespace Onyx.Binding
{
    public sealed class BoundArray : BoundInstance<ArrayType>
    {
        public int Size { get; }

        private OnyxValue[] array;

        internal BoundArray(ArrayType type, int size) : base(type) => Size = size;

        public OnyxValue? Get(int index)
        {
            if (array != null && index <= array.Length)
                return array[index];

            return null;
        }
        public void Assign(int index, OnyxValue value)
        {
            if (array == null)
                array = new OnyxValue[Size];

            array[index] = value;
        }
        public IEnumerable<OnyxValue> GetArray()
        {
            if (array == null)
                return Enumerable.Empty<OnyxValue>();

            return array;
        }
    }
}
