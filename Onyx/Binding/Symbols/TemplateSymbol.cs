using Onyx.Syntax.Nodes;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Onyx.Binding.Symbols
{
    public class TemplateSymbol : DeclarationSymbol<BoundTemplate, KeyValuePair<string, OnyxValue>>, IInstanced
    {
        public override SymbolType Type => SymbolType.Template;
        public GenericsDeclarationSyntax? GenericsDeclaration { get; }
        public new bool IsGeneric => GenericsDeclaration != null;

        public TemplateSymbol(string name, GenericsDeclarationSyntax? genericsDeclaration) : base(name) => GenericsDeclaration = genericsDeclaration;

        private void RebuildSymbols(Dictionary<string, TypeSymbol> types)
        {
            var newSymbols = new Dictionary<string, Symbol>();

            foreach (var pair in symbols)
            {
                if (pair.Value is VariableSymbol symbol)
                {
                    if (symbol.ValueType is GenericsSymbol genericSymbol)
                        newSymbols[pair.Key] = new LocalVariableSymbol(symbol.Name, symbol.IsReadOnly, types[genericSymbol.Name], symbol.Constant, symbol.Signature);
                    else
                        newSymbols[pair.Key] = symbol;
                }
            }

            symbols = newSymbols;
        }

        internal override BoundTemplate New(ImmutableArray<KeyValuePair<string, OnyxValue>> arguments, Dictionary<string, TypeSymbol> types)
        {
            var model = new BoundTemplate(this);

            if (IsGeneric && types.Any())
                RebuildSymbols(types);

            foreach (var pair in symbols)
            {
                if (pair.Value is VariableSymbol symbol)
                {
                    OnyxValue value;

                    if (ContainsSymbol(arguments, symbol, out OnyxValue ret))
                        value = new OnyxValue(ret.Value, ret.Type);
                    else
                        value = new OnyxValue(symbol.ValueType.Default, symbol.ValueType);

                    model.Assign(symbol, value);

                    /*if (symbol.ValueType.Name == value.Type.Name)
                        model.Assign(symbol, value);
                    else
                        diagnostics($"The argument type ({value.Type.Name}) is incompatible with the parameter type ({symbol.ValueType.Name}).");*/
                }
            }

            return model;
        }
        internal bool ContainsSymbol(ImmutableArray<KeyValuePair<string, OnyxValue>> parameters, VariableSymbol symbol, out OnyxValue ret)
        {
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    if (symbol.Name == parameter.Key)
                    {
                        ret = parameter.Value;
                        return true;
                    }
                }
            }

            ret = null;
            return false;
        }

        public override string GetUniqueName() => Name;
    }
}
