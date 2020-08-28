using Onyx.Binding.Symbols;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using Onyx.Syntax;
using Onyx.Binding.Nodes;
using Onyx.Syntax.Nodes.Members;
using Onyx.Binding.Nodes.Statements;
using Onyx.Binding.Nodes.Expressions;
using Onyx.Syntax.Nodes;
using Onyx.Syntax.Nodes.Statements;
using System;
using System.Diagnostics.CodeAnalysis;
using Onyx.Text;
using Onyx.Services.References;

namespace Onyx.Binding
{
    internal sealed partial class Binder
    {
        public DiagnosticsContainer Diagnostics => diagnostics;

        private readonly DiagnosticsContainer diagnostics = new DiagnosticsContainer();
        private readonly bool isScript;
        private readonly FunctionSymbol? function;

        private Stack<(BoundLabel BreakLabel, BoundLabel ContinueLabel)> loopStack = new Stack<(BoundLabel BreakLabel, BoundLabel ContinueLabel)>();
        private int labelCounter;
        private BoundScope scope;

        private Binder(bool isScript, BoundScope? parent, FunctionSymbol? function)
        {
            scope = new BoundScope(parent);
            this.isScript = isScript;
            this.function = function;

            if (function != null)
            {
                foreach (var p in function.Parameters)
                    scope.TryDeclareVariable(p);
            }
        }

        public static BoundGlobalScope BindGlobalScope(bool isScript, BoundGlobalScope? previous, ImmutableArray<SyntaxTree> syntaxTrees)
        {
            var parentScope = CreateParentScope(previous);
            var binder = new Binder(isScript, parentScope, function: null);

            binder.Diagnostics.AddRange(syntaxTrees.SelectMany(st => st.Diagnostics));

            if (binder.Diagnostics.Any())
                return new BoundGlobalScope(previous, binder.Diagnostics.ToImmutableArray(), null, null, ImmutableArray<BoundNamespace>.Empty, ImmutableArray<FunctionSymbol>.Empty, ImmutableArray<TemplateSymbol>.Empty, ImmutableArray<VariableSymbol>.Empty, ImmutableArray<BoundStatement>.Empty);

            var importDeclarations = syntaxTrees.SelectMany(st => st.Root.Members)
                                                  .OfType<ImportDeclarationSyntax>();

            foreach (var import in importDeclarations)
                binder.BindImportDeclaration(import);

            var namespaceDeclarations = syntaxTrees.SelectMany(st => st.Root.Members)
                                                  .OfType<NamespaceDeclarationSyntax>();

            foreach (var namespaceSyntax in namespaceDeclarations)
                binder.BindNamespaceDeclaration(namespaceSyntax);

            var annotationDeclarations = syntaxTrees.SelectMany(st => st.Root.Members)
                                                  .OfType<AnnotationDeclarationSyntax>();

            foreach (var annotation in annotationDeclarations)
                binder.BindAnnotationDeclaration(annotation);

            var templateDeclarations = syntaxTrees.SelectMany(st => st.Root.Members)
                                                  .OfType<TemplateDeclarationSyntax>();

            foreach (var template in templateDeclarations)
                binder.BindTemplateDeclaration(template);

            var functionDeclarations = syntaxTrees.SelectMany(st => st.Root.Members)
                                                  .OfType<FunctionDeclarationSyntax>();

            foreach (var function in functionDeclarations)
                binder.BindFunctionDeclaration(function);

            var globalStatements = syntaxTrees.SelectMany(st => st.Root.Members).OfType<GlobalStatementSyntax>();

            var statements = ImmutableArray.CreateBuilder<BoundStatement>();

            foreach (var globalStatement in globalStatements)
            {
                var statement = binder.BindGlobalStatement(globalStatement.Statement);
                statements.Add(statement);
            }

            // Check global statements

            var firstGlobalStatementPerSyntaxTree = syntaxTrees.Select(st => st.Root.Members.OfType<GlobalStatementSyntax>().FirstOrDefault()).Where(g => g != null) .ToArray();

            if (firstGlobalStatementPerSyntaxTree.Length > 1)
            {
                foreach (var globalStatement in firstGlobalStatementPerSyntaxTree)
                    binder.Diagnostics.ReportOnlyOneFileCanHaveGlobalStatements(globalStatement.Location);
            }

            // Check for main/script with global statements

            var namespaces = binder.scope.GetNamespaces();
            var functions = binder.scope.GetDeclaredFunctions();
            var templates = binder.scope.GetDeclaredTemplates();

            FunctionSymbol? mainFunction;
            FunctionSymbol? scriptFunction;

            if (isScript)
            {
                mainFunction = null;

                if (globalStatements.Any())
                    scriptFunction = new FunctionSymbol("$eval", ImmutableArray<ParameterSymbol>.Empty, TypeSymbol.Any, null);
                else
                    scriptFunction = null;
            }
            else
            {
                mainFunction = functions.FirstOrDefault(f => f.Name == "main");
                scriptFunction = null;

                if (mainFunction == null)
                    mainFunction = functions.FirstOrDefault(f => f.HasPragma("runtime:entry_point")); 

                if (mainFunction != null)
                {
                    if (mainFunction.ValueType != TypeSymbol.Void || mainFunction.Parameters.Any())
                        binder.Diagnostics.ReportMainMustHaveCorrectSignature(mainFunction.Declaration!.Identifier.Location);
                }

                if (globalStatements.Any())
                {
                    if (mainFunction != null)
                    {
                        binder.Diagnostics.ReportCannotMixMainAndGlobalStatements(mainFunction.Declaration!.Identifier.Location);

                        foreach (var globalStatement in firstGlobalStatementPerSyntaxTree)
                            binder.Diagnostics.ReportCannotMixMainAndGlobalStatements(globalStatement.Location);
                    }
                    else
                    {
                        mainFunction = new FunctionSymbol("main", ImmutableArray<ParameterSymbol>.Empty, TypeSymbol.Void, null);
                    }
                }
            }

            var diagnostics = binder.Diagnostics.ToImmutableArray();
            var variables = binder.scope.GetDeclaredVariables();

            if (previous != null)
                diagnostics = diagnostics.InsertRange(0, previous.Diagnostics);

            return new BoundGlobalScope(previous, diagnostics, mainFunction, scriptFunction, namespaces, functions, templates, variables, statements.ToImmutable());
        }

        public static BoundProgram BindProgram(bool isScript, BoundProgram? previous, BoundGlobalScope globalScope)
        {
            var parentScope = CreateParentScope(globalScope);

            if (globalScope.Diagnostics.Any())
                return new BoundProgram(previous, globalScope.Diagnostics, null, null, ImmutableDictionary<FunctionSymbol, BoundBlockStatement>.Empty);

            var functionBodies = ImmutableDictionary.CreateBuilder<FunctionSymbol, BoundBlockStatement>();
            var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();

            foreach (var function in globalScope.Functions)
            {
                var binder = new Binder(isScript, parentScope, function);
                var body = binder.BindStatement(function.Declaration!.Body);
                var loweredBody = Lowerer.Lower(function, body);

                if (function.ValueType != TypeSymbol.Void && !ControlFlowGraph.AllPathsReturn(loweredBody))
                    binder.diagnostics.ReportAllPathsMustReturn(function.Declaration.Identifier.Location);

                functionBodies.Add(function, loweredBody);

                diagnostics.AddRange(binder.Diagnostics);
            }

            var compilationUnit = globalScope.Statements.Any()
                                    ? globalScope.Statements.First().Syntax.AncestorsAndSelf().LastOrDefault()
                                    : null;

            if (globalScope.MainFunction != null && globalScope.Statements.Any())
            {
                var body = Lowerer.Lower(globalScope.MainFunction, new BoundBlockStatement(compilationUnit!, globalScope.Statements));
                functionBodies.Add(globalScope.MainFunction, body);
            }
            else if (globalScope.ScriptFunction != null)
            {
                var statements = globalScope.Statements;

                if (statements.Length == 1 && statements[0] is BoundExpressionStatement es && es.Expression.ValueType != TypeSymbol.Void)
                    statements = statements.SetItem(0, new BoundReturnStatement(es.Expression.Syntax, es.Expression));
                else if (statements.Any() && statements.Last().Type != BoundNodeType.ReturnStatement)
                {
                    var nullValue = new BoundLiteralExpression(compilationUnit!, "");
                    statements = statements.Add(new BoundReturnStatement(compilationUnit!, nullValue));
                }

                var body = Lowerer.Lower(globalScope.ScriptFunction, new BoundBlockStatement(compilationUnit!, statements));
                functionBodies.Add(globalScope.ScriptFunction, body);
            }

            return new BoundProgram(previous, diagnostics.ToImmutable(), globalScope.MainFunction, globalScope.ScriptFunction, functionBodies.ToImmutable());
        }

        private static BoundScope CreateParentScope(BoundGlobalScope? previous)
        {
            var stack = new Stack<BoundGlobalScope>();

            while (previous != null)
            {
                stack.Push(previous);
                previous = previous.Previous;
            }

            var parent = CreateRootScope();

            while (stack.Count > 0)
            {
                previous = stack.Pop();
                var scope = new BoundScope(parent);

                foreach (var t in previous.TemplateTypes)
                    scope.TryDeclareTemplate(t);

                foreach (var f in previous.Functions)
                    scope.TryDeclareFunction(f);

                foreach (var v in previous.Variables)
                    scope.TryDeclareVariable(v);

                parent = scope;
            }

            return parent;
        }
        private static BoundScope CreateRootScope()
        {
            var result = new BoundScope(null);

            foreach (var a in AnnotationSymbol.GetAll())
                result.TryDeclareAnnotation(a);

            foreach (var f in BuiltinFunctions.GetAll())
                result.TryDeclareFunction(f);

            return result;
        }

        private BoundNamespace BindNamespaceDeclaration(NamespaceDeclarationSyntax syntax) => BoundNamespace.Rebuild(syntax.Identifier, syntax.Members);
        private IAnnotatable BindAnnotationDeclaration(AnnotationDeclarationSyntax syntax, List<BoundAnnotation>? annotations = null)
        {
            var arguments = ImmutableArray.CreateBuilder<object>();

            if (syntax.Call != null)
            {
                var boundArguments = ImmutableArray.CreateBuilder<BoundExpression>();
                var boundTypes = ImmutableArray.CreateBuilder<TypeSymbol>();

                foreach (var argument in syntax.Call.Arguments)
                {
                    var boundArgument = BindExpression(argument);

                    if (!ClampAnnotationType(boundArgument.ValueType))
                        diagnostics.ReportInvalidAnnotationType(syntax.Identifier.Location);

                    boundArguments.Add(boundArgument);
                    boundTypes.Add(boundArgument.ValueType);
                }

                var unique = TypeUtils.BuildUniqueName(syntax.Identifier.Text, boundTypes);
                var symbol = scope.TryLookupSymbol(unique);

                if (symbol == null)
                {
                    diagnostics.ReportUndefinedAnnotation(syntax.Identifier.Location, syntax.Identifier.Text);
                    return null;
                }

                var annotation = symbol as AnnotationSymbol;

                if (annotation == null)
                {
                    diagnostics.ReportNotAnAnnotation(syntax.Identifier.Location, syntax.Identifier.Text);
                    return null;
                }

                if (syntax.Call.Arguments.Count != annotation.Parameters.Length)
                {
                    TextSpan span;

                    if (syntax.Call.Arguments.Count > annotation.Parameters.Length)
                    {
                        SyntaxNode firstExceedingNode;

                        if (annotation.Parameters.Length > 0)
                            firstExceedingNode = syntax.Call.Arguments.GetSeparator(annotation.Parameters.Length - 1);
                        else
                            firstExceedingNode = syntax.Call.Arguments[0];

                        var lastExceedingArgument = syntax.Call.Arguments[syntax.Call.Arguments.Count - 1];
                        span = TextSpan.FromBounds(firstExceedingNode.Span.Start, lastExceedingArgument.Span.End);
                    }
                    else
                        span = syntax.Call.LeftParenthesisToken.Span;

                    var location = new TextLocation(syntax.SyntaxTree.Text, span);
                    diagnostics.ReportWrongArgumentCount(location, annotation.Name, annotation.Parameters.Length, syntax.Call.Arguments.Count);
                }

                foreach (var argument in boundArguments)
                    arguments.Add(argument.ConstantValue.Value);
            }

            var boundAnnotation = new BoundAnnotation(syntax.Identifier.Text, arguments.ToImmutable());
            var member = BindMemberDeclaration(syntax.AnnotatedMember);

            member.Annotate(boundAnnotation);

            return member;
        }
        private FunctionSymbol BindFunctionDeclaration(FunctionDeclarationSyntax syntax)
        {
            var parameters = ImmutableArray.CreateBuilder<ParameterSymbol>();

            var seenParameterNames = new HashSet<string>();

            foreach (var parameterSyntax in syntax.Parameters)
            {
                var parameterName = parameterSyntax.Identifier.Text;
                var parameterType = BindTypeClause(parameterSyntax.TypeDeclaration);
                if (!seenParameterNames.Add(parameterName))
                {
                    diagnostics.ReportParameterAlreadyDeclared(parameterSyntax.Location, parameterName);
                }
                else
                {
                    var parameter = new ParameterSymbol(parameterName, parameterType, parameters.Count);
                    parameters.Add(parameter);
                }
            }

            var type = BindTypeClause(syntax.TypeDeclaration) ?? TypeSymbol.Void;

            var function = new FunctionSymbol(syntax.Identifier.Text, parameters.ToImmutable(), type, syntax);

            if (syntax.Identifier.Text != null &&
                !scope.TryDeclareFunction(function))
            {
                diagnostics.ReportSymbolAlreadyDeclared(syntax.Identifier.Location, function.Name);
            }

            return function;
        }
        private TemplateSymbol BindTemplateDeclaration(TemplateDeclarationSyntax syntax)
        {
            var seenParameterNames = new HashSet<string>();

            var template = new TemplateSymbol(syntax.Identifier.Text, syntax.GenericsDeclaration);

            foreach (var parameterSyntax in syntax.Declarations)
            {
                var parameterName = parameterSyntax.Identifier.Text;
                var parameterType = BindTypeClause(parameterSyntax.TypeDeclaration, syntax.GenericsDeclaration, template);

                if (!seenParameterNames.Add(parameterName))
                    diagnostics.ReportParameterAlreadyDeclared(parameterSyntax.Location, parameterName);
                else
                {
                    var parameter = new LocalVariableSymbol(parameterName, parameterSyntax.ReadOnlyToken != null, parameterType, null);
                    template.TryDeclareVariable(parameter);
                }
            }

            if (syntax.Identifier.Text != null && !scope.TryDeclareTemplate(template))
            {
                diagnostics.ReportSymbolAlreadyDeclared(syntax.Identifier.Location, template.Name);
            }

            return template;
        }
        private void BindImportDeclaration(ImportDeclarationSyntax syntax)
        {
            var candidate = BoundNamespace.Query(syntax.Identifier);

            if (candidate != null)
            {
                scope.AddNamespace(candidate);

                foreach (var template in candidate.Templates)
                    BindTemplateDeclaration(template);

                foreach (var function in candidate.Functions)
                    BindFunctionDeclaration(function);
            }

            // Report an error if no namespace candidate was found
        }

        [return: NotNullIfNotNull("member")]
        private IAnnotatable BindMemberDeclaration(MemberSyntax? member)
        {
            if (member is NamespaceDeclarationSyntax namespaceSyntax)
                return BindNamespaceDeclaration(namespaceSyntax);
            else if (member is AnnotationDeclarationSyntax annotationSyntax)
                return BindAnnotationDeclaration(annotationSyntax);
            else if (member is FunctionDeclarationSyntax functionSyntax)
                return BindFunctionDeclaration(functionSyntax);
            else if (member is TemplateDeclarationSyntax templateSyntax)
                return BindTemplateDeclaration(templateSyntax);
            else
                return null;
        }

        private BoundStatement BindVariableDeclaration(VariableDeclarationSyntax syntax)
        {
            var isReadOnly = syntax.Keyword.Type == SyntaxType.LetKeyword;
            var type = BindTypeClause(syntax.TypeDeclaration);
            var initializer = BindExpression(syntax.Initializer);
            var variableType = type ?? initializer.ValueType;
            var variable = BindVariableDeclaration(syntax.Identifier, isReadOnly, variableType, initializer.ConstantValue);
            var convertedInitializer = BindConversion(syntax.Initializer.Location, initializer, variableType);

            return new BoundVariableDeclaration(syntax, variable, convertedInitializer);
        }

        private BoundStatement BindStatement(StatementSyntax syntax, bool isGlobal = false)
        {
            var result = BindStatementInternal(syntax);

            if (!isScript || !isGlobal)
            {
                if (result is BoundExpressionStatement es)
                {
                    var isAllowedExpression = es.Expression.Type == BoundNodeType.ErrorExpression ||
                                              es.Expression.Type == BoundNodeType.AssignmentExpression ||
                                              es.Expression.Type == BoundNodeType.CallExpression ||
                                              es.Expression.Type == BoundNodeType.CompoundAssignmentExpression;
                    if (!isAllowedExpression)
                        diagnostics.ReportInvalidExpressionStatement(syntax.Location);
                }
            }

            return result;
        }
        private BoundStatement BindStatementInternal(StatementSyntax syntax)
        {
            switch (syntax.Type)
            {
                case SyntaxType.BlockStatement:
                    return BindBlockStatement((BlockStatementSyntax)syntax);
                case SyntaxType.VariableDeclarationStatement:
                    return BindVariableDeclaration((VariableDeclarationSyntax)syntax);
                case SyntaxType.IfStatement:
                    return BindIfStatement((IfStatementSyntax)syntax);
                case SyntaxType.WhileStatement:
                    return BindWhileStatement((WhileStatementSyntax)syntax);
                case SyntaxType.DoWhileStatement:
                    return BindDoWhileStatement((DoWhileStatementSyntax)syntax);
                case SyntaxType.ForStatement:
                    return BindForStatement((ForStatementSyntax)syntax);
                case SyntaxType.BreakStatement:
                    return BindBreakStatement((BreakStatementSyntax)syntax);
                case SyntaxType.ContinueStatement:
                    return BindContinueStatement((ContinueStatementSyntax)syntax);
                case SyntaxType.ReturnStatement:
                    return BindReturnStatement((ReturnStatementSyntax)syntax);
                case SyntaxType.ExpressionStatement:
                    return BindExpressionStatement((ExpressionStatementSyntax)syntax);
                default:
                    throw new Exception($"Unexpected syntax {syntax.Type}");
            }
        }

        private BoundExpression BindExpression(ExpressionSyntax syntax, TypeSymbol targetType) => BindConversion(syntax, targetType);
        private BoundExpression BindExpression(ExpressionSyntax syntax, bool canBeVoid = false)
        {
            var result = BindExpressionInternal(syntax);

            if (!canBeVoid && result.ValueType == TypeSymbol.Void)
            {
                diagnostics.ReportExpressionMustHaveValue(syntax.Location);

                return new BoundErrorExpression(syntax);
            }

            return result;
        }
        private BoundExpression BindExpressionInternal(ExpressionSyntax syntax)
        {
            switch (syntax.Type)
            {
                case SyntaxType.ParenthesizedExpression:
                    return BindParenthesizedExpression((ParenthesizedExpressionSyntax)syntax);
                case SyntaxType.LiteralExpression:
                    return BindLiteralExpression((LiteralExpressionSyntax)syntax);
                case SyntaxType.NewExpression:
                    return BindNewExpression((NewExpressionSyntax)syntax);
                case SyntaxType.NameExpression:
                    return BindNameExpression((NameExpressionSyntax)syntax);
                case SyntaxType.DotExpression:
                    return BindDotExpression((DotExpressionSyntax)syntax);
                case SyntaxType.TypeofExpression:
                    return BindTypeofExpression((TypeofExpressionSyntax)syntax);
                case SyntaxType.AssignmentExpression:
                    return BindAssignmentExpression((AssignmentExpressionSyntax)syntax);
                case SyntaxType.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionSyntax)syntax);
                case SyntaxType.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionSyntax)syntax);
                case SyntaxType.CallExpression:
                    return BindCallExpression((CallExpressionSyntax)syntax);
                default:
                    throw new Exception($"Unexpected syntax {syntax.Type}");
            }
        }

        private BoundExpression BindConversion(ExpressionSyntax syntax, TypeSymbol type, bool allowExplicit = false)
        {
            var expression = BindExpression(syntax);

            return BindConversion(syntax.Location, expression, type, allowExplicit);
        }
        private BoundExpression BindConversion(TextLocation diagnosticLocation, BoundExpression expression, TypeSymbol type, bool allowExplicit = false)
        {
            var conversion = Conversion.Classify(expression.ValueType, type);

            if (!conversion.Exists)
            {
                if (expression.ValueType != TypeSymbol.Error && type != TypeSymbol.Error)
                    diagnostics.ReportCannotConvert(diagnosticLocation, expression.ValueType, type);

                return new BoundErrorExpression(expression.Syntax);
            }

            if (!allowExplicit && conversion.IsExplicit)
            {
                diagnostics.ReportCannotConvertImplicitly(diagnosticLocation, expression.ValueType, type);
            }

            if (conversion.IsIdentity)
                return expression;

            return new BoundConversionExpression(expression.Syntax, type, expression);
        }

        [return: NotNullIfNotNull("syntax")]
        private TypeSymbol? BindTypeClause(TypeDeclarationSyntax? syntax, GenericsDeclarationSyntax? generics = null, Symbol? owner = null)
        {
            if (syntax == null)
                return null;

            var name = syntax.Identifier.Text;

            if (generics != null && owner != null)
            {
                if (generics.ContainsParameter(name))
                {
                    return new GenericsSymbol(owner, name, "undefined", typeof(object), new Dictionary<string, Symbol>());
                }
            }

            var type = LookupType(name, syntax.IsArray);

            if (type == null)
                diagnostics.ReportUndefinedType(syntax.Identifier.Location, syntax.Identifier.Text);

            return type;
        }
        private VariableSymbol BindVariableDeclaration(SyntaxToken identifier, bool isReadOnly, TypeSymbol type, BoundConstant? constant = null)
        {
            var name = identifier.Text ?? "?";
            var declare = !identifier.IsMissing;
            var variable = function == null
                                ? (VariableSymbol)new GlobalVariableSymbol(name, isReadOnly, type, constant)
                                : new LocalVariableSymbol(name, isReadOnly, type, constant);

            if (declare && !scope.TryDeclareVariable(variable))
                diagnostics.ReportSymbolAlreadyDeclared(identifier.Location, name);

            return variable;
        }
        private VariableSymbol? BindVariableReference(SyntaxToken identifierToken)
        {
            var name = identifierToken.Text;

            switch (scope.TryLookupSymbol(name))
            {
                case VariableSymbol variable:
                    variable.References.AddReference(identifierToken, ReferenceType.Variable);
                    return variable;
                case null:
                    diagnostics.ReportUndefinedVariable(identifierToken.Location, name);
                    return null;
                default:
                    diagnostics.ReportNotAVariable(identifierToken.Location, name);
                    return null;
            }
        }
        private BoundVariableChain BindDotReference(DotExpressionSyntax syntax)
        {
            var bound = BindVariableReference(syntax.LeftToken);

            if (syntax.RightExpression != null)
                return new BoundVariableChain(bound, BindReference(bound.ValueType, syntax.RightExpression as IdentifiableExpressionSyntax), null);

            return new BoundVariableChain(bound, null, null);
        }
        private BoundVariableChain? BindReference(TypeSymbol symbol, IdentifiableExpressionSyntax child)
        {
            if (symbol != null)
            {
                var text = child.IdentifierToken.Text;

                switch (symbol.TryLookupSymbol(text))
                {
                    case VariableSymbol variable:
                        variable.References.AddReference(child.IdentifierToken, ReferenceType.Variable);

                        if (child.Child != null)
                            return new BoundVariableChain(variable, BindReference(variable.ValueType, child.Child), null);
                        else
                            return new BoundVariableChain(variable, null, null);
                    case FunctionSymbol function:
                        //function.References.AddReference(child.IdentifierToken);

                        if (child is CallExpressionSyntax ces)
                        {
                            var bound = BindCallExpression(ces, function);

                            if (child.Child != null)
                                return new BoundVariableChain(function, BindReference(function.ValueType, child.Child), bound);
                            else
                                return new BoundVariableChain(function, null, bound);
                        } 
                        else
                        {
                            diagnostics.ReportNotAVariable(child.Location, text);
                            return null;
                        }
                    case null:
                        diagnostics.ReportUndefinedVariable(child.Location, text);
                        return null;
                    default:
                        diagnostics.ReportNotAVariable(child.Location, text);
                        return null;
                }
            }

            return null;
        }

        private TypeSymbol? LookupType(string name, bool isArray)
        {
            TypeSymbol? symbol;

            switch (name)
            {
                case "any":
                    symbol = TypeSymbol.Any;
                    break;
                case "bool":
                    symbol = TypeSymbol.Bool;
                    break;
                case "int":
                    symbol = TypeSymbol.Int;
                    break;
                case "string":
                    symbol = TypeSymbol.String;
                    break;
                case "char":
                    symbol = TypeSymbol.Char;
                    break;
                case "Type":
                    symbol = TypeSymbol.InternalType;
                    break;
                default:
                    symbol = LookupInstancedType(name);
                    break;
            }

            if (isArray)
                return symbol.Array;

            return symbol;
        }
        private TypeSymbol? LookupInstancedType(string name)
        {
            var symbol = scope.TryLookupSymbol(name);

            if (symbol != null && symbol is TemplateSymbol template)
                return template;
            else
                return null;
        }
        private bool ClampAnnotationType(TypeSymbol? type) => ClampType(type, TypeSymbol.String, TypeSymbol.Char, TypeSymbol.Int, TypeSymbol.Bool, TypeSymbol.InternalType);
        private bool ClampType(TypeSymbol? type, params TypeSymbol[] expectedTypes)
        {
            if (type == null || !expectedTypes.Any())
                return false;

            foreach (var expected in expectedTypes)
                if (type == expected)
                    return true;

            return false;
        }
    }
}
