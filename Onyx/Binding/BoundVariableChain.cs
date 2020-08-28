using Onyx.Binding.Nodes;
using Onyx.Binding.Symbols;

namespace Onyx.Binding
{
    internal sealed class BoundVariableChain
    {
        public TypedSymbol Variable { get; }
        public BoundVariableChain? Child { get; }
        public BoundVariableChain Final
        {
            get
            {
                if (Child != null)
                    return Child.Final;

                return this;
            }
        }
        public TypedSymbol FinalVariable => Final.Variable;
        public BoundExpression Syntax { get; }
        public bool HasChild => Child != null;

        public BoundVariableChain(TypedSymbol variable, BoundVariableChain? child, BoundExpression syntax)
        {
            Variable = variable;
            Child = child;
            Syntax = syntax;
        }
    }
}
