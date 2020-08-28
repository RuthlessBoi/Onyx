using Onyx.Binding.Nodes;
using Onyx.Binding.Nodes.Expressions;
using Onyx.Binding.Nodes.Statements;
using Onyx.Binding.Symbols;
using Onyx.Syntax;
using System;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Onyx.Binding
{
    internal static class BoundNodeFactory
    {
        public static BoundBlockStatement Block(SyntaxNode syntax, params BoundStatement[] statements) => new BoundBlockStatement(syntax, ImmutableArray.Create(statements));
        public static BoundVariableDeclaration VariableDeclaration(SyntaxNode syntax, VariableSymbol symbol, BoundExpression initializer) => new BoundVariableDeclaration(syntax, symbol, initializer);
        public static BoundVariableDeclaration VariableDeclaration(SyntaxNode syntax, string name, BoundExpression initializer) => VariableDeclarationInternal(syntax, name, initializer, isReadOnly: false);
        public static BoundVariableDeclaration ConstantDeclaration(SyntaxNode syntax, string name, BoundExpression initializer) => VariableDeclarationInternal(syntax, name, initializer, isReadOnly: true);
        private static BoundVariableDeclaration VariableDeclarationInternal(SyntaxNode syntax, string name, BoundExpression initializer, bool isReadOnly)
        {
            var local = new LocalVariableSymbol(name, isReadOnly, initializer.ValueType, initializer.ConstantValue);

            return new BoundVariableDeclaration(syntax, local, initializer);
        }
        public static BoundWhileStatement While(SyntaxNode syntax, BoundExpression condition, BoundStatement body, BoundLabel breakLabel, BoundLabel continueLabel) => new BoundWhileStatement(syntax, condition, body, breakLabel, continueLabel);
        public static BoundGotoStatement Goto(SyntaxNode syntax, BoundLabel label) => new BoundGotoStatement(syntax, label);
        public static BoundConditionalGotoStatement GotoTrue(SyntaxNode syntax, BoundLabel label, BoundExpression condition) => new BoundConditionalGotoStatement(syntax, label, condition, jumpIfTrue: true);
        public static BoundConditionalGotoStatement GotoFalse(SyntaxNode syntax, BoundLabel label, BoundExpression condition) => new BoundConditionalGotoStatement(syntax, label, condition, jumpIfTrue: false);
        public static BoundLabelStatement Label(SyntaxNode syntax, BoundLabel label) => new BoundLabelStatement(syntax, label);
        public static BoundNopStatement Nop(SyntaxNode syntax) => new BoundNopStatement(syntax);
        public static BoundAssignmentExpression Assignment(SyntaxNode syntax, VariableSymbol variable, BoundExpression expression) => new BoundAssignmentExpression(syntax, variable, expression);
        public static BoundBinaryExpression Binary(SyntaxNode syntax, BoundExpression left, SyntaxType type, BoundExpression right)
        {
            var op = BoundBinaryOperator.Bind(type, left.ValueType, right.ValueType)!;

            return Binary(syntax, left, op, right);
        }
        public static BoundBinaryExpression Binary(SyntaxNode syntax, BoundExpression left, BoundBinaryOperator op, BoundExpression right) => new BoundBinaryExpression(syntax, left, op, right);
        public static BoundBinaryExpression Add(SyntaxNode syntax, BoundExpression left, BoundExpression right) => Binary(syntax, left, SyntaxType.PlusToken, right);
        public static BoundBinaryExpression LessOrEqual(SyntaxNode syntax, BoundExpression left, BoundExpression right) => Binary(syntax, left, SyntaxType.LessOrEqualsToken, right);
        public static BoundExpressionStatement Increment(SyntaxNode syntax, BoundVariableExpression variable)
        {
            var increment = Add(syntax, variable, Literal(syntax, 1));
            var incrementAssign = new BoundAssignmentExpression(syntax, variable.Variable as VariableSymbol, increment);

            return new BoundExpressionStatement(syntax, incrementAssign);
        }
        public static BoundUnaryExpression Not(SyntaxNode syntax, BoundExpression condition)
        {
            Console.WriteLine(condition.ValueType);
            Debug.Assert(condition.ValueType == TypeSymbol.Bool);

            var op = BoundUnaryOperator.Bind(SyntaxType.BangToken, TypeSymbol.Bool);
            Debug.Assert(op != null);
            return new BoundUnaryExpression(syntax, op, condition);
        }
        public static BoundVariableExpression Variable(SyntaxNode syntax, BoundVariableDeclaration variable) => Variable(syntax, variable);
        public static BoundVariableExpression Variable(SyntaxNode syntax, VariableSymbol variable) => new BoundVariableExpression(syntax, variable);
        public static BoundLiteralExpression Literal(SyntaxNode syntax, object literal)
        {
            Debug.Assert(literal is string || literal is bool || literal is int);

            return new BoundLiteralExpression(syntax, literal);
        }
    }
}
