using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Onyx.Binding.Symbols
{
    public sealed class AnnotationSymbol : Symbol
    {
        /// <summary>
        /// Pragma contains annotation values of:
        ///     - runtime:entry_point (if a main() function isn't present, use the annotated function instead)
        /// </summary>
        public static readonly AnnotationSymbol PragmaAnnotation = new AnnotationSymbol("pragma", ImmutableArray.Create(new ParameterSymbol("option", TypeSymbol.String, 0)));
        public static readonly AnnotationSymbol DeprecatedAnnotation = new AnnotationSymbol("deprecated", ImmutableArray<ParameterSymbol>.Empty);

        public override SymbolType Type => SymbolType.Annotation;
        public ImmutableArray<ParameterSymbol> Parameters { get; }

        internal AnnotationSymbol(string name, ImmutableArray<ParameterSymbol> parameters) : base(name) => Parameters = parameters;

        internal static IEnumerable<AnnotationSymbol> GetAll() => typeof(AnnotationSymbol)
                                       .GetFields(BindingFlags.Public | BindingFlags.Static)
                                       .Where(f => f.FieldType == typeof(AnnotationSymbol))
                                       .Select(f => (AnnotationSymbol)f.GetValue(null)!);

        public override string GetUniqueName() => TypeUtils.BuildUniqueName(Name, TypeUtils.GetTypesFromParameters(Parameters));

        public override string ToString() => Name;
        public override bool Equals(object obj) => obj is AnnotationSymbol annotation && annotation.GetUniqueName() == GetUniqueName();
        public override int GetHashCode() => Name.GetHashCode() * Parameters.GetHashCode();
    }
}
