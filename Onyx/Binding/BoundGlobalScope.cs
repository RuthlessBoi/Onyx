﻿using Onyx.Binding.Nodes;
using Onyx.Binding.Symbols;
using System.Collections.Immutable;

namespace Onyx.Binding
{
    internal sealed class BoundGlobalScope
    {
        public BoundGlobalScope? Previous { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public FunctionSymbol? MainFunction { get; }
        public FunctionSymbol? ScriptFunction { get; }
        public ImmutableArray<BoundNamespace> Namespaces { get; }
        public ImmutableArray<FunctionSymbol> Functions { get; }
        public ImmutableArray<TemplateSymbol> TemplateTypes { get; }
        public ImmutableArray<VariableSymbol> Variables { get; }
        public ImmutableArray<BoundStatement> Statements { get; }

        public BoundGlobalScope(BoundGlobalScope? previous, ImmutableArray<Diagnostic> diagnostics, FunctionSymbol? mainFunction, FunctionSymbol? scriptFunction, ImmutableArray<BoundNamespace> namespaces, ImmutableArray<FunctionSymbol> functions, ImmutableArray<TemplateSymbol> modelTypes, ImmutableArray<VariableSymbol> variables, ImmutableArray<BoundStatement> statements)
        {
            Previous = previous;
            Diagnostics = diagnostics;
            MainFunction = mainFunction;
            ScriptFunction = scriptFunction;
            Namespaces = namespaces;
            Functions = functions;
            TemplateTypes = modelTypes;
            Variables = variables;
            Statements = statements;
        }
    }
}
