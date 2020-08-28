using Onyx.Binding;
using Onyx.Binding.Symbols;
using Onyx.Compiler;
using Onyx.Compiler.CIL;
using Onyx.Compiler.OX;
using Onyx.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace Onyx
{
    public sealed class Compilation
    {
        public bool IsScript { get; }
        public Compilation? Previous { get; }
        public ImmutableArray<SyntaxTree> SyntaxTrees { get; }
        public FunctionSymbol? MainFunction => GlobalScope.MainFunction;
        public ImmutableArray<FunctionSymbol> Functions => GlobalScope.Functions;
        public ImmutableArray<VariableSymbol> Variables => GlobalScope.Variables;

        internal BoundGlobalScope GlobalScope
        {
            get
            {
                if (globalScope == null)
                {
                    var globalScope = Binder.BindGlobalScope(IsScript, Previous?.GlobalScope, SyntaxTrees);
                    Interlocked.CompareExchange(ref this.globalScope, globalScope, null);
                }

                return globalScope;
            }
        }

        private BoundGlobalScope? globalScope;

        private Compilation(bool isScript, Compilation? previous, params SyntaxTree[] syntaxTrees)
        {
            IsScript = isScript;
            Previous = previous;
            SyntaxTrees = syntaxTrees.ToImmutableArray();
        }

        public static Compilation Create(params SyntaxTree[] syntaxTrees) => new Compilation(isScript: false, previous: null, syntaxTrees);
        public static Compilation CreateScript(Compilation? previous, params SyntaxTree[] syntaxTrees) => new Compilation(isScript: true, previous, syntaxTrees);

        public IEnumerable<Symbol> GetSymbols()
        {
            var submission = this;
            var seenSymbolNames = new HashSet<string>();

            var builtinFunctions = BuiltinFunctions.GetAll().ToList();

            while (submission != null)
            {
                foreach (var function in submission.Functions)
                    if (seenSymbolNames.Add(function.Name))
                        yield return function;

                foreach (var variable in submission.Variables)
                    if (seenSymbolNames.Add(variable.Name))
                        yield return variable;

                foreach (var builtin in builtinFunctions)
                    if (seenSymbolNames.Add(builtin.Name))
                        yield return builtin;

                submission = submission.Previous;
            }
        }

        private BoundProgram GetProgram()
        {
            var previous = Previous == null ? null : Previous.GetProgram();
            return Binder.BindProgram(IsScript, previous, GlobalScope);
        }

        public EvaluationResult Evaluate(Dictionary<VariableSymbol, OnyxValue> variables)
        {
            if (GlobalScope.Diagnostics.Any())
                return new EvaluationResult(GlobalScope.Diagnostics, null);

            var program = GetProgram();

            if (program.Diagnostics.HasErrors())
                return new EvaluationResult(program.Diagnostics, null);

            var evaluator = new Evaluator(program, variables);
            var value = evaluator.Evaluate();

            return new EvaluationResult(program.Diagnostics, value);
        }

        public CompilationResult Compile([NotNull] string path, CompilerType type = CompilerType.OX)
        {
            switch (type)
            {
                case CompilerType.CIL:
                    {
                        var cil = new CILCompiler();
                        var result = cil.Compile();

                        return new CompilationResult(ImmutableArray<Diagnostic>.Empty, path, type, result);
                    }
                case CompilerType.OX:
                    {
                        var onyx = new OXCompiler();
                        var result = onyx.Compile();

                        return new CompilationResult(ImmutableArray<Diagnostic>.Empty, path, type, result);
                    }
                default:
                    return new CompilationResult(ImmutableArray<Diagnostic>.Empty, path, type, CompilerResult.Fail);
            }
        }
    }
}
