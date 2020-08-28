using Onyx.Binding.Symbols;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using System.Text;

namespace Onyx.Binding
{
    internal static class TypeUtils
    {
        internal static string BuildUniqueName(string name, IEnumerable<TypeSymbol> types)
        {
            var builder = new StringBuilder();

            builder.Append(name);

            if (types.Any())
                builder.Append('(').Append(string.Join(", ", types)).Append(')');

            return builder.ToString();
        }

        internal static ImmutableArray<TypeSymbol> GetTypesFromParameters(IEnumerable<ParameterSymbol> parameters)
        {
            var array = ImmutableArray.CreateBuilder<TypeSymbol>();

            foreach (var parameter in parameters)
                array.Add(parameter.ValueType);

            return array.ToImmutable();
        }
    }
}
