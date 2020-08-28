using Onyx.Syntax;

namespace Onyx.Binding.Nodes.Statements
{
    internal abstract class BoundLoopStatement : BoundStatement
    {
        public BoundLabel BreakLabel { get; }
        public BoundLabel ContinueLabel { get; }

        protected BoundLoopStatement(SyntaxNode syntax, BoundLabel breakLabel, BoundLabel continueLabel) : base(syntax)
        {
            BreakLabel = breakLabel;
            ContinueLabel = continueLabel;
        }
    }
}
