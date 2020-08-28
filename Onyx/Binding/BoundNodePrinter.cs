using Onyx.Binding.Symbols;
using Onyx.Syntax;
using Onyx.Binding.Nodes;
using Onyx.Binding.Nodes.Statements;
using Onyx.Binding.Nodes.Expressions;
using System;
using System.IO;
using System.CodeDom.Compiler;

namespace Onyx.Binding
{
    internal static class BoundNodePrinter
    {
        public static void WriteTo(this BoundNode node, TextWriter writer)
        {
            if (writer is IndentedTextWriter iw)
                WriteTo(node, iw);
            else
                WriteTo(node, new IndentedTextWriter(writer));
        }
        public static void WriteTo(this BoundNode node, IndentedTextWriter writer)
        {
            switch (node.Type)
            {
                case BoundNodeType.BlockStatement:
                    WriteBlockStatement((BoundBlockStatement)node, writer);
                    break;
                case BoundNodeType.NopStatement:
                    WriteNopStatement((BoundNopStatement)node, writer);
                    break;
                case BoundNodeType.VariableDeclaration:
                    WriteVariableDeclaration((BoundVariableDeclaration)node, writer);
                    break;
                case BoundNodeType.IfStatement:
                    WriteIfStatement((BoundIfStatement)node, writer);
                    break;
                case BoundNodeType.WhileStatement:
                    WriteWhileStatement((BoundWhileStatement)node, writer);
                    break;
                case BoundNodeType.DoWhileStatement:
                    WriteDoWhileStatement((BoundDoWhileStatement)node, writer);
                    break;
                case BoundNodeType.ForStatement:
                    WriteForStatement((BoundForStatement)node, writer);
                    break;
                case BoundNodeType.LabelStatement:
                    WriteLabelStatement((BoundLabelStatement)node, writer);
                    break;
                case BoundNodeType.GotoStatement:
                    WriteGotoStatement((BoundGotoStatement)node, writer);
                    break;
                case BoundNodeType.ConditionalGotoStatement:
                    WriteConditionalGotoStatement((BoundConditionalGotoStatement)node, writer);
                    break;
                case BoundNodeType.ReturnStatement:
                    WriteReturnStatement((BoundReturnStatement)node, writer);
                    break;
                case BoundNodeType.ExpressionStatement:
                    WriteExpressionStatement((BoundExpressionStatement)node, writer);
                    break;
                case BoundNodeType.ErrorExpression:
                    WriteErrorExpression((BoundErrorExpression)node, writer);
                    break;
                case BoundNodeType.LiteralExpression:
                    WriteLiteralExpression((BoundLiteralExpression)node, writer);
                    break;
                case BoundNodeType.VariableExpression:
                    WriteVariableExpression((BoundVariableExpression)node, writer);
                    break;
                case BoundNodeType.AssignmentExpression:
                    WriteAssignmentExpression((BoundAssignmentExpression)node, writer);
                    break;
                case BoundNodeType.CompoundAssignmentExpression:
                    WriteCompoundAssignmentExpression((BoundCompoundAssignmentExpression)node, writer);
                    break;
                case BoundNodeType.UnaryExpression:
                    WriteUnaryExpression((BoundUnaryExpression)node, writer);
                    break;
                case BoundNodeType.BinaryExpression:
                    WriteBinaryExpression((BoundBinaryExpression)node, writer);
                    break;
                case BoundNodeType.CallExpression:
                    WriteCallExpression((BoundCallExpression)node, writer);
                    break;
                case BoundNodeType.ConversionExpression:
                    WriteConversionExpression((BoundConversionExpression)node, writer);
                    break;
                default:
                    throw new Exception($"Unexpected node {node.Type}");
            }
        }
        private static void WriteNestedStatement(this IndentedTextWriter writer, BoundStatement node)
        {
            var needsIndentation = !(node is BoundBlockStatement);

            if (needsIndentation)
                writer.Indent++;

            node.WriteTo(writer);

            if (needsIndentation)
                writer.Indent--;
        }
        private static void WriteNestedExpression(this IndentedTextWriter writer, int parentPrecedence, BoundExpression expression)
        {
            if (expression is BoundUnaryExpression unary)
                writer.WriteNestedExpression(parentPrecedence, SyntaxFacts.GetUnaryOperatorPrecedence(unary.Op.SyntaxType), unary);
            else if (expression is BoundBinaryExpression binary)
                writer.WriteNestedExpression(parentPrecedence, SyntaxFacts.GetBinaryOperatorPrecedence(binary.Op.SyntaxType), binary);
            else
                expression.WriteTo(writer);
        }
        private static void WriteNestedExpression(this IndentedTextWriter writer, int parentPrecedence, int currentPrecedence, BoundExpression expression)
        {
            var needsParenthesis = parentPrecedence >= currentPrecedence;

            if (needsParenthesis)
                writer.WritePunctuation(SyntaxType.LeftParenthesisToken);

            expression.WriteTo(writer);

            if (needsParenthesis)
                writer.WritePunctuation(SyntaxType.RightParenthesisToken);
        }
        private static void WriteBlockStatement(BoundBlockStatement node, IndentedTextWriter writer)
        {
            writer.WritePunctuation(SyntaxType.LeftBraceToken);
            writer.WriteLine();
            writer.Indent++;

            foreach (var s in node.Statements)
                s.WriteTo(writer);

            writer.Indent--;
            writer.WritePunctuation(SyntaxType.RightBraceToken);
            writer.WriteLine();
        }
        private static void WriteNopStatement(BoundNopStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword("nop");
            writer.WriteLine();
        }
        private static void WriteVariableDeclaration(BoundVariableDeclaration node, IndentedTextWriter writer)
        {
            writer.WriteKeyword(node.Variable.IsReadOnly ? SyntaxType.LetKeyword : SyntaxType.VarKeyword);
            writer.WriteSpace();
            writer.WriteIdentifier(node.Variable.Name);
            writer.WriteSpace();
            writer.WritePunctuation(SyntaxType.EqualsToken);
            writer.WriteSpace();
            node.Initializer.WriteTo(writer);
            writer.WriteLine();
        }
        private static void WriteIfStatement(BoundIfStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword(SyntaxType.IfKeyword);
            writer.WriteSpace();
            node.Condition.WriteTo(writer);
            writer.WriteLine();
            writer.WriteNestedStatement(node.ThenStatement);

            if (node.ElseStatement != null)
            {
                writer.WriteKeyword(SyntaxType.ElseKeyword);
                writer.WriteLine();
                writer.WriteNestedStatement(node.ElseStatement);
            }
        }
        private static void WriteWhileStatement(BoundWhileStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword(SyntaxType.WhileKeyword);
            writer.WriteSpace();
            node.Condition.WriteTo(writer);
            writer.WriteLine();
            writer.WriteNestedStatement(node.Body);
        }
        private static void WriteDoWhileStatement(BoundDoWhileStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword(SyntaxType.DoKeyword);
            writer.WriteLine();
            writer.WriteNestedStatement(node.Body);
            writer.WriteKeyword(SyntaxType.WhileKeyword);
            writer.WriteSpace();
            node.Condition.WriteTo(writer);
            writer.WriteLine();
        }
        private static void WriteForStatement(BoundForStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword(SyntaxType.ForKeyword);
            writer.WriteSpace();
            writer.WriteIdentifier(node.Variable.Name);
            writer.WriteSpace();
            writer.WritePunctuation(SyntaxType.EqualsToken);
            writer.WriteSpace();
            node.LowerBound.WriteTo(writer);
            writer.WriteSpace();
            writer.WriteKeyword(SyntaxType.ToKeyword);
            writer.WriteSpace();
            node.UpperBound.WriteTo(writer);
            writer.WriteLine();
            writer.WriteNestedStatement(node.Body);
        }
        private static void WriteLabelStatement(BoundLabelStatement node, IndentedTextWriter writer)
        {
            var unindent = writer.Indent > 0;
            if (unindent)
                writer.Indent--;

            writer.WritePunctuation(node.Label.Name);
            writer.WritePunctuation(SyntaxType.ColonToken);
            writer.WriteLine();

            if (unindent)
                writer.Indent++;
        }
        private static void WriteGotoStatement(BoundGotoStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword("goto"); // There is no SyntaxKind for goto
            writer.WriteSpace();
            writer.WriteIdentifier(node.Label.Name);
            writer.WriteLine();
        }
        private static void WriteConditionalGotoStatement(BoundConditionalGotoStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword("goto"); // There is no SyntaxKind for goto
            writer.WriteSpace();
            writer.WriteIdentifier(node.Label.Name);
            writer.WriteSpace();
            writer.WriteKeyword(node.JumpIfTrue ? "if" : "unless");
            writer.WriteSpace();
            node.Condition.WriteTo(writer);
            writer.WriteLine();
        }
        private static void WriteReturnStatement(BoundReturnStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword(SyntaxType.ReturnKeyword);
            if (node.Expression != null)
            {
                writer.WriteSpace();
                node.Expression.WriteTo(writer);
            }
            writer.WriteLine();
        }
        private static void WriteExpressionStatement(BoundExpressionStatement node, IndentedTextWriter writer)
        {
            node.Expression.WriteTo(writer);
            writer.WriteLine();
        }
        private static void WriteErrorExpression(BoundErrorExpression node, IndentedTextWriter writer) => writer.WriteKeyword("?");
        private static void WriteLiteralExpression(BoundLiteralExpression node, IndentedTextWriter writer)
        {
            var value = node.Value.ToString()!;

            if (node.ValueType == TypeSymbol.Bool)
                writer.WriteKeyword((bool)node.Value ? SyntaxType.TrueKeyword : SyntaxType.FalseKeyword);
            else if (node.ValueType == TypeSymbol.Int)
                writer.WriteNumber(value);
            else if (node.ValueType == TypeSymbol.String)
            {
                value = "\"" + value.Replace("\"", "\"\"") + "\"";
                writer.WriteString(value);
            }
            else
                throw new Exception($"Unexpected type {node.Type}");
        }
        private static void WriteVariableExpression(BoundVariableExpression node, IndentedTextWriter writer) => writer.WriteIdentifier(node.Variable.Name);
        private static void WriteAssignmentExpression(BoundAssignmentExpression node, IndentedTextWriter writer)
        {
            writer.WriteIdentifier(node.Variable.Name);
            writer.WriteSpace();
            writer.WritePunctuation(SyntaxType.EqualsToken);
            writer.WriteSpace();
            node.Expression.WriteTo(writer);
        }
        private static void WriteCompoundAssignmentExpression(BoundCompoundAssignmentExpression node, IndentedTextWriter writer)
        {
            writer.WriteIdentifier(node.Variable.Name);
            writer.WriteSpace();
            writer.WritePunctuation(node.Op.SyntaxType);
            writer.WritePunctuation(SyntaxType.EqualsToken);
            writer.WriteSpace();
            node.Expression.WriteTo(writer);
        }
        private static void WriteUnaryExpression(BoundUnaryExpression node, IndentedTextWriter writer)
        {
            var precedence = SyntaxFacts.GetUnaryOperatorPrecedence(node.Op.SyntaxType);

            writer.WritePunctuation(node.Op.SyntaxType);
            writer.WriteNestedExpression(precedence, node.Operand);
        }
        private static void WriteBinaryExpression(BoundBinaryExpression node, IndentedTextWriter writer)
        {
            var precedence = SyntaxFacts.GetBinaryOperatorPrecedence(node.Op.SyntaxType);

            writer.WriteNestedExpression(precedence, node.Left);
            writer.WriteSpace();
            writer.WritePunctuation(node.Op.SyntaxType);
            writer.WriteSpace();
            writer.WriteNestedExpression(precedence, node.Right);
        }
        private static void WriteCallExpression(BoundCallExpression node, IndentedTextWriter writer)
        {
            writer.WriteIdentifier(node.Function.Name);
            writer.WritePunctuation(SyntaxType.LeftParenthesisToken);

            var isFirst = true;
            foreach (var argument in node.Arguments)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    writer.WritePunctuation(SyntaxType.CommaToken);
                    writer.WriteSpace();
                }

                argument.WriteTo(writer);
            }

            writer.WritePunctuation(SyntaxType.RightParenthesisToken);
        }
        private static void WriteConversionExpression(BoundConversionExpression node, IndentedTextWriter writer)
        {
            writer.WriteIdentifier(node.ValueType.Name);
            writer.WritePunctuation(SyntaxType.LeftParenthesisToken);
            node.Expression.WriteTo(writer);
            writer.WritePunctuation(SyntaxType.RightParenthesisToken);
        }
    }
}
