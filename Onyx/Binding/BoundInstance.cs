﻿using Onyx.Binding.Symbols;
using System.Collections.Generic;
using Onyx.Binding.Nodes.Expressions;
using System;

namespace Onyx.Binding
{
    public abstract class BoundInstance<TSymbol> where TSymbol : IInstanced
    {
        public TSymbol InstanceType { get; protected set; }

        private Dictionary<VariableSymbol, OnyxValue> values;
        private Dictionary<FunctionSymbol, Func<BoundCallExpression, object?>> functions;

        internal BoundInstance(TSymbol type) => InstanceType = type;

        public OnyxValue? Get(VariableSymbol variable)
        {
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
    }
}
