using Onyx.Binding.Symbols;
using Onyx.Text;
using System.Collections.Immutable;
using System.Text;

namespace Onyx.Syntax
{
    internal sealed class Lexer
    {
        public DiagnosticsContainer Diagnostics => diagnostics;

        private readonly DiagnosticsContainer diagnostics = new DiagnosticsContainer();
        private readonly SyntaxTree syntaxTree;
        private readonly SourceText text;

        private int position;
        private int start;
        private SyntaxType type;
        private object? value;
        private ImmutableArray<SyntaxTrivia>.Builder triviaBuilder = ImmutableArray.CreateBuilder<SyntaxTrivia>();

        private char Current => Peek(0);
        private char Lookahead => Peek(1);

        public Lexer(SyntaxTree syntaxTree)
        {
            this.syntaxTree = syntaxTree;
            text = syntaxTree.Text;
        }

        private char Peek(int offset)
        {
            var index = position + offset;

            if (index >= text.Length)
                return '\0';

            return text[index];
        }

        public SyntaxToken Lex()
        {
            ReadTrivia(leading: true);

            var leadingTrivia = triviaBuilder.ToImmutable();
            var tokenStart = position;

            ReadToken();

            var tokenKind = type;
            var tokenValue = value;
            var tokenLength = position - start;

            ReadTrivia(leading: false);

            var trailingTrivia = triviaBuilder.ToImmutable();

            var tokenText = SyntaxFacts.GetText(tokenKind);
            if (tokenText == null)
                tokenText = text.ToString(tokenStart, tokenLength);

            return new SyntaxToken(syntaxTree, tokenKind, tokenStart, tokenText, tokenValue, leadingTrivia, trailingTrivia);
        }

        private void ReadToken()
        {
            start = position;
            type = SyntaxType.BadToken;
            value = null;

            switch (Current)
            {
                case '\0':
                    type = SyntaxType.EoFToken;
                    break;
                case ';':
                    position++;
                    type = SyntaxType.SemiColonToken;
                    break;
                case '+':
                    position++;
                    if (Current != '=')
                        type = SyntaxType.PlusToken;
                    else
                        Eat(SyntaxType.PlusEqualsToken);
                    break;
                case '-':
                    position++;
                    if (Current != '=')
                        type = SyntaxType.MinusToken;
                    else
                        Eat(SyntaxType.MinusEqualsToken);
                    break;
                case '*':
                    position++;
                    if (Current != '=')
                        type = SyntaxType.StarToken;
                    else
                        Eat(SyntaxType.StarEqualsToken);
                    break;
                case '/':
                    position++;
                    if (Current != '=')
                        type = SyntaxType.SlashToken;
                    else
                        Eat(SyntaxType.SlashEqualsToken);
                    break;
                case '(':
                    type = SyntaxType.LeftParenthesisToken;
                    position++;
                    break;
                case ')':
                    type = SyntaxType.RightParenthesisToken;
                    position++;
                    break;
                case '{':
                    type = SyntaxType.LeftBraceToken;
                    position++;
                    break;
                case '}':
                    type = SyntaxType.RightBraceToken;
                    position++;
                    break;
                case '[':
                    type = SyntaxType.LeftBracketToken;
                    position++;
                    break;
                case ']':
                    type = SyntaxType.RightBracketToken;
                    position++;
                    break;
                case ':':
                    type = SyntaxType.ColonToken;
                    position++;
                    break;
                case ',':
                    type = SyntaxType.CommaToken;
                    position++;
                    break;
                case '~':
                    type = SyntaxType.TildeToken;
                    position++;
                    break;
                case '@':
                    type = SyntaxType.AtToken;
                    position++;
                    break;
                case '^':
                    position++;
                    if (Current != '=')
                        type = SyntaxType.HatToken;
                    else
                        Eat(SyntaxType.HatEqualsToken);
                    break;
                case '&':
                    position++;
                    if (Current == '&')
                        Eat(SyntaxType.AmpersandAmpersandToken);
                    else if (Current == '=')
                        Eat(SyntaxType.AmpersandEqualsToken);
                    else
                        type = SyntaxType.AmpersandToken;
                    break;
                case '|':
                    position++;
                    if (Current == '|')
                        Eat(SyntaxType.PipePipeToken);
                    else if (Current == '=')
                        Eat(SyntaxType.PipeEqualsToken);
                    else
                        type = SyntaxType.PipeToken;
                    break;
                case '=':
                    position++;
                    if (Current == '=')
                        Eat(SyntaxType.EqualsEqualsToken);
                    else if (Current == '>')
                        Eat(SyntaxType.EqualsGreaterToken);
                    else
                        type = SyntaxType.EqualsToken;
                    break;
                case '!':
                    position++;
                    if (Current != '=')
                        type = SyntaxType.BangToken;
                    else
                        Eat(SyntaxType.BangEqualsToken);
                    break;
                case '<':
                    position++;
                    if (Current != '=')
                        type = SyntaxType.LessToken;
                    else
                        Eat(SyntaxType.LessOrEqualsToken);
                    break;
                case '>':
                    position++;
                    if (Current != '=')
                        type = SyntaxType.GreaterToken;
                    else
                        Eat(SyntaxType.GreaterOrEqualsToken);
                    break;
                case '.':
                    position++;
                    type = SyntaxType.DotToken;
                    break;
                case '"':
                    ReadString();
                    break;
                case '\'':
                    ReadChar();
                    break;
                case '0': case '1': case '2': case '3': case '4':
                case '5': case '6': case '7': case '8': case '9':
                    ReadNumber();
                    break;
                case '_':
                    ReadIdentifierOrKeyword();
                    break;
                default:
                    if (char.IsLetter(Current))
                        ReadIdentifierOrKeyword();
                    else
                    {
                        var span = new TextSpan(position, 1);
                        var location = new TextLocation(text, span);
                        diagnostics.ReportBadCharacter(location, Current);
                        position++;
                    }
                    break;
            }
        }

        private void ReadString()
        {
            // Skip the current quote
            position++;

            var sb = new StringBuilder();
            var done = false;

            while (!done)
            {
                switch (Current)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        var span = new TextSpan(start, 1);
                        var location = new TextLocation(text, span);
                        diagnostics.ReportUnterminatedString(location);
                        done = true;
                        break;
                    case '"':
                        if (Lookahead == '"')
                        {
                            sb.Append(Current);
                            position += 2;
                        }
                        else
                        {
                            position++;
                            done = true;
                        }
                        break;
                    default:
                        sb.Append(Current);
                        position++;
                        break;
                }
            }

            type = SyntaxType.StringToken;
            value = sb.ToString();
        }
        private void ReadChar()
        {
            // Skip the current quote
            position++;

            var chars = 0;
            var c = '\0';
            var done = false;

            while (!done)
            {
                switch (Current)
                {
                    case '\'':
                        if (Lookahead == '\'')
                        {
                            c = Current;
                            position += 2;
                            chars++;
                        }
                        else
                        {
                            position++;
                            done = true;
                        }
                        break;
                    default:
                        c = Current;
                        position++;
                        chars++;
                        break;
                }
            }

            if (chars > 1)
                diagnostics.ReportInvalidCharLength();

            type = SyntaxType.CharToken;
            value = c;
        }
        private void ReadNumber()
        {
            while (char.IsDigit(Current))
                position++;

            var length = position - start;
            var text = this.text.ToString(start, length);
            if (!int.TryParse(text, out var value))
            {
                var span = new TextSpan(start, length);
                var location = new TextLocation(this.text, span);
                diagnostics.ReportInvalidNumber(location, text, TypeSymbol.Int);
            }

            this.value = value;
            type = SyntaxType.NumberToken;
        }
        private void ReadIdentifierOrKeyword()
        {
            while (char.IsLetterOrDigit(Current) || Current == '_')
                position++;

            var length = position - start;
            var text = this.text.ToString(start, length);
            type = SyntaxFacts.GetKeywordType(text);
        }

        private void ReadTrivia(bool leading)
        {
            triviaBuilder.Clear();

            var done = false;

            while (!done)
            {
                start = position;
                type = SyntaxType.BadToken;
                value = null;

                switch (Current)
                {
                    case '\0':
                        done = true;
                        break;
                    case '/':
                        if (Lookahead == '/')
                        {
                            ReadSingleLineComment();
                        }
                        else if (Lookahead == '*')
                        {
                            ReadMultiLineComment();
                        }
                        else
                        {
                            done = true;
                        }
                        break;
                    case '\n':
                    case '\r':
                        if (!leading)
                            done = true;
                        ReadLineBreak();
                        break;
                    case ' ':
                    case '\t':
                        ReadWhiteSpace();
                        break;
                    default:
                        if (char.IsWhiteSpace(Current))
                            ReadWhiteSpace();
                        else
                            done = true;
                        break;
                }

                var length = position - start;
                if (length > 0)
                {
                    var text = this.text.ToString(start, length);
                    var trivia = new SyntaxTrivia(syntaxTree, type, start, text);
                    triviaBuilder.Add(trivia);
                }
            }
        }
        private void ReadLineBreak()
        {
            if (Current == '\r' && Lookahead == '\n')
            {
                position += 2;
            }
            else
            {
                position++;
            }

            type = SyntaxType.LineBreakTrivia;
        }
        private void ReadWhiteSpace()
        {
            var done = false;

            while (!done)
            {
                switch (Current)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        done = true;
                        break;
                    default:
                        if (!char.IsWhiteSpace(Current))
                            done = true;
                        else
                            position++;
                        break;
                }
            }

            type = SyntaxType.WhitespaceTrivia;
        }
        private void ReadSingleLineComment()
        {
            position += 2;
            var done = false;

            while (!done)
            {
                switch (Current)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        done = true;
                        break;
                    default:
                        position++;
                        break;
                }
            }

            type = SyntaxType.SingleLineCommentTrivia;
        }
        private void ReadMultiLineComment()
        {
            position += 2;
            var done = false;

            while (!done)
            {
                switch (Current)
                {
                    case '\0':
                        var span = new TextSpan(start, 2);
                        var location = new TextLocation(text, span);
                        diagnostics.ReportUnterminatedMultiLineComment(location);
                        done = true;
                        break;
                    case '*':
                        if (Lookahead == '/')
                        {
                            position++;
                            done = true;
                        }
                        position++;
                        break;
                    default:
                        position++;
                        break;
                }
            }

            type = SyntaxType.MultiLineCommentTrivia;
        }

        private void Eat(SyntaxType type)
        {
            this.type = type;
            position++;
        }
    }
}
