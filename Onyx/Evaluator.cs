using Onyx.Binding;
using Onyx.Binding.Nodes;
using Onyx.Binding.Nodes.Expressions;
using Onyx.Binding.Nodes.Statements;
using Onyx.Binding.Symbols;
using Onyx.Syntax.Nodes;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Onyx
{
    // TODO: Convert most object/object? to OnyxValue
    internal sealed class Evaluator
    {
        private readonly BoundProgram program;
        private readonly Dictionary<VariableSymbol, OnyxValue> globals;
        private readonly Dictionary<FunctionSymbol, BoundBlockStatement> functions = new Dictionary<FunctionSymbol, BoundBlockStatement>();
        private readonly Stack<Dictionary<VariableSymbol, OnyxValue>> locals = new Stack<Dictionary<VariableSymbol, OnyxValue>>();

        private object? lastValue;

        public Evaluator(BoundProgram program, Dictionary<VariableSymbol, OnyxValue> variables)
        {
            this.program = program;
            globals = variables;
            locals.Push(new Dictionary<VariableSymbol, OnyxValue>());

            var current = program;
            while (current != null)
            {
                foreach (var kv in current.Functions)
                {
                    var function = kv.Key;
                    var body = kv.Value;
                    functions.Add(function, body);
                }

                current = current.Previous;
            }

            BuiltinFunctions.Initialize(this);
        }

        public object? Evaluate()
        {
            var function = program.MainFunction ?? program.ScriptFunction;

            if (function == null)
                return null;

            var body = functions[function];

            return EvaluateStatement(body);
        }
        private object? EvaluateStatement(BoundBlockStatement body)
        {
            var labelToIndex = new Dictionary<BoundLabel, int>();

            for (var i = 0; i < body.Statements.Length; i++)
            {
                if (body.Statements[i] is BoundLabelStatement l)
                    labelToIndex.Add(l.Label, i + 1);
            }

            var index = 0;

            while (index < body.Statements.Length)
            {
                var s = body.Statements[index];

                switch (s.Type)
                {
                    case BoundNodeType.NopStatement:
                        index++;
                        break;
                    case BoundNodeType.VariableDeclaration:
                        EvaluateVariableDeclaration((BoundVariableDeclaration)s);
                        index++;
                        break;
                    case BoundNodeType.ExpressionStatement:
                        EvaluateExpressionStatement((BoundExpressionStatement)s);
                        index++;
                        break;
                    case BoundNodeType.GotoStatement:
                        var gs = (BoundGotoStatement)s;
                        index = labelToIndex[gs.Label];
                        break;
                    case BoundNodeType.ConditionalGotoStatement:
                        var cgs = (BoundConditionalGotoStatement)s;
                        var condition = (bool)EvaluateExpression(cgs.Condition);
                        if (condition == cgs.JumpIfTrue)
                            index = labelToIndex[cgs.Label];
                        else
                            index++;
                        break;
                    case BoundNodeType.LabelStatement:
                        index++;
                        break;
                    case BoundNodeType.ReturnStatement:
                        var rs = (BoundReturnStatement)s;
                        lastValue = rs.Expression == null ? null : EvaluateExpression(rs.Expression);
                        return lastValue;
                    default:
                        throw new Exception($"Unexpected node {s.Type}");
                }
            }

            return lastValue;
        }
        private void EvaluateVariableDeclaration(BoundVariableDeclaration node)
        {
            var value = EvaluateExpression(node.Initializer);
            Debug.Assert(value != null);

            lastValue = value;
            Assign(node.Variable, value);
        }
        private void EvaluateExpressionStatement(BoundExpressionStatement node) => lastValue = EvaluateExpression(node.Expression);
        internal object? EvaluateExpression(BoundExpression node)
        {
            if (node.ConstantValue != null)
                return EvaluateConstantExpression(node);

            switch (node.Type)
            {
                case BoundNodeType.VariableExpression:
                    return EvaluateVariableExpression((BoundVariableExpression)node);
                case BoundNodeType.NewExpression:
                    return EvaluateNewExpression((BoundNewExpression)node);
                case BoundNodeType.DotExpression:
                    return EvaluateDotExpression((BoundDotExpression)node);
                case BoundNodeType.AssignmentExpression:
                    return EvaluateAssignmentExpression((BoundAssignmentExpression)node);
                case BoundNodeType.UnaryExpression:
                    return EvaluateUnaryExpression((BoundUnaryExpression)node);
                case BoundNodeType.BinaryExpression:
                    return EvaluateBinaryExpression((BoundBinaryExpression)node);
                case BoundNodeType.CallExpression:
                    return EvaluateCallExpression((BoundCallExpression)node);
                case BoundNodeType.ConversionExpression:
                    return EvaluateConversionExpression((BoundConversionExpression)node);
                case BoundNodeType.ErrorExpression:
                    throw new Exception($"Error: {node.Syntax}");
                default:
                    throw new Exception($"Unexpected node {node.Type}");
            }
        }
        private static object EvaluateConstantExpression(BoundExpression n)
        {
            Debug.Assert(n.ConstantValue != null);

            return n.ConstantValue.Value;
        }
        private object EvaluateVariableExpression(BoundVariableExpression v)
        {
            if (v.Variable.Type == SymbolType.GlobalVariable)
                return globals[v.Variable].Value;
            else
            {
                var locals = this.locals.Peek();
                var value = locals[v.Variable];

                if (v.Syntax is NameExpressionSyntax nes && nes.Modifier is IndexerModifierSyntax ims && value.Value is BoundArray ba)
                    return ba.Get((int)ims.IndexToken.Value);

                return value.Value;
            }
        }
        private object EvaluateNewExpression(BoundNewExpression n)
        {
            object? value = null;

            if (n.Initializer is BoundTemplateInitializerExpression template)
                value = template.Type.New(EvaluateTemplateInitializer(template.Arguments), template.References);
            else if (n.Initializer is BoundArrayInitializerExpression array)
                value = array.Type.New(array.Arguments, array.Size);

            return value;
        }
        private ImmutableArray<KeyValuePair<string, OnyxValue>> EvaluateTemplateInitializer(ImmutableArray<BoundTemplateInitializer> arguments)
        {
            var array = ImmutableArray.CreateBuilder<KeyValuePair<string, OnyxValue>>();

            foreach (var argument in arguments)
            {
                var expression = EvaluateExpression(argument.Expression);
                var type = argument.Expression.ValueType;
                var value = new OnyxValue(expression, type);
                var pair = new KeyValuePair<string, OnyxValue>(argument.Identifier.Text, value);

                array.Add(pair);
            }

            return array.ToImmutable();
        }
        private object EvaluateDotExpression(BoundDotExpression d) => EvaluateVariableChain(d.Chain);
        private object EvaluateVariableChain(BoundVariableChain chain, OnyxValue? onxyValue = null)
        {
            var locals = this.locals.Peek();

            if (chain.Variable is VariableSymbol variable)
            {
                var value = onxyValue != null ? onxyValue.Get(variable) : locals[variable];

                if (chain.HasChild)
                    return EvaluateVariableChain(chain.Child, value);

                return value?.Value ?? null;
            }
            else if (chain.Variable is FunctionSymbol function && chain.Syntax is BoundCallExpression call)
            {
                var value = NewValue(function.ValueType, onxyValue.Get(function).Invoke(call));

                if (chain.HasChild)
                    return EvaluateVariableChain(chain.Child, value);

                return value?.Value ?? null;
            }

            return null;
        }
        private object EvaluateAssignmentExpression(BoundAssignmentExpression a)
        {
            var value = EvaluateExpression(a.Expression);
            Debug.Assert(value != null);

            Assign(a.Variable, value);

            return value;
        }
        private object EvaluateUnaryExpression(BoundUnaryExpression u)
        {
            var operand = EvaluateExpression(u.Operand);

            Debug.Assert(operand != null);

            switch (u.Op.Type)
            {
                case BoundUnaryOperatorType.Identity:
                    return (int)operand;
                case BoundUnaryOperatorType.Negation:
                    return -(int)operand;
                case BoundUnaryOperatorType.LogicalNegation:
                    return !(bool)operand;
                case BoundUnaryOperatorType.OnesComplement:
                    return ~(int)operand;
                default:
                    throw new Exception($"Unexpected unary operator {u.Op}");
            }
        }
        private object EvaluateBinaryExpression(BoundBinaryExpression b)
        {
            var left = EvaluateExpression(b.Left);
            var right = EvaluateExpression(b.Right);

            Debug.Assert(left != null && right != null);

            switch (b.Op.Type)
            {
                case BoundBinaryOperatorType.Addition:
                    if (b.ValueType == TypeSymbol.Int)
                        return (int)left + (int)right;
                    else
                        return (string)left + (string)right;
                case BoundBinaryOperatorType.Subtraction:
                    return (int)left - (int)right;
                case BoundBinaryOperatorType.Multiplication:
                    return (int)left * (int)right;
                case BoundBinaryOperatorType.Division:
                    return (int)left / (int)right;
                case BoundBinaryOperatorType.BitwiseAnd:
                    if (b.ValueType == TypeSymbol.Int)
                        return (int)left & (int)right;
                    else
                        return (bool)left & (bool)right;
                case BoundBinaryOperatorType.BitwiseOr:
                    if (b.ValueType == TypeSymbol.Int)
                        return (int)left | (int)right;
                    else
                        return (bool)left | (bool)right;
                case BoundBinaryOperatorType.BitwiseXor:
                    if (b.ValueType == TypeSymbol.Int)
                        return (int)left ^ (int)right;
                    else
                        return (bool)left ^ (bool)right;
                case BoundBinaryOperatorType.LogicalAnd:
                    return (bool)left && (bool)right;
                case BoundBinaryOperatorType.LogicalOr:
                    return (bool)left || (bool)right;
                case BoundBinaryOperatorType.Equals:
                    return Equals(left, right);
                case BoundBinaryOperatorType.NotEquals:
                    return !Equals(left, right);
                case BoundBinaryOperatorType.Less:
                    return (int)left < (int)right;
                case BoundBinaryOperatorType.LessOrEquals:
                    return (int)left <= (int)right;
                case BoundBinaryOperatorType.Greater:
                    return (int)left > (int)right;
                case BoundBinaryOperatorType.GreaterOrEquals:
                    return (int)left >= (int)right;
                case BoundBinaryOperatorType.Is:
                    return left.GetType() == (right as TypeContainer).InternalType;
                default:
                    throw new Exception($"Unexpected binary operator {b.Op}");
            }
        }
        private object? EvaluateCallExpression(BoundCallExpression node)
        {
            if (BuiltinFunctions.HasFunction(node.Function))
                return BuiltinFunctions.ExecuteFunction(node);
            else
            {
                var locals = new Dictionary<VariableSymbol, OnyxValue>();
                for (int i = 0; i < node.Arguments.Length; i++)
                {
                    var parameter = node.Function.Parameters[i];
                    var value = EvaluateExpression(node.Arguments[i]);
                    Debug.Assert(value != null);
                    locals.Add(parameter, NewValue(parameter.ValueType, value));
                }

                this.locals.Push(locals);

                var statement = functions[node.Function];
                var result = EvaluateStatement(statement);

                this.locals.Pop();

                return result;
            }
        }
        private object? EvaluateConversionExpression(BoundConversionExpression node)
        {
            var value = EvaluateExpression(node.Expression);

            if (node.ValueType == TypeSymbol.Any)
                return value;
            else if (node.ValueType is TemplateSymbol mts)
                return mts;
            else if (node.ValueType is ArrayType array)
                return array;
            else if (node.ValueType is GenericsSymbol generics)
                return generics;
            else if (node.ValueType == TypeSymbol.Bool)
                return Convert.ToBoolean(value);
            else if (node.ValueType == TypeSymbol.Int)
                return Convert.ToInt32(value);
            else if (node.ValueType == TypeSymbol.String)
                return Convert.ToString(value);
            else if (node.ValueType == TypeSymbol.Char)
                return Convert.ToChar(value);
            else
                throw new Exception($"Unexpected type {node.ValueType}");
        }
        private void Assign(VariableSymbol variable, object value)
        {
            if (variable.Type == SymbolType.GlobalVariable)
                globals[variable] = NewValue(variable.ValueType, value);
            else
            {
                var locals = this.locals.Peek();
                locals[variable] = NewValue(variable.ValueType, value);
            }
        }
        private OnyxValue NewValue(TypeSymbol type, object value)
        {
            var onyxValue = new OnyxValue(value, type);

            // TODO: create a better way of binding runtime functions/variables to types
            if (value is string str)
            {
                onyxValue.Assign(TypeSymbol.LengthSymbol, NewValue(TypeSymbol.Int, str.Length));
                onyxValue.Assign(TypeSymbol.StringAsCharArray, NewArray(str.ToCharArray(), TypeSymbol.Char, str.Length));
                onyxValue.Assign(TypeSymbol.StringGetChar, node =>
                {
                    var index = (int)EvaluateExpression(node.Arguments[0])!;

                    return str[index];
                });
            } 
            else if (value is BoundArray array)
            {
                onyxValue.Assign(TypeSymbol.LengthSymbol, NewValue(TypeSymbol.Int, array.Size));
            }

            return onyxValue;
        }
        private OnyxValue NewArray<T>(IEnumerable<T> enumerable, TypeSymbol type, int size)
        {
            var array = new BoundArray(type.Array, size);
            var index = 0;

            foreach (var obj in enumerable)
                array.Assign(index++, NewValue(type, obj));

            return NewValue(type.Array, array);
        }
    }
}