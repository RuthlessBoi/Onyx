using Onyx.Binding.Symbols;
using Onyx.Syntax;
using System;
using System.Collections.Immutable;

namespace Onyx.Binding.Nodes.Expressions
{
    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public override BoundNodeType Type => BoundNodeType.LiteralExpression;
        public override TypeSymbol ValueType { get; }
        public object Value => ConstantValue.Value;
        public override BoundConstant ConstantValue { get; }

        public BoundLiteralExpression(SyntaxNode syntax, object value) : base(syntax)
        {
            if (value is bool)
                ValueType = TypeSymbol.Bool;
            else if (value is int)
                ValueType = TypeSymbol.Int;
            else if (value is string)
                ValueType = TypeSymbol.String;
            else if (value is char)
                ValueType = TypeSymbol.Char;
            else if (value is BoundTemplate bt)
                ValueType = bt.InstanceType;
            else if (value is TypeContainer)
                ValueType = TypeSymbol.InternalType;
            else if (value is BoundArray ba)
                ValueType = ba.InstanceType;
            else
                throw new Exception($"Unexpected literal '{value}' of type {value.GetType()}");

            ConstantValue = new BoundConstant(value);
        }
    }

    internal abstract class BoundInitializerExpression { }

    internal sealed class BoundTemplateInitializerExpression : BoundInitializerExpression
    {
        public TemplateSymbol Type { get; }
        public ImmutableArray<BoundTemplateInitializer> Arguments { get; }
        public ImmutableArray<TypeSymbol> References { get; }

        public BoundTemplateInitializerExpression(TemplateSymbol type, ImmutableArray<BoundTemplateInitializer> arguments, ImmutableArray<TypeSymbol> references)
        {
            Type = type;
            Arguments = arguments;
            References = references;
        }
    }

    internal sealed class BoundArrayInitializerExpression : BoundInitializerExpression
    {
        public ArrayType Type { get; }
        public ImmutableArray<BoundExpression> Arguments { get; }
        public int Size { get; }

        public BoundArrayInitializerExpression(ArrayType type, ImmutableArray<BoundExpression> arguments, int size)
        {
            Type = type;
            Arguments = arguments;
            Size = size;
        }
    }
}
