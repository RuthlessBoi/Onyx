using Onyx.Syntax;
using System.Collections.Immutable;

namespace Onyx.Binding.Nodes.Statements
{
    internal sealed class BoundBlockStatement : BoundStatement
    {
        public override BoundNodeType Type => BoundNodeType.BlockStatement;
        public ImmutableArray<BoundStatement> Statements { get; }

        public BoundBlockStatement(SyntaxNode syntax, ImmutableArray<BoundStatement> statements) : base(syntax) => Statements = statements;
    }
}
