using Onyx.Binding.Nodes.Statements;
using Onyx.Binding.Symbols;
using System.Collections.Immutable;

namespace Onyx.Binding
{
    internal sealed class BoundProgram
    {
        public BoundProgram? Previous { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public FunctionSymbol? MainFunction { get; }
        public FunctionSymbol? ScriptFunction { get; }
        public ImmutableDictionary<FunctionSymbol, BoundBlockStatement> Functions { get; }

        public BoundProgram(BoundProgram? previous, ImmutableArray<Diagnostic> diagnostics, FunctionSymbol? mainFunction, FunctionSymbol? scriptFunction, ImmutableDictionary<FunctionSymbol, BoundBlockStatement> functions)
        {
            Previous = previous;
            Diagnostics = diagnostics;
            MainFunction = mainFunction;
            ScriptFunction = scriptFunction;
            Functions = functions;
        }
    }
}
