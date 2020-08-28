using Onyx.Binding.Symbols;

namespace Onyx.Binding
{
    internal sealed class Conversion
    {
        public static readonly Conversion None = new Conversion(exists: false, isIdentity: false, isImplicit: false);
        public static readonly Conversion Identity = new Conversion(exists: true, isIdentity: true, isImplicit: true);
        public static readonly Conversion Implicit = new Conversion(exists: true, isIdentity: false, isImplicit: true);
        public static readonly Conversion Explicit = new Conversion(exists: true, isIdentity: false, isImplicit: false);

        public bool Exists { get; }
        public bool IsIdentity { get; }
        public bool IsImplicit { get; }
        public bool IsExplicit => Exists && !IsImplicit;

        private Conversion(bool exists, bool isIdentity, bool isImplicit)
        {
            Exists = exists;
            IsIdentity = isIdentity;
            IsImplicit = isImplicit;
        }

        public static Conversion Classify(TypeSymbol from, TypeSymbol to)
        {
            if (from == to)
                return Identity;

            if (from != TypeSymbol.Void && to == TypeSymbol.Any)
                return Implicit;

            if (from == TypeSymbol.Any && to != TypeSymbol.Void)
                return Explicit;

            if (from == TypeSymbol.Bool || from == TypeSymbol.Int || from == TypeSymbol.Char)
            {
                if (to == TypeSymbol.String)
                    return Explicit;
            }

            if (from == TypeSymbol.String)
            {
                if (to == TypeSymbol.Bool || to == TypeSymbol.Int)
                    return Explicit;
            }

            if (from is TypeSymbol && to is TemplateSymbol)
                return Implicit;

            if (from is TypeSymbol && to is GenericsSymbol)
                return Implicit;

            if (from is TemplateSymbol)
                return Explicit;

            return None;
        }
    }
}
