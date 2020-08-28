using Onyx.Binding.Nodes;
using Onyx.Syntax;

namespace Onyx.Binding
{
    internal sealed class BoundTemplateInitializer
    {
        public BoundExpression Expression { get; }
        public SyntaxToken Identifier { get; }
        public bool IsGeneric { get; }

        internal BoundTemplateInitializer(BoundExpression expression, SyntaxToken identifier, bool isGeneric)
        {
            Expression = expression;
            Identifier = identifier;
            IsGeneric = isGeneric;
        }
    }
}
