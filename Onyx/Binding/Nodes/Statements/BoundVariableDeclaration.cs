using Onyx.Binding.Symbols;
using Onyx.Syntax;

namespace Onyx.Binding.Nodes.Statements
{
    internal sealed class BoundVariableDeclaration : BoundStatement
    {
        public override BoundNodeType Type => BoundNodeType.VariableDeclaration;
        public VariableSymbol Variable { get; }
        public BoundExpression Initializer { get; }

        public BoundVariableDeclaration(SyntaxNode syntax, VariableSymbol variable, BoundExpression initializer) : base(syntax)
        {
            Variable = variable;
            Initializer = initializer;
        }
    }
}
