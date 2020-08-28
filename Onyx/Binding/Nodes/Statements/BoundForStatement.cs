using Onyx.Binding.Symbols;
using Onyx.Syntax;

namespace Onyx.Binding.Nodes.Statements
{
    internal sealed class BoundForStatement : BoundLoopStatement
    {
        public override BoundNodeType Type => BoundNodeType.ForStatement;
        public VariableSymbol Variable { get; }
        public BoundExpression LowerBound { get; }
        public BoundExpression UpperBound { get; }
        public BoundStatement Body { get; }

        public BoundForStatement(SyntaxNode syntax, VariableSymbol variable, BoundExpression lowerBound, BoundExpression upperBound, BoundStatement body, BoundLabel breakLabel, BoundLabel continueLabel) : base(syntax, breakLabel, continueLabel)
        {
            Variable = variable;
            LowerBound = lowerBound;
            UpperBound = upperBound;
            Body = body;
        }
    }
}
