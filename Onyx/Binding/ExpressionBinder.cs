using Onyx.Binding.Nodes;
using Onyx.Binding.Nodes.Expressions;
using Onyx.Binding.Symbols;
using Onyx.Syntax;
using Onyx.Syntax.Nodes;
using Onyx.Text;
using System.Collections.Immutable;
using System.Linq;

namespace Onyx.Binding
{
    internal sealed partial class Binder
    {
        private BoundExpression BindTypeofExpression(TypeofExpressionSyntax syntax)
        {
            var type = LookupType(syntax.InternalType.Identifier.Text, syntax.InternalType.IsArray);

            return new BoundLiteralExpression(syntax, type.ContainedType);
        }
        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            if (syntax.IdentifierToken.IsMissing)
                // This means the token was inserted by the parser. We already
                // reported error so we can just return an error expression.
                return new BoundErrorExpression(syntax);

            var variable = BindVariableReference(syntax.IdentifierToken);
            //var references = BindVariableChain(syntax);

            if (variable == null)
                return new BoundErrorExpression(syntax);

            return new BoundVariableExpression(syntax, variable);
        }
        private BoundExpression BindDotExpression(DotExpressionSyntax syntax)
        {
            if (syntax.LeftToken.IsMissing)
                // This means the token was inserted by the parser. We already
                // reported error so we can just return an error expression.
                return new BoundErrorExpression(syntax);

            var reference = BindDotReference(syntax);

            if (reference == null)
                return new BoundErrorExpression(syntax);

            return new BoundDotExpression(syntax, reference);
        }
        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            var name = syntax.Identifier.Text;
            var boundExpression = BindExpression(syntax.Expression);

            var variable = BindVariableReference(syntax.Identifier);

            if (variable == null)
                return boundExpression;

            if (variable.IsReadOnly)
                diagnostics.ReportCannotAssign(syntax.AssignmentToken.Location, name);

            if (syntax.AssignmentToken.Type != SyntaxType.EqualsToken)
            {
                var equivalentOperatorTokenKind = SyntaxFacts.GetBinaryOperatorOfAssignmentOperator(syntax.AssignmentToken.Type);
                var boundOperator = BoundBinaryOperator.Bind(equivalentOperatorTokenKind, variable.ValueType, boundExpression.ValueType);

                if (boundOperator == null)
                {
                    diagnostics.ReportUndefinedBinaryOperator(syntax.AssignmentToken.Location, syntax.AssignmentToken.Text, variable.ValueType, boundExpression.ValueType);
                    return new BoundErrorExpression(syntax);
                }

                var convertedExpression = BindConversion(syntax.Expression.Location, boundExpression, variable.ValueType);

                return new BoundCompoundAssignmentExpression(syntax, variable, boundOperator, convertedExpression);
            }
            else
            {
                var convertedExpression = BindConversion(syntax.Expression.Location, boundExpression, variable.ValueType);
                return new BoundAssignmentExpression(syntax, variable, convertedExpression);
            }
        }
        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var boundOperand = BindExpression(syntax.Operand);

            if (boundOperand.ValueType == TypeSymbol.Error)
                return new BoundErrorExpression(syntax);

            var boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Type, boundOperand.ValueType);

            if (boundOperator == null)
            {
                diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Location, syntax.OperatorToken.Text, boundOperand.ValueType);

                return new BoundErrorExpression(syntax);
            }

            return new BoundUnaryExpression(syntax, boundOperator, boundOperand);
        }
        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);

            if (boundLeft.ValueType == TypeSymbol.Error || boundRight.ValueType == TypeSymbol.Error)
                return new BoundErrorExpression(syntax);

            var boundOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Type, boundLeft.ValueType, boundRight.ValueType);

            if (boundOperator == null)
            {
                diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Location, syntax.OperatorToken.Text, boundLeft.ValueType, boundRight.ValueType);

                return new BoundErrorExpression(syntax);
            }

            return new BoundBinaryExpression(syntax, boundLeft, boundOperator, boundRight);
        }
        private BoundExpression BindCallExpression(CallExpressionSyntax syntax, FunctionSymbol? known = null)
        {
            if (syntax.Arguments.Count == 1 && LookupType(syntax.IdentifierToken.Text, false) is TypeSymbol type)
                return BindConversion(syntax.Arguments[0], type, allowExplicit: true);

            var boundArguments = ImmutableArray.CreateBuilder<BoundExpression>();
            var boundTypes = ImmutableArray.CreateBuilder<TypeSymbol>();

            foreach (var argument in syntax.Arguments)
            {
                var boundArgument = BindExpression(argument);

                boundArguments.Add(boundArgument);
                boundTypes.Add(boundArgument.ValueType);
            }

            var symbol = known ?? scope.TryLookupSymbol(syntax.IdentifierToken.Text);

            if (symbol == null)
            {
                diagnostics.ReportUndefinedFunction(syntax.IdentifierToken.Location, syntax.IdentifierToken.Text);
                return new BoundErrorExpression(syntax);
            }

            var function = symbol as FunctionSymbol;

            if (function == null)
            {
                diagnostics.ReportNotAFunction(syntax.IdentifierToken.Location, syntax.IdentifierToken.Text);
                return new BoundErrorExpression(syntax);
            }

            if (syntax.Arguments.Count != function.Parameters.Length)
            {
                TextSpan span;

                if (syntax.Arguments.Count > function.Parameters.Length)
                {
                    SyntaxNode firstExceedingNode;

                    if (function.Parameters.Length > 0)
                        firstExceedingNode = syntax.Arguments.GetSeparator(function.Parameters.Length - 1);
                    else
                        firstExceedingNode = syntax.Arguments[0];

                    var lastExceedingArgument = syntax.Arguments[syntax.Arguments.Count - 1];
                    span = TextSpan.FromBounds(firstExceedingNode.Span.Start, lastExceedingArgument.Span.End);
                }
                else
                    span = syntax.CloseParenthesisToken.Span;

                var location = new TextLocation(syntax.SyntaxTree.Text, span);
                diagnostics.ReportWrongArgumentCount(location, function.Name, function.Parameters.Length, syntax.Arguments.Count);

                return new BoundErrorExpression(syntax);
            }

            for (var i = 0; i < syntax.Arguments.Count; i++)
            {
                var argumentLocation = syntax.Arguments[i].Location;
                var argument = boundArguments[i];
                var parameter = function.Parameters[i];

                boundArguments[i] = BindConversion(argumentLocation, argument, parameter.ValueType);
            }

            if (function.HasAnnotation("deprecated"))
                diagnostics.ReportCalledDeprecatedFunction(syntax.Location, function.GetUniqueName());

            return new BoundCallExpression(syntax, function, boundArguments.ToImmutable());
        }
        private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax) => BindExpression(syntax.Expression);
        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var value = syntax.Value ?? 0;

            return new BoundLiteralExpression(syntax, value);
        }
        private BoundExpression BindNewExpression(NewExpressionSyntax syntax)
        {
            var internalType = syntax.InternalType;
            var type = LookupType(internalType.Identifier.Text, internalType.IsArray);

            if (type is TemplateSymbol template && syntax.Initializer is TemplateInitializerSyntax templateInitializer)
            {
                var references = BindGenericsReferences(internalType);
                var arguments = BindModelArguments(template, templateInitializer);

                return new BoundNewExpression(syntax, new BoundTemplateInitializerExpression(template, arguments, references), template);
            }
            else if (type is ArrayType array && syntax.Initializer is ArrayInitializerSyntax arrayInitializer)
                return new BoundNewExpression(syntax, new BoundArrayInitializerExpression(array, BindArrayArguments(arrayInitializer), (int)internalType.SizeToken.Value), array);
            else
                diagnostics.ReportNotAObject(syntax.InternalType.Identifier.Location, type);

            return new BoundErrorExpression(syntax);
        }
        private ImmutableArray<TypeSymbol> BindGenericsReferences(TypeSyntax syntax)
        {
            var array = ImmutableArray.CreateBuilder<TypeSymbol>();

            foreach (var generics in syntax.GenericsArguments.GenericsArguments)
            {
                var type = LookupType(generics.Identifier.Text, generics.IsArray);

                array.Add(type);
            }

            return array.ToImmutable();
        }
        private ImmutableArray<BoundTemplateInitializer> BindModelArguments(TemplateSymbol template, TemplateInitializerSyntax? syntax)
        {
            var array = ImmutableArray.CreateBuilder<BoundTemplateInitializer>();
            var variables = template.GetDeclaredVariables();

            if (syntax == null)
                return ImmutableArray<BoundTemplateInitializer>.Empty;

            for (int i = 0; i < variables.Count(); i++)
            {
                var parameter = syntax.Parameters[i];
                var type = variables[i].ValueType;
                var value = parameter.Value;
                var expression = type.IsGeneric ? BindExpression(value) : BindExpression(value, type);

                array.Add(new BoundTemplateInitializer(expression, parameter.Identifier, type.IsGeneric));
            }

            return array.ToImmutable();
        }
        private ImmutableArray<BoundExpression> BindArrayArguments(ArrayInitializerSyntax? syntax)
        {
            var array = ImmutableArray.CreateBuilder<BoundExpression>();

            foreach (var argument in syntax.Arguments)
                array.Add(BindExpression(argument));

            return array.ToImmutable();
        }
    }
}
