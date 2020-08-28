using System.Collections.Generic;
using System;
using System.Collections.Immutable;
using Onyx.Binding.Nodes;
using Onyx.Binding.Nodes.Expressions;

namespace Onyx.Binding.Symbols
{
    public sealed class ArrayType : TypeSymbol, IInstanced
    {
        public TypeSymbol StorageType { get; }
        public BoundArray Empty { get; }

        internal ArrayType(TypeSymbol type, string name, Dictionary<string, Symbol>? symbols = null) : base($"{name}[]", null, typeof(BoundArray), symbols)
        {
            StorageType = type;
            Empty = new BoundArray(this, 0);
        }

        internal BoundArray New(ImmutableArray<BoundExpression> initializers, int size)
        {
            var array = new BoundArray(this, size);

            for (int i = 0; i < size; i++)
            {
                if (i < initializers.Length)
                {
                    var initializer = initializers[i];

                    array.Assign(i, new OnyxValue((initializer is BoundLiteralExpression ble ? ble.Value : initializer), StorageType));
                }
                else
                    array.Assign(i, new OnyxValue(StorageType.Default, StorageType));
            }

            return array;
        }
    }
}
