using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Onyx.Binding.Symbols
{
    public class TypeSymbol : Symbol, IScoped, IAnnotatable
    {
        public static readonly TypeSymbol Error = new TypeSymbol("?", null);
        public static readonly TypeSymbol Void = new TypeSymbol("void", null);
        public static readonly TypeSymbol Any = new TypeSymbol("any", null, typeof(object));
        public static readonly TypeSymbol Bool = new TypeSymbol("bool", false, typeof(bool));
        public static readonly TypeSymbol Int = new TypeSymbol("int", 0, typeof(int));
        public static readonly TypeSymbol Char = new TypeSymbol("char", '\0', typeof(char));

        public static readonly LocalVariableSymbol LengthSymbol = new LocalVariableSymbol("length", true, Int, null);
        public static readonly LocalVariableSymbol StringAsCharArray = new LocalVariableSymbol("asCharArray", true, Char.Array, null);
        public static readonly FunctionSymbol StringGetChar = new FunctionSymbol("getChar", ImmutableArray.Create(new ParameterSymbol("index", Int, 0)), Char);

        public static readonly Dictionary<string, Symbol> StringSymbols = new Dictionary<string, Symbol>()
        {
            { "length", LengthSymbol },
            { "asCharArray", StringAsCharArray },
            { "getChar", StringGetChar }
        };

        public static readonly Dictionary<string, Symbol> ArraySymbols = new Dictionary<string, Symbol>()
        {
            { "length", LengthSymbol }
        };

        public static readonly TypeSymbol String = new TypeSymbol("string", "undefined", typeof(string), StringSymbols);
        public static readonly TypeSymbol InternalType = new TypeSymbol("Type", null, typeof(TemplateSymbol));

        public override SymbolType Type => SymbolType.Type;
        public object? Default { get; }
        public TypeContainer ContainedType { get; }
        public ArrayType Array { get; }
        public bool IsGeneric => this is GenericsSymbol;
        public bool IsArray => this is ArrayType;
        public bool IsTemplate => this is TemplateSymbol;
        //public bool IsClass => this is ClassSymbol;
        public ImmutableArray<BoundAnnotation> Annotations => annotations.ToImmutableArray();

        protected Dictionary<string, Symbol>? symbols;

        private readonly List<BoundAnnotation> annotations;

        public TypeSymbol(string name, object? defaultValue = null, Type? internalType = null, Dictionary<string, Symbol>? symbols = null) : base(name)
        {
            this.symbols = symbols;
            Default = defaultValue;
            ContainedType = new TypeContainer(internalType ?? typeof(object), this);

            annotations = new List<BoundAnnotation>();

            if (!IsArray)
                Array = new ArrayType(this, name, ArraySymbols);
        }

        public override string ToString() => Name;

        public void Annotate(BoundAnnotation? annotation)
        {
            if (annotation == null)
                return;

            annotations.AddRange(annotation.Annotations);
            annotations.Add(annotation);
        }
        public void Annotate(IEnumerable<BoundAnnotation> annotations)
        {
            foreach (var annotation in annotations)
                Annotate(annotation);
        }
        internal bool HasAnnotations() => annotations.Any();
        internal bool HasAnnotation(string name, out BoundAnnotation value)
        {
            foreach (var annotation in annotations)
            {
                if (annotation.Name == name)
                {
                    value = annotation;
                    return true;
                }
            }

            value = null;
            return false;
        }
        internal bool HasAnnotation(string name)
        {
            foreach (var annotation in annotations)
            {
                if (annotation.Name == name)
                {
                    return true;
                }
            }

            return false;
        }
        internal bool HasPragma(string pragma) =>
            HasAnnotation("pragma", out BoundAnnotation annotation) &&
            annotation.Values.Length > 0 &&
            annotation.Values[0] is string annotationPragma &&
            annotationPragma == pragma;

        public bool TryDeclareVariable(VariableSymbol variable) => TryDeclareSymbol(variable);
        public bool TryDeclareFunction(FunctionSymbol function) => TryDeclareSymbol(function);
        public ImmutableArray<VariableSymbol> GetDeclaredVariables() => GetDeclaredSymbols<VariableSymbol>();
        public ImmutableArray<FunctionSymbol> GetDeclaredFunctions() => GetDeclaredSymbols<FunctionSymbol>();
        public bool TryDeclareSymbol<TSymbol>(TSymbol symbol) where TSymbol : Symbol
        {
            if (symbols == null)
                symbols = new Dictionary<string, Symbol>();
            else if (symbols.ContainsKey(symbol.GetUniqueName()))
                return false;

            symbols.Add(symbol.GetUniqueName(), symbol);

            return true;
        }
        public ImmutableArray<TSymbol> GetDeclaredSymbols<TSymbol>() where TSymbol : Symbol
        {
            if (symbols == null)
                return ImmutableArray<TSymbol>.Empty;

            return symbols.Values.OfType<TSymbol>().ToImmutableArray();
        }
        public Symbol? TryLookupSymbol(string name)
        {
            if (symbols != null && symbols.TryGetValue(name, out var symbol))
                return symbol;

            return null;
        }

        public static UniqueSymbol Unique(string name, ImmutableArray<TypeSymbol>? types = null) => new UniqueSymbol(name, types ?? ImmutableArray<TypeSymbol>.Empty);

        public override string GetUniqueName() => Name;
    }
}
