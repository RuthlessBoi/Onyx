using Onyx.Syntax.Nodes;
using Onyx.Syntax.Nodes.Members;
using Onyx.Syntax.Nodes.Statements;
using Onyx.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Onyx.Syntax
{
    internal sealed partial class Parser
    {
        public DiagnosticsContainer Diagnostics => diagnostics;

        private readonly DiagnosticsContainer diagnostics = new DiagnosticsContainer();
        private readonly SyntaxTree syntaxTree;
        private readonly SourceText text;
        private readonly ImmutableArray<SyntaxToken> tokens;
        private int position;

        public Parser(SyntaxTree syntaxTree)
        {
            var tokens = new List<SyntaxToken>();
            var badTokens = new List<SyntaxToken>();

            var lexer = new Lexer(syntaxTree);
            SyntaxToken token;

            do
            {
                token = lexer.Lex();

                if (token.Type == SyntaxType.BadToken)
                    badTokens.Add(token);
                else
                {
                    if (badTokens.Count > 0)
                    {
                        var leadingTrivia = token.LeadingTrivia.ToBuilder();
                        var index = 0;

                        foreach (var badToken in badTokens)
                        {
                            foreach (var lt in badToken.LeadingTrivia)
                                leadingTrivia.Insert(index++, lt);

                            var trivia = new SyntaxTrivia(syntaxTree, SyntaxType.SkippedTextTrivia, badToken.Position, badToken.Text);

                            leadingTrivia.Insert(index++, trivia);

                            foreach (var tt in badToken.TrailingTrivia)
                                leadingTrivia.Insert(index++, tt);
                        }

                        badTokens.Clear();
                        token = new SyntaxToken(token.SyntaxTree, token.Type, token.Position, token.Text, token.Value, leadingTrivia.ToImmutable(), token.TrailingTrivia);
                    }

                    tokens.Add(token);
                }
            } while (token.Type != SyntaxType.EoFToken);

            this.syntaxTree = syntaxTree;
            text = syntaxTree.Text;
            this.tokens = tokens.ToImmutableArray();
            diagnostics.AddRange(lexer.Diagnostics);
        }

        private SyntaxToken Peek(int offset)
        {
            var index = position + offset;
            if (index >= tokens.Length)
                return tokens[tokens.Length - 1];

            return tokens[index];
        }
        private SyntaxToken Current => Peek(0);
        private SyntaxToken NextToken()
        {
            var current = Current;
            position++;
            return current;
        }
        private SyntaxToken MatchToken(SyntaxType type)
        {
            if (Current.Type == type)
                return NextToken();

            diagnostics.ReportUnexpectedToken(Current.Location, Current.Type, type);
            return new SyntaxToken(syntaxTree, type, Current.Position, null, null, ImmutableArray<SyntaxTrivia>.Empty, ImmutableArray<SyntaxTrivia>.Empty);
        }
        private SyntaxToken MatchAny(params SyntaxType[] types)
        {
            if (!types.Any())
                throw new Exception("MatchAny needs at least one SyntaxType");

            foreach (var type in types)
            {
                if (Current.Type == type)
                {
                    return NextToken();
                }
            }

            diagnostics.ReportUnexpectedToken(Current.Location, Current.Type, types);
            return new SyntaxToken(syntaxTree, types[0], Current.Position, null, null, ImmutableArray<SyntaxTrivia>.Empty, ImmutableArray<SyntaxTrivia>.Empty);
        }
        private SyntaxToken? MatchOptional(SyntaxType type)
        {
            if (Current.Type == type)
                return NextToken();

            return null;
        }
        private SyntaxToken? MatchAnyOptional(params SyntaxType[] types)
        {
            if (!types.Any())
                throw new Exception("MatchAny needs at least one SyntaxType");

            foreach (var type in types)
            {
                if (Current.Type == type)
                {
                    return NextToken();
                }
            }

            return null;
        }
        private SyntaxToken? MatchConditional(SyntaxType type, bool condition)
        {
            if (Current.Type == type && condition)
                return NextToken();

            return null;
        }
        private SyntaxToken EmptyToken(SyntaxType type) => new SyntaxToken(syntaxTree, type, Current.Position, null, null, ImmutableArray<SyntaxTrivia>.Empty, ImmutableArray<SyntaxTrivia>.Empty);

        public CompilationUnitSyntax ParseCompilationUnit()
        {
            var members = ParseMembers();
            var eofToken = MatchToken(SyntaxType.EoFToken);

            return new CompilationUnitSyntax(syntaxTree, members, eofToken);
        }

        private ImmutableArray<MemberSyntax> ParseMembers()
        {
            var members = ImmutableArray.CreateBuilder<MemberSyntax>();

            while (Current.Type != SyntaxType.EoFToken)
            {
                var startToken = Current;

                members.Add(ParseMember());

                // If ParseMember() did not consume any tokens,
                // we need to skip the current token and continue
                // in order to avoid an infinite loop.
                //
                // We don't need to report an error, because we'll
                // already tried to parse an expression statement
                // and reported one.
                if (Current == startToken)
                    NextToken();
            }

            return members.ToImmutable();
        }
        private ImmutableArray<MemberSyntax> ParseNamespaceMembers()
        {
            var members = ImmutableArray.CreateBuilder<MemberSyntax>();

            while (Current.Type != SyntaxType.EoFToken && Current.Type != SyntaxType.RightBraceToken )
            {
                var startToken = Current;

                members.Add(ParseNamespaceMember());

                // If ParseMember() did not consume any tokens,
                // we need to skip the current token and continue
                // in order to avoid an infinite loop.
                //
                // We don't need to report an error, because we'll
                // already tried to parse an expression statement
                // and reported one.
                if (Current == startToken)
                    NextToken();
            }

            return members.ToImmutable();
        }
        private MemberSyntax ParseMember()
        {
            if (Current.Type == SyntaxType.NamespaceKeyword)
                return ParseNamespaceDeclaration();
            else if (Current.Type == SyntaxType.FunctionKeyword)
                return ParseFunctionDeclaration();
            else if (Current.Type == SyntaxType.TemplateKeyword)
                return ParseTemplateDeclaration();
            else if (Current.Type == SyntaxType.ImportKeyword)
                return ParseImportDeclaration();
            else if (Current.Type == SyntaxType.AtToken)
                return ParseAnnotationDeclaration();

            return ParseGlobalStatement();
        }
        private MemberSyntax ParseNamespaceMember()
        {
            if (Current.Type == SyntaxType.FunctionKeyword)
                return ParseFunctionDeclaration();
            else if (Current.Type == SyntaxType.TemplateKeyword)
                return ParseTemplateDeclaration();
            else if (Current.Type == SyntaxType.AtToken)
                return ParseAnnotationDeclaration();

            return ParseGlobalStatement();
        }
        private MemberSyntax ParseNamespaceDeclaration()
        {
            var namespaceKeyword = MatchToken(SyntaxType.NamespaceKeyword);
            var identifier = ParseNamespace();
            var leftBraceToken = MatchToken(SyntaxType.LeftBraceToken);
            var members = ParseNamespaceMembers();
            var rightBraceToken = MatchToken(SyntaxType.RightBraceToken);

            return new NamespaceDeclarationSyntax(syntaxTree, namespaceKeyword, identifier, leftBraceToken, members, rightBraceToken);
        }
        private MemberSyntax ParseFunctionDeclaration()
        {
            var functionKeyword = MatchToken(SyntaxType.FunctionKeyword);
            var identifier = MatchToken(SyntaxType.IdentifierToken);
            var openParenthesisToken = MatchToken(SyntaxType.LeftParenthesisToken);
            var parameters = ParseParameterList();
            var closeParenthesisToken = MatchToken(SyntaxType.RightParenthesisToken);
            var type = ParseOptionalTypeClause();

            BlockStatementSyntax body;

            // Allow parsing of single-statement function declarations
            if (Current.Type == SyntaxType.EqualsGreaterToken)
            {
                MatchToken(SyntaxType.EqualsGreaterToken);
                body = ParseLambdaStatement(type == null);
            } 
            else
                body = ParseBlockStatement();

            return new FunctionDeclarationSyntax(syntaxTree, functionKeyword, identifier, openParenthesisToken, parameters, closeParenthesisToken, type, body);
        }
        private MemberSyntax ParseTemplateDeclaration()
        {
            var templateKeyword = MatchToken(SyntaxType.TemplateKeyword);
            var identifier = MatchToken(SyntaxType.IdentifierToken);
            var genericsDeclaration = ParseOptionalGenericsDeclaration();
            var openBraceToken = MatchToken(SyntaxType.LeftBraceToken);
            var modelDeclarations = ParseTemplateParameterList();
            var closeBraceToken = MatchToken(SyntaxType.RightBraceToken);

            return new TemplateDeclarationSyntax(syntaxTree, templateKeyword, identifier, genericsDeclaration, openBraceToken, modelDeclarations, closeBraceToken);
        }
        private MemberSyntax ParseImportDeclaration()
        {
            var importKeyword = MatchToken(SyntaxType.ImportKeyword);
            var identifier = ParseNamespace();
            //var identifier = MatchToken(SyntaxType.StringToken);

            // Eventually renaming of modules
            // Example: import <identifier> as <name>

            return new ImportDeclarationSyntax(syntaxTree, importKeyword, identifier);
        }
        private MemberSyntax ParseAnnotationDeclaration()
        {
            var atToken = MatchToken(SyntaxType.AtToken);
            var identifier = MatchToken(SyntaxType.IdentifierToken);
            var call = ParseOptionalCallSyntax();
            var annotatedMember = ParseMember();

            return new AnnotationDeclarationSyntax(syntaxTree, atToken, identifier, call, annotatedMember);
        }
        private SeparatedSyntaxList<ParameterSyntax> ParseParameterList()
        {
            var nodesAndSeparators = ImmutableArray.CreateBuilder<SyntaxNode>();

            var parseNextParameter = true;
            while (parseNextParameter && Current.Type != SyntaxType.RightParenthesisToken && Current.Type != SyntaxType.EoFToken)
            {
                var parameter = ParseParameter();
                nodesAndSeparators.Add(parameter);

                if (Current.Type == SyntaxType.CommaToken)
                {
                    var comma = MatchToken(SyntaxType.CommaToken);
                    nodesAndSeparators.Add(comma);
                }
                else
                    parseNextParameter = false;
            }

            return new SeparatedSyntaxList<ParameterSyntax>(nodesAndSeparators.ToImmutable());
        }
        private ParameterSyntax ParseParameter()
        {
            var identifier = MatchToken(SyntaxType.IdentifierToken);
            var type = ParseTypeClause();

            return new ParameterSyntax(syntaxTree, identifier, type);
        }
        private SeparatedSyntaxList<TemplateParameterSyntax> ParseTemplateParameterList()
        {
            var nodesAndSeparators = ImmutableArray.CreateBuilder<SyntaxNode>();

            var parseNextParameter = true;
            while (parseNextParameter && Current.Type != SyntaxType.RightBraceToken && Current.Type != SyntaxType.EoFToken)
            {
                var parameter = ParseTemplateParameter();
                nodesAndSeparators.Add(parameter);

                if (Current.Type == SyntaxType.CommaToken)
                {
                    var comma = MatchToken(SyntaxType.CommaToken);
                    nodesAndSeparators.Add(comma);
                }
                else
                    parseNextParameter = false;
            }

            return new SeparatedSyntaxList<TemplateParameterSyntax>(nodesAndSeparators.ToImmutable());
        }
        private TemplateParameterSyntax ParseTemplateParameter()
        {
            SyntaxToken? readOnlyToken = null;

            if (Peek(0).Type == SyntaxType.ReadOnlyKeyword)
                readOnlyToken = NextToken();

            var identifier = MatchToken(SyntaxType.IdentifierToken);
            var type = ParseTypeClause();

            return new TemplateParameterSyntax(syntaxTree, readOnlyToken, identifier, type);
        }
        private MemberSyntax ParseGlobalStatement()
        {
            var statement = ParseStatement();

            return new GlobalStatementSyntax(syntaxTree, statement);
        }
        private StatementSyntax ParseStatement()
        {
            switch (Current.Type)
            {
                case SyntaxType.LeftBraceToken:
                    return ParseBlockStatement();
                case SyntaxType.LetKeyword:
                case SyntaxType.VarKeyword:
                    return ParseVariableDeclaration();
                case SyntaxType.IfKeyword:
                    return ParseIfStatement();
                case SyntaxType.WhileKeyword:
                    return ParseWhileStatement();
                case SyntaxType.DoKeyword:
                    return ParseDoWhileStatement();
                case SyntaxType.ForKeyword:
                    return ParseForStatement();
                case SyntaxType.BreakKeyword:
                    return ParseBreakStatement();
                case SyntaxType.ContinueKeyword:
                    return ParseContinueStatement();
                case SyntaxType.ReturnKeyword:
                    return ParseReturnStatement();
                default:
                    return ParseExpressionStatement();
            }
        }
        private BlockStatementSyntax ParseBlockStatement()
        {
            var statements = ImmutableArray.CreateBuilder<StatementSyntax>();

            var openBraceToken = MatchToken(SyntaxType.LeftBraceToken);

            while (Current.Type != SyntaxType.EoFToken && Current.Type != SyntaxType.RightBraceToken)
            {
                var startToken = Current;

                var statement = ParseStatement();
                statements.Add(statement);

                // If ParseStatement() did not consume any tokens,
                // we need to skip the current token and continue
                // in order to avoid an infinite loop.
                //
                // We don't need to report an error, because we'll
                // already tried to parse an expression statement
                // and reported one.
                if (Current == startToken)
                    NextToken();
            }

            var closeBraceToken = MatchToken(SyntaxType.RightBraceToken);

            return new BlockStatementSyntax(syntaxTree, openBraceToken, statements.ToImmutable(), closeBraceToken);
        }
        private BlockStatementSyntax ParseLambdaStatement(bool isVoid)
        {
            var statements = ImmutableArray.CreateBuilder<StatementSyntax>();

            if (Current.Type != SyntaxType.EoFToken && !isVoid)
                statements.Add(new ReturnStatementSyntax(syntaxTree, EmptyToken(SyntaxType.ReturnKeyword), ParseExpression()));
            else
                statements.Add(ParseExpressionStatement());

            //MatchToken(SyntaxType.SemiColonToken);

            return new BlockStatementSyntax(syntaxTree, EmptyToken(SyntaxType.LeftBraceToken), statements.ToImmutable(), EmptyToken(SyntaxType.RightBraceToken));
        }
        private StatementSyntax ParseVariableDeclaration()
        {
            var expected = Current.Type == SyntaxType.LetKeyword ? SyntaxType.LetKeyword : SyntaxType.VarKeyword;
            var keyword = MatchToken(expected);
            var identifier = MatchToken(SyntaxType.IdentifierToken);
            var typeClause = ParseOptionalTypeClause();
            var equals = MatchToken(SyntaxType.EqualsToken);
            var initializer = ParseExpression();

            return new VariableDeclarationSyntax(syntaxTree, keyword, identifier, typeClause, equals, initializer);
        }
        private TypeDeclarationSyntax? ParseOptionalTypeClause()
        {
            if (Current.Type != SyntaxType.ColonToken)
                return null;

            return ParseTypeClause();
        }
        private TypeDeclarationSyntax ParseTypeClause()
        {
            var colonToken = MatchToken(SyntaxType.ColonToken);
            var identifier = MatchToken(SyntaxType.IdentifierToken);
            var leftBracketToken = MatchOptional(SyntaxType.LeftBracketToken);
            var rightBracketToken = MatchOptional(SyntaxType.RightBracketToken);

            return new TypeDeclarationSyntax(syntaxTree, colonToken, identifier, leftBracketToken, rightBracketToken);
        }
        private StatementSyntax ParseIfStatement()
        {
            var keyword = MatchToken(SyntaxType.IfKeyword);
            var condition = ParseExpression();
            var statement = ParseStatement();
            var elseClause = ParseOptionalElseClause();

            return new IfStatementSyntax(syntaxTree, keyword, condition, statement, elseClause);
        }
        private ElseDeclarationSyntax? ParseOptionalElseClause()
        {
            if (Current.Type != SyntaxType.ElseKeyword)
                return null;

            var keyword = NextToken();
            var statement = ParseStatement();

            return new ElseDeclarationSyntax(syntaxTree, keyword, statement);
        }
        private StatementSyntax ParseWhileStatement()
        {
            var keyword = MatchToken(SyntaxType.WhileKeyword);
            var condition = ParseExpression();
            var body = ParseStatement();

            return new WhileStatementSyntax(syntaxTree, keyword, condition, body);
        }
        private StatementSyntax ParseDoWhileStatement()
        {
            var doKeyword = MatchToken(SyntaxType.DoKeyword);
            var body = ParseStatement();
            var whileKeyword = MatchToken(SyntaxType.WhileKeyword);
            var condition = ParseExpression();

            return new DoWhileStatementSyntax(syntaxTree, doKeyword, body, whileKeyword, condition);
        }
        private StatementSyntax ParseForStatement()
        {
            var keyword = MatchToken(SyntaxType.ForKeyword);
            var identifier = MatchToken(SyntaxType.IdentifierToken);
            var equalsToken = MatchToken(SyntaxType.EqualsToken);
            var lowerBound = ParseExpression();
            var toKeyword = MatchToken(SyntaxType.ToKeyword);
            var upperBound = ParseExpression();
            var body = ParseStatement();

            return new ForStatementSyntax(syntaxTree, keyword, identifier, equalsToken, lowerBound, toKeyword, upperBound, body);
        }
        private StatementSyntax ParseBreakStatement()
        {
            var keyword = MatchToken(SyntaxType.BreakKeyword);

            return new BreakStatementSyntax(syntaxTree, keyword);
        }
        private StatementSyntax ParseContinueStatement()
        {
            var keyword = MatchToken(SyntaxType.ContinueKeyword);

            return new ContinueStatementSyntax(syntaxTree, keyword);
        }
        private StatementSyntax ParseReturnStatement()
        {
            var keyword = MatchToken(SyntaxType.ReturnKeyword);
            var keywordLine = text.GetLineIndex(keyword.Span.Start);
            var currentLine = text.GetLineIndex(Current.Span.Start);
            var isEof = Current.Type == SyntaxType.EoFToken;
            var sameLine = !isEof && keywordLine == currentLine;
            var expression = sameLine ? ParseExpression() : null;

            return new ReturnStatementSyntax(syntaxTree, keyword, expression);
        }
        private ExpressionStatementSyntax ParseExpressionStatement()
        {
            var expression = ParseExpression();

            return new ExpressionStatementSyntax(syntaxTree, expression);
        }
        private ExpressionSyntax ParseExpression() => ParseAssignmentExpression();
        private ExpressionSyntax ParseAssignmentExpression()
        {
            if (Peek(0).Type == SyntaxType.IdentifierToken)
            {
                switch (Peek(1).Type)
                {
                    case SyntaxType.PlusEqualsToken:
                    case SyntaxType.MinusEqualsToken:
                    case SyntaxType.StarEqualsToken:
                    case SyntaxType.SlashEqualsToken:
                    case SyntaxType.AmpersandEqualsToken:
                    case SyntaxType.PipeEqualsToken:
                    case SyntaxType.HatEqualsToken:
                    case SyntaxType.EqualsToken:
                        var identifier = NextToken();
                        var operatorToken = NextToken();
                        var right = ParseAssignmentExpression();
                        return new AssignmentExpressionSyntax(syntaxTree, identifier, operatorToken, right);
                }
            }

            return ParseBinaryExpression();
        }
        private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
        {
            ExpressionSyntax left;

            var unaryOperatorPrecedence = Current.Type.GetUnaryOperatorPrecedence();

            if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
            {
                var operatorToken = NextToken();
                var operand = ParseBinaryExpression(unaryOperatorPrecedence);
                left = new UnaryExpressionSyntax(syntaxTree, operatorToken, operand);
            }
            else
                left = ParsePrimaryExpression();

            while (true)
            {
                var precedence = Current.Type.GetBinaryOperatorPrecedence();

                if (precedence == 0 || precedence <= parentPrecedence)
                    break;

                var operatorToken = NextToken();
                var right = ParseBinaryExpression(precedence);

                left = new BinaryExpressionSyntax(syntaxTree, left, operatorToken, right);
            }

            return left;
        }
        private ExpressionSyntax ParsePrimaryExpression()
        {
            switch (Current.Type)
            {
                case SyntaxType.LeftParenthesisToken:
                    return ParseParenthesizedExpression();
                case SyntaxType.FalseKeyword:
                case SyntaxType.TrueKeyword:
                    return ParseBooleanLiteral();
                case SyntaxType.NumberToken:
                    return ParseNumberLiteral();
                case SyntaxType.StringToken:
                    return ParseStringLiteral();
                case SyntaxType.CharToken:
                    return ParseCharLiteral();
                case SyntaxType.NewKeyword:
                    return ParseNewExpression();
                case SyntaxType.TypeofKeyword:
                    return ParseTypeofExpression();
                case SyntaxType.IdentifierToken:
                default:
                    return ParseNameOrCallExpression();
            }
        }
        private ExpressionSyntax ParseParenthesizedExpression()
        {
            var left = MatchToken(SyntaxType.LeftParenthesisToken);
            var expression = ParseExpression();
            var right = MatchToken(SyntaxType.RightParenthesisToken);

            return new ParenthesizedExpressionSyntax(syntaxTree, left, expression, right);
        }
        private ExpressionSyntax ParseBooleanLiteral()
        {
            var isTrue = Current.Type == SyntaxType.TrueKeyword;
            var keywordToken = isTrue ? MatchToken(SyntaxType.TrueKeyword) : MatchToken(SyntaxType.FalseKeyword);

            return new LiteralExpressionSyntax(syntaxTree, keywordToken, isTrue);
        }
        private ExpressionSyntax ParseNumberLiteral()
        {
            var numberToken = MatchToken(SyntaxType.NumberToken);

            return new LiteralExpressionSyntax(syntaxTree, numberToken);
        }
        private ExpressionSyntax ParseNewExpression()
        {
            var newKeyword = MatchToken(SyntaxType.NewKeyword);
            var type = ParseType(true);
            var initializer = ParseOptionalInitializer(type.IsArray);

            return new NewExpressionSyntax(syntaxTree, newKeyword, type, initializer);
        }
        private ExpressionSyntax ParseStringLiteral()
        {
            var stringToken = MatchToken(SyntaxType.StringToken);

            return new LiteralExpressionSyntax(syntaxTree, stringToken);
        }
        private ExpressionSyntax ParseCharLiteral()
        {
            var charToken = MatchToken(SyntaxType.CharToken);

            return new LiteralExpressionSyntax(syntaxTree, charToken);
        }
        private InitializerSyntax? ParseOptionalInitializer(bool isArray)
        {
            if (Peek(0).Type == SyntaxType.LeftBraceToken)
            {
                if (isArray)
                    return ParseArrayInitializer();
                else
                    return ParseTemplateInitializer();
            }

            return null;
        }
        private TemplateInitializerSyntax ParseTemplateInitializer()
        {
            var leftBrace = MatchToken(SyntaxType.LeftBraceToken);
            var parameters = ParseTemplateParameters();
            var rightBrace = MatchToken(SyntaxType.RightBraceToken);

            return new TemplateInitializerSyntax(syntaxTree, leftBrace, parameters, rightBrace);
        }
        private SeparatedSyntaxList<TemplateParameterInitializerSyntax> ParseTemplateParameters()
        {
            var nodesAndSeparators = ImmutableArray.CreateBuilder<SyntaxNode>();

            var parseNextParameter = true;
            while (parseNextParameter && Current.Type != SyntaxType.RightBraceToken && Current.Type != SyntaxType.EoFToken)
            {
                var parameter = ParseTemplateParameterInitializer();
                nodesAndSeparators.Add(parameter);

                if (Current.Type == SyntaxType.CommaToken)
                {
                    var comma = MatchToken(SyntaxType.CommaToken);
                    nodesAndSeparators.Add(comma);
                }
                else
                    parseNextParameter = false;
            }

            return new SeparatedSyntaxList<TemplateParameterInitializerSyntax>(nodesAndSeparators.ToImmutable());
        }
        private TemplateParameterInitializerSyntax ParseTemplateParameterInitializer()
        {
            var identifier = MatchToken(SyntaxType.IdentifierToken);
            var equalsToken = MatchToken(SyntaxType.EqualsToken);
            var value = ParseExpression();

            return new TemplateParameterInitializerSyntax(syntaxTree, identifier, equalsToken, value);
        }
        private ArrayInitializerSyntax ParseArrayInitializer()
        {
            var leftBrace = MatchToken(SyntaxType.LeftBraceToken);
            var arguments = ParseArrayArguments();
            var rightBrace = MatchToken(SyntaxType.RightBraceToken);

            return new ArrayInitializerSyntax(syntaxTree, leftBrace, arguments, rightBrace);
        }
        private SeparatedSyntaxList<ExpressionSyntax> ParseArrayArguments()
        {
            var nodes = ImmutableArray.CreateBuilder<SyntaxNode>();

            var parseNextArgument = true;
            while (parseNextArgument && Current.Type != SyntaxType.RightBraceToken && Current.Type != SyntaxType.EoFToken)
            {
                nodes.Add(ParseExpression());

                if (Current.Type == SyntaxType.CommaToken)
                    nodes.Add(MatchToken(SyntaxType.CommaToken));
                else
                    parseNextArgument = false;
            }

            return new SeparatedSyntaxList<ExpressionSyntax>(nodes.ToImmutable());
        }
        private ExpressionSyntax ParseNameOrCallExpression()
        {
            if (Peek(0).Type == SyntaxType.IdentifierToken && Peek(1).Type == SyntaxType.LeftParenthesisToken)
                return ParseCallExpression();
            else if (Peek(0).Type == SyntaxType.IdentifierToken && Peek(1).Type == SyntaxType.DotToken)
                return ParseDotExpression();

            return ParseNameExpression();
        }
        private ExpressionSyntax ParseCallExpression()
        {
            var identifier = MatchToken(SyntaxType.IdentifierToken);
            var openParenthesisToken = MatchToken(SyntaxType.LeftParenthesisToken);
            var arguments = ParseArguments();
            var closeParenthesisToken = MatchToken(SyntaxType.RightParenthesisToken);

            IdentifiableExpressionSyntax? child = null;

            return new CallExpressionSyntax(syntaxTree, identifier, openParenthesisToken, arguments, closeParenthesisToken, child);
        }
        private SeparatedSyntaxList<ExpressionSyntax> ParseArguments()
        {
            var nodesAndSeparators = ImmutableArray.CreateBuilder<SyntaxNode>();

            var parseNextArgument = true;
            while (parseNextArgument &&  Current.Type != SyntaxType.RightParenthesisToken && Current.Type != SyntaxType.EoFToken)
            {
                var expression = ParseExpression();
                nodesAndSeparators.Add(expression);

                if (Current.Type == SyntaxType.CommaToken)
                {
                    var comma = MatchToken(SyntaxType.CommaToken);
                    nodesAndSeparators.Add(comma);
                }
                else
                {
                    parseNextArgument = false;
                }
            }

            return new SeparatedSyntaxList<ExpressionSyntax>(nodesAndSeparators.ToImmutable());
        }
        private ExpressionSyntax ParseTypeofExpression()
        {
            var keyword = MatchToken(SyntaxType.TypeofKeyword);
            var type = ParseType();

            return new TypeofExpressionSyntax(syntaxTree, keyword, type);
        }
        private ExpressionSyntax ParseNameExpression()
        {
            var identifierToken = MatchToken(SyntaxType.IdentifierToken);
            var modifier = ParseModifier();

            return new NameExpressionSyntax(syntaxTree, identifierToken, modifier);
        }
        private ExpressionSyntax ParseDotExpression()
        {
            var leftToken = MatchToken(SyntaxType.IdentifierToken);
            var dotToken = MatchToken(SyntaxType.DotToken);
            var rightExpression = ParseNameOrCallExpression();

            return new DotExpressionSyntax(syntaxTree, leftToken, dotToken, rightExpression);
        }
        private TypeSyntax ParseType(bool parseArraySize = false)
        {
            var identifier = MatchToken(SyntaxType.IdentifierToken);
            var arguments = ParseOptionalGenericsArguments();
            var leftBracketToken = MatchOptional(SyntaxType.LeftBracketToken);
            var sizeToken = MatchConditional(SyntaxType.NumberToken, parseArraySize);
            var rightBracketToken = MatchOptional(SyntaxType.RightBracketToken);

            return new TypeSyntax(syntaxTree, identifier, arguments, leftBracketToken, sizeToken, rightBracketToken);
        }
        private GenericsArgumentsSyntax? ParseOptionalGenericsArguments()
        {
            if (Current.Type == SyntaxType.LessToken)
            {
                var lessThanToken = MatchToken(SyntaxType.LessToken);
                var arguments = ParseGenericsArguments();
                var greaterThanToken = MatchToken(SyntaxType.GreaterToken);

                return new GenericsArgumentsSyntax(syntaxTree, lessThanToken, arguments, greaterThanToken);
            }

            return null;
        }
        private SeparatedSyntaxList<TypeSyntax> ParseGenericsArguments()
        {
            var nodes = ImmutableArray.CreateBuilder<SyntaxNode>();
            var parseNext = true;

            while (parseNext && Current.Type != SyntaxType.GreaterToken && Current.Type != SyntaxType.EoFToken)
            {
                nodes.Add(ParseType());

                if (Current.Type == SyntaxType.CommaToken)
                    nodes.Add(MatchToken(SyntaxType.CommaToken));
                else
                    parseNext = false;
            }

            return new SeparatedSyntaxList<TypeSyntax>(nodes.ToImmutable());
        }
        private ModifierSyntax? ParseModifier()
        {
            if (Current.Type == SyntaxType.LeftBracketToken)
                return ParseIndexerModifier();

            return null;
        }
        private ModifierSyntax ParseIndexerModifier()
        {
            var leftBracketToken = MatchToken(SyntaxType.LeftBracketToken);
            var indexToken = MatchToken(SyntaxType.NumberToken);
            var rightBracketToken = MatchToken(SyntaxType.RightBracketToken);

            return new IndexerModifierSyntax(syntaxTree, leftBracketToken, indexToken, rightBracketToken);
        }
        private GenericsDeclarationSyntax? ParseOptionalGenericsDeclaration()
        {
            if (Current.Type == SyntaxType.LessToken)
                return ParseGenericsDeclaration();

            return null;
        }
        private GenericsDeclarationSyntax ParseGenericsDeclaration()
        {
            var lessThanToken = MatchToken(SyntaxType.LessToken);
            var parameters = ParseGenericsParameters();
            var greaterThanToken = MatchToken(SyntaxType.GreaterToken);

            return new GenericsDeclarationSyntax(syntaxTree, lessThanToken, parameters, greaterThanToken);
        }
        private SeparatedSyntaxList<GenericsParameterSyntax> ParseGenericsParameters()
        {
            var nodesAndSeparators = ImmutableArray.CreateBuilder<SyntaxNode>();
            var parseNextArgument = true;

            while (parseNextArgument && Current.Type != SyntaxType.GreaterToken && Current.Type != SyntaxType.EoFToken)
            {
                nodesAndSeparators.Add(ParseGenericsParamter());

                if (Current.Type == SyntaxType.CommaToken)
                    nodesAndSeparators.Add(MatchToken(SyntaxType.CommaToken));
                else
                    parseNextArgument = false;
            }

            return new SeparatedSyntaxList<GenericsParameterSyntax>(nodesAndSeparators.ToImmutable());
        }
        private GenericsParameterSyntax ParseGenericsParamter()
        {
            var identifier = MatchToken(SyntaxType.IdentifierToken);
            var colonToken = MatchOptional(SyntaxType.ColonToken);
            var parentIdentifier = MatchAnyOptional(SyntaxType.IdentifierToken, SyntaxType.TemplateKeyword);

            return new GenericsParameterSyntax(syntaxTree, identifier, colonToken, parentIdentifier);
        }
        private NamespaceSyntax ParseNamespace()
        {
            var nodes = ImmutableArray.CreateBuilder<SyntaxNode>();
            var parseNext = true;

            while (parseNext && Current.Type != SyntaxType.EoFToken)
            {
                nodes.Add(MatchToken(SyntaxType.IdentifierToken));

                if (Current.Type == SyntaxType.DotToken)
                    nodes.Add(MatchToken(SyntaxType.DotToken));
                else
                    parseNext = false;
            }

            return new NamespaceSyntax(syntaxTree, new SeparatedSyntaxList<SyntaxToken>(nodes.ToImmutable()));
        }
        private CallSyntax? ParseOptionalCallSyntax()
        {
            if (Current.Type != SyntaxType.LeftParenthesisToken)
                return null;

            var leftParenthesisToken = MatchToken(SyntaxType.LeftParenthesisToken);
            var arguments = ParseArguments();
            var rightParenthesisToken = MatchToken(SyntaxType.RightParenthesisToken);

            return new CallSyntax(syntaxTree, leftParenthesisToken, arguments, rightParenthesisToken);
        }
    }

    internal sealed partial class Parser
    {
        /*private MemberSyntax ParseClassDeclaration()
        {
            var classKeyword = MatchToken(SyntaxType.ClassKeyword);
            var identifier = MatchToken(SyntaxType.IdentifierToken);
            var body = ParseClassBody();

            return new ClassDeclarationSyntax(syntaxTree, classKeyword, identifier, body);
        }
        private ClassBodySyntax ParseClassBody()
        {
            var leftBraceToken = MatchToken(SyntaxType.LeftBraceToken);
            var members = ParseClassMembers();
            var rightBraceToken = MatchToken(SyntaxType.RightBraceToken);

            return new ClassBodySyntax(syntaxTree, leftBraceToken, members, rightBraceToken);
        }
        private ImmutableArray<MemberSyntax> ParseClassMembers()
        {
            var members = ImmutableArray.CreateBuilder<MemberSyntax>();

            while (Current.Type != SyntaxType.EoFToken)
            {
                var startToken = Current;

                members.Add(ParseClassMember());

                // If ParseMember() did not consume any tokens,
                // we need to skip the current token and continue
                // in order to avoid an infinite loop.
                //
                // We don't need to report an error, because we'll
                // already tried to parse an expression statement
                // and reported one.
                if (Current == startToken)
                    NextToken();
            }

            return members.ToImmutable();
        }
        private MemberSyntax ParseClassMember()
        {
            if (Current.Type == SyntaxType.FunctionKeyword)
                return ParseFunctionDeclaration();
            else if (Current.Type == SyntaxType.AtToken)
                return ParseAnnotationDeclaration();

            return ParseGlobalStatement();
        }*/
    }
}
