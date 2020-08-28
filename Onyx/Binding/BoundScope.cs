using Onyx.Binding.Symbols;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Onyx.Binding
{
    internal sealed class BoundScope : IScoped
    {
        public BoundScope? Parent { get; }
        public bool IsRoot => Parent == null;
        public int Depth { get; private set; }

        private Dictionary<string, Symbol>? symbols;
        private List<BoundNamespace> namespaces;

        public BoundScope(BoundScope? parent)
        {
            Parent = parent;

            if (Parent != null)
            {
                Depth = Parent.Depth + 1;

                if (Parent.namespaces != null)
                    namespaces.AddRange(Parent.namespaces);
            }
            else
                Depth = 0;
        }

        public void AddNamespace(BoundNamespace boundNamespace)
        {
            if (namespaces == null)
                namespaces = new List<BoundNamespace>();

            namespaces.Add(boundNamespace);
        }
        public ImmutableArray<BoundNamespace> GetNamespaces()
        {
            if (namespaces == null)
                return ImmutableArray<BoundNamespace>.Empty;

            return namespaces.ToImmutableArray();
        }
        public bool TryDeclareVariable(VariableSymbol variable) => TryDeclareSymbol(variable);
        public bool TryDeclareFunction(FunctionSymbol function) => TryDeclareSymbol(function);
        public bool TryDeclareTemplate(TemplateSymbol model) => TryDeclareSymbol(model);
        public bool TryDeclareAnnotation(AnnotationSymbol annotation) => TryDeclareSymbol(annotation);
        public bool TryDeclareSymbol<TSymbol>(TSymbol symbol) where TSymbol : Symbol
        {
            if (symbols == null)
                symbols = new Dictionary<string, Symbol>();
            else if (symbols.ContainsKey(symbol.GetUniqueName()))
                return false;

            symbols.Add(symbol.GetUniqueName(), symbol);

            return true;
        }
        public Symbol? TryLookupSymbol(string name)
        {
            if (symbols != null && symbols.TryGetValue(name, out var symbol))
                return symbol;

            return Parent?.TryLookupSymbol(name);
        }
        public ImmutableArray<VariableSymbol> GetDeclaredVariables() => GetDeclaredSymbols<VariableSymbol>();
        public ImmutableArray<FunctionSymbol> GetDeclaredFunctions() => GetDeclaredSymbols<FunctionSymbol>();
        public ImmutableArray<TemplateSymbol> GetDeclaredTemplates() => GetDeclaredSymbols<TemplateSymbol>();
        public ImmutableArray<AnnotationSymbol> GetDeclaredAnnotations() => GetDeclaredSymbols<AnnotationSymbol>();
        public ImmutableArray<TSymbol> GetDeclaredSymbols<TSymbol>() where TSymbol : Symbol
        {
            if (symbols == null)
                return ImmutableArray<TSymbol>.Empty;

            return symbols.Values.OfType<TSymbol>().ToImmutableArray();
        }
    }
}
