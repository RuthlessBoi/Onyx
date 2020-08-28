using Onyx.Binding.Nodes;
using Onyx.Binding.Nodes.Expressions;
using Onyx.Binding.Nodes.Statements;
using Onyx.Binding.Symbols;
using Onyx.Syntax;
using Onyx.Syntax.Nodes;
using Onyx.Syntax.Nodes.Statements;
using System.Collections.Immutable;

namespace Onyx.Binding
{
    internal sealed partial class Binder
    {
        private BoundStatement BindErrorStatement(SyntaxNode syntax) => new BoundExpressionStatement(syntax, new BoundErrorExpression(syntax));
        private BoundStatement BindGlobalStatement(StatementSyntax syntax) => BindStatement(syntax, isGlobal: true);
        private BoundStatement BindBlockStatement(BlockStatementSyntax syntax)
        {
            var statements = ImmutableArray.CreateBuilder<BoundStatement>();
            scope = new BoundScope(scope);

            foreach (var statementSyntax in syntax.Statements)
            {
                var statement = BindStatement(statementSyntax);
                statements.Add(statement);
            }

            scope = scope.Parent!;

            return new BoundBlockStatement(syntax, statements.ToImmutable());
        }
        private BoundStatement BindIfStatement(IfStatementSyntax syntax)
        {
            var condition = BindExpression(syntax.Condition, TypeSymbol.Bool);

            if (condition.ConstantValue != null)
            {
                if ((bool)condition.ConstantValue.Value == false)
                    diagnostics.ReportUnreachableCode(syntax.ThenStatement);
                else if (syntax.ElseDeclaration != null)
                    diagnostics.ReportUnreachableCode(syntax.ElseDeclaration.ElseStatement);
            }

            var thenStatement = BindStatement(syntax.ThenStatement);
            var elseStatement = syntax.ElseDeclaration == null ? null : BindStatement(syntax.ElseDeclaration.ElseStatement);

            return new BoundIfStatement(syntax, condition, thenStatement, elseStatement);
        }
        private BoundStatement BindWhileStatement(WhileStatementSyntax syntax)
        {
            var condition = BindExpression(syntax.Condition, TypeSymbol.Bool);

            if (condition.ConstantValue != null)
            {
                if (!(bool)condition.ConstantValue.Value)
                {
                    diagnostics.ReportUnreachableCode(syntax.Body);
                }
            }

            var body = BindLoopBody(syntax.Body, out var breakLabel, out var continueLabel);

            return new BoundWhileStatement(syntax, condition, body, breakLabel, continueLabel);
        }
        private BoundStatement BindDoWhileStatement(DoWhileStatementSyntax syntax)
        {
            var body = BindLoopBody(syntax.Body, out var breakLabel, out var continueLabel);
            var condition = BindExpression(syntax.Condition, TypeSymbol.Bool);

            return new BoundDoWhileStatement(syntax, body, condition, breakLabel, continueLabel);
        }
        private BoundStatement BindForStatement(ForStatementSyntax syntax)
        {
            var lowerBound = BindExpression(syntax.LowerBound, TypeSymbol.Int);
            var upperBound = BindExpression(syntax.UpperBound, TypeSymbol.Int);

            scope = new BoundScope(scope);

            var variable = BindVariableDeclaration(syntax.Identifier, isReadOnly: true, TypeSymbol.Int);
            var body = BindLoopBody(syntax.Body, out var breakLabel, out var continueLabel);

            scope = scope.Parent!;

            return new BoundForStatement(syntax, variable, lowerBound, upperBound, body, breakLabel, continueLabel);
        }
        private BoundStatement BindLoopBody(StatementSyntax body, out BoundLabel breakLabel, out BoundLabel continueLabel)
        {
            labelCounter++;

            breakLabel = new BoundLabel($"break{labelCounter}");
            continueLabel = new BoundLabel($"continue{labelCounter}");

            loopStack.Push((breakLabel, continueLabel));
            var boundBody = BindStatement(body);
            loopStack.Pop();

            return boundBody;
        }
        private BoundStatement BindBreakStatement(BreakStatementSyntax syntax)
        {
            if (loopStack.Count == 0)
            {
                diagnostics.ReportInvalidBreakOrContinue(syntax.Keyword.Location, syntax.Keyword.Text);

                return BindErrorStatement(syntax);
            }

            var breakLabel = loopStack.Peek().BreakLabel;

            return new BoundGotoStatement(syntax, breakLabel);
        }
        private BoundStatement BindContinueStatement(ContinueStatementSyntax syntax)
        {
            if (loopStack.Count == 0)
            {
                diagnostics.ReportInvalidBreakOrContinue(syntax.Keyword.Location, syntax.Keyword.Text);

                return BindErrorStatement(syntax);
            }

            var continueLabel = loopStack.Peek().ContinueLabel;

            return new BoundGotoStatement(syntax, continueLabel);
        }
        private BoundStatement BindReturnStatement(ReturnStatementSyntax syntax)
        {
            var expression = syntax.Expression == null ? null : BindExpression(syntax.Expression);

            if (function == null)
            {
                if (isScript)
                {
                    // Ignore because we allow both return with and without values.
                    if (expression == null)
                        expression = new BoundLiteralExpression(syntax, "");
                }
                else if (expression != null)
                    // Main does not support return values.
                    diagnostics.ReportInvalidReturnWithValueInGlobalStatements(syntax.Expression!.Location);
            }
            else
            {
                if (function.ValueType == TypeSymbol.Void)
                {
                    if (expression != null)
                        diagnostics.ReportInvalidReturnExpression(syntax.Expression!.Location, function.Name);
                }
                else
                {
                    if (expression == null)
                        diagnostics.ReportMissingReturnExpression(syntax.ReturnKeyword.Location, function.ValueType);
                    else
                        expression = BindConversion(syntax.Expression!.Location, expression, function.ValueType);
                }
            }

            return new BoundReturnStatement(syntax, expression);
        }
        private BoundStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
        {
            var expression = BindExpression(syntax.Expression, canBeVoid: true);

            return new BoundExpressionStatement(syntax, expression);
        }
    }
}
