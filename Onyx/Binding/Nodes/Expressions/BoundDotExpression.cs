using Onyx.Binding.Symbols;
using Onyx.Syntax;

namespace Onyx.Binding.Nodes.Expressions
{
    internal sealed class BoundDotExpression : BoundExpression
    {
        public override BoundNodeType Type => BoundNodeType.DotExpression;
        public override TypeSymbol ValueType => Chain.FinalVariable.ValueType;
        public BoundVariableChain Chain { get; }
        public override BoundConstant? ConstantValue
        {
            get
            {
                if (Chain != null && Chain.FinalVariable is VariableSymbol vs)
                    return vs.Constant;

                return null;
            }
        }

        public BoundDotExpression(SyntaxNode syntax, BoundVariableChain chain) : base(syntax) => Chain = chain;
    }
}
