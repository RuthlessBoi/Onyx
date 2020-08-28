using Onyx.Binding.Symbols;
using Onyx.Syntax;
using System.Collections.Immutable;

namespace Onyx.Binding.Nodes.Expressions
{
    internal sealed class BoundCallExpression : BoundExpression
    {
        public override BoundNodeType Type => BoundNodeType.CallExpression;
        public override TypeSymbol ValueType => Function.ValueType;
        public FunctionSymbol Function { get; }
        public ImmutableArray<BoundExpression> Arguments { get; }

        public BoundCallExpression(SyntaxNode syntax, FunctionSymbol function, ImmutableArray<BoundExpression> arguments) : base(syntax)
        {
            Function = function;
            Arguments = arguments;
        }
    }
}
