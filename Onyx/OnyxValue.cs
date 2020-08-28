using Onyx.Binding;
using Onyx.Binding.Nodes.Expressions;
using Onyx.Binding.Symbols;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Onyx
{
    public sealed class OnyxValue
    {
        public static readonly OnyxValue Undefined = new OnyxValue("undefined", TypeSymbol.Any);

        public object? Value { get; }
        public TypeSymbol Type { get; }

        private Dictionary<VariableSymbol, OnyxValue> values;
        private Dictionary<FunctionSymbol, Func<BoundCallExpression, object?>> functions;

        internal OnyxValue(object? value, TypeSymbol type)
        {
            Value = value;
            Type = type;
        }

        public OnyxValue? Get(VariableSymbol variable)
        {
            if (Value is BoundTemplate bm)
                return bm.Get(variable);

            if (values == null)
                values = new Dictionary<VariableSymbol, OnyxValue>();

            if (values.ContainsKey(variable))
                return values[variable];

            return null;
        }
        public void Assign(VariableSymbol variable, OnyxValue value)
        {
            if (values == null)
                values = new Dictionary<VariableSymbol, OnyxValue>();

            values[variable] = value;
        }
        internal Func<BoundCallExpression, object?> Get(FunctionSymbol function)
        {
            if (Value is BoundTemplate bm)
                return bm.Get(function);

            if (functions == null)
                functions = new Dictionary<FunctionSymbol, Func<BoundCallExpression, object?>>();

            if (functions.ContainsKey(function))
                return functions[function];

            return null;
        }
        internal void Assign(FunctionSymbol function, Func<BoundCallExpression, object?> callback)
        {
            if (functions == null)
                functions = new Dictionary<FunctionSymbol, Func<BoundCallExpression, object?>>();

            functions[function] = callback;
        }
        public ImmutableDictionary<VariableSymbol, OnyxValue> GetSymbols()
        {
            if (values == null)
                return ImmutableDictionary<VariableSymbol, OnyxValue>.Empty;

            return values.ToImmutableDictionary();
        }
        public override string ToString() => Value?.ToString() ?? "null";
    }
}
