using Onyx.Syntax.Nodes.Members;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Onyx.Binding.Symbols
{
    public sealed class FunctionSymbol : TypedSymbol, IAnnotatable
    {
        public override SymbolType Type => SymbolType.Function;
        public FunctionDeclarationSyntax? Declaration { get; }
        public ImmutableArray<ParameterSymbol> Parameters { get; }
        public ImmutableArray<BoundAnnotation> Annotations => annotations.ToImmutableArray();

        private readonly List<BoundAnnotation> annotations;

        internal FunctionSymbol(string name, ImmutableArray<ParameterSymbol> parameters, TypeSymbol type, FunctionDeclarationSyntax? declaration = null) : base(name, type)
        {
            Parameters = parameters;
            Declaration = declaration;

            annotations = new List<BoundAnnotation>();
        }

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

        public override string GetUniqueName() => Name; // TypeUtils.BuildUniqueName(Name, TypeUtils.GetTypesFromParameters(Parameters));

        public override string ToString() => Name;
        public override bool Equals(object obj) => obj is FunctionSymbol function && function.GetUniqueName() == GetUniqueName();
        public override int GetHashCode() => GetUniqueName().GetHashCode();
    }

    public sealed class UniqueSymbol
    {
        public string Name { get; }
        public ImmutableArray<TypeSymbol> Symbols { get; }

        public UniqueSymbol(string name, ImmutableArray<TypeSymbol> symbols)
        {
            Name = name;
            Symbols = symbols;
        }

        public bool Compare(UniqueSymbol symbol)
        {
            if (Name == symbol.Name)
            {
                if (Symbols.Any() && symbol.Symbols.Any())
                {
                    var success = true;

                    for (int i = 0; i < Symbols.Length; i++)
                    {
                        if (success)
                            success = (symbol.Symbols[i] == Symbols[i] || TypeSymbol.Any == Symbols[i]);
                        else
                            break;
                    }

                    return success;
                }
                else
                    return true;
            }

            return false;
        }

        public override bool Equals(object obj) => obj is UniqueSymbol symbol && Compare(symbol);
        public override int GetHashCode() => Name.GetHashCode() * Symbols.GetHashCode();
        public override string ToString()
        {
            var names = new List<string>();

            foreach (var symbol in Symbols)
                names.Add(symbol.Name);

            return $"{Name}({string.Join(", ", names)})";
        }
    }
}
