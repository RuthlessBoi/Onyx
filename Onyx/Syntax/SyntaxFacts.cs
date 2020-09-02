using System;
using System.Collections.Generic;

namespace Onyx.Syntax
{
    public static class SyntaxFacts
    {
        public static SyntaxType GetKeywordType(string text)
        {
            switch (text)
            {
                case "break":
                    return SyntaxType.BreakKeyword;
                case "continue":
                    return SyntaxType.ContinueKeyword;
                case "else":
                    return SyntaxType.ElseKeyword;
                case "false":
                    return SyntaxType.FalseKeyword;
                case "for":
                    return SyntaxType.ForKeyword;
                case "function":
                    return SyntaxType.FunctionKeyword;
                case "template":
                    return SyntaxType.TemplateKeyword;
                case "class":
                    return SyntaxType.ClassKeyword;
                case "abstract":
                    return SyntaxType.AbstractKeyword;
                case "constructor":
                    return SyntaxType.ConstructorKeyword;
                case "import":
                    return SyntaxType.ImportKeyword;
                case "is":
                    return SyntaxType.IsKeyword;
                case "if":
                    return SyntaxType.IfKeyword;
                case "let":
                    return SyntaxType.LetKeyword;
                case "return":
                    return SyntaxType.ReturnKeyword;
                case "to":
                    return SyntaxType.ToKeyword;
                case "true":
                    return SyntaxType.TrueKeyword;
                case "var":
                    return SyntaxType.VarKeyword;
                case "while":
                    return SyntaxType.WhileKeyword;
                case "do":
                    return SyntaxType.DoKeyword;
                case "new":
                    return SyntaxType.NewKeyword;
                case "typeof":
                    return SyntaxType.TypeofKeyword;
                case "readonly":
                    return SyntaxType.ReadOnlyKeyword;
                default:
                    return SyntaxType.IdentifierToken;
            }
        }

        public static string? GetText(SyntaxType kind)
        {
            switch (kind)
            {
                case SyntaxType.PlusToken:
                    return "+";
                case SyntaxType.PlusEqualsToken:
                    return "+=";
                case SyntaxType.MinusToken:
                    return "-";
                case SyntaxType.MinusEqualsToken:
                    return "-=";
                case SyntaxType.StarToken:
                    return "*";
                case SyntaxType.StarEqualsToken:
                    return "*=";
                case SyntaxType.SlashToken:
                    return "/";
                case SyntaxType.SlashEqualsToken:
                    return "/=";
                case SyntaxType.BangToken:
                    return "!";
                case SyntaxType.EqualsToken:
                    return "=";
                case SyntaxType.TildeToken:
                    return "~";
                case SyntaxType.LessToken:
                    return "<";
                case SyntaxType.LessOrEqualsToken:
                    return "<=";
                case SyntaxType.GreaterToken:
                    return ">";
                case SyntaxType.GreaterOrEqualsToken:
                    return ">=";
                case SyntaxType.AmpersandToken:
                    return "&";
                case SyntaxType.AmpersandAmpersandToken:
                    return "&&";
                case SyntaxType.AmpersandEqualsToken:
                    return "&=";
                case SyntaxType.PipeToken:
                    return "|";
                case SyntaxType.PipeEqualsToken:
                    return "|=";
                case SyntaxType.PipePipeToken:
                    return "||";
                case SyntaxType.HatToken:
                    return "^";
                case SyntaxType.HatEqualsToken:
                    return "^=";
                case SyntaxType.EqualsEqualsToken:
                    return "==";
                case SyntaxType.BangEqualsToken:
                    return "!=";
                case SyntaxType.LeftParenthesisToken:
                    return "(";
                case SyntaxType.RightParenthesisToken:
                    return ")";
                case SyntaxType.LeftBraceToken:
                    return "{";
                case SyntaxType.RightBraceToken:
                    return "}";
                case SyntaxType.ColonToken:
                    return ":";
                case SyntaxType.CommaToken:
                    return ",";
                case SyntaxType.DotToken:
                    return ".";
                case SyntaxType.BreakKeyword:
                    return "break";
                case SyntaxType.ContinueKeyword:
                    return "continue";
                case SyntaxType.ElseKeyword:
                    return "else";
                case SyntaxType.FalseKeyword:
                    return "false";
                case SyntaxType.ForKeyword:
                    return "for";
                case SyntaxType.FunctionKeyword:
                    return "function";
                case SyntaxType.TemplateKeyword:
                    return "model";
                case SyntaxType.ImportKeyword:
                    return "import";
                case SyntaxType.ClassKeyword:
                    return "class";
                case SyntaxType.AbstractKeyword:
                    return "abstract";
                case SyntaxType.ConstructorKeyword:
                    return "constructor";
                case SyntaxType.IfKeyword:
                    return "if";
                case SyntaxType.IsKeyword:
                    return "is";
                case SyntaxType.LetKeyword:
                    return "let";
                case SyntaxType.ReturnKeyword:
                    return "return";
                case SyntaxType.ToKeyword:
                    return "to";
                case SyntaxType.TrueKeyword:
                    return "true";
                case SyntaxType.VarKeyword:
                    return "var";
                case SyntaxType.WhileKeyword:
                    return "while";
                case SyntaxType.DoKeyword:
                    return "do";
                case SyntaxType.NewKeyword:
                    return "new";
                case SyntaxType.TypeofKeyword:
                    return "typeof";
                case SyntaxType.ReadOnlyKeyword:
                    return "readonly";
                default:
                    return null;
            }
        }

        public static int GetUnaryOperatorPrecedence(this SyntaxType type)
        {
            switch (type)
            {
                case SyntaxType.PlusToken:
                case SyntaxType.MinusToken:
                case SyntaxType.BangToken:
                case SyntaxType.TildeToken:
                    return 6;
                default:
                    return 0;
            }
        }
        public static int GetBinaryOperatorPrecedence(this SyntaxType type)
        {
            switch (type)
            {
                case SyntaxType.DotToken:
                case SyntaxType.IsKeyword:
                    return 6;
                case SyntaxType.StarToken:
                case SyntaxType.SlashToken:
                    return 5;
                case SyntaxType.PlusToken:
                case SyntaxType.MinusToken:
                    return 4;
                case SyntaxType.EqualsEqualsToken:
                case SyntaxType.BangEqualsToken:
                case SyntaxType.LessToken:
                case SyntaxType.LessOrEqualsToken:
                case SyntaxType.GreaterToken:
                case SyntaxType.GreaterOrEqualsToken:
                    return 3;
                case SyntaxType.AmpersandToken:
                case SyntaxType.AmpersandAmpersandToken:
                    return 2;
                case SyntaxType.PipeToken:
                case SyntaxType.PipePipeToken:
                case SyntaxType.HatToken:
                    return 1;
                default:
                    return 0;
            }
        }
        public static IEnumerable<SyntaxType> GetUnaryOperatorKinds()
        {
            var kinds = (SyntaxType[])Enum.GetValues(typeof(SyntaxType));
            foreach (var kind in kinds)
            {
                if (GetUnaryOperatorPrecedence(kind) > 0)
                    yield return kind;
            }
        }
        public static IEnumerable<SyntaxType> GetBinaryOperatorKinds()
        {
            var kinds = (SyntaxType[])Enum.GetValues(typeof(SyntaxType));

            foreach (var kind in kinds)
            {
                if (GetBinaryOperatorPrecedence(kind) > 0)
                    yield return kind;
            }
        }
        public static SyntaxType GetBinaryOperatorOfAssignmentOperator(SyntaxType kind)
        {
            switch (kind)
            {
                case SyntaxType.PlusEqualsToken:
                    return SyntaxType.PlusToken;
                case SyntaxType.MinusEqualsToken:
                    return SyntaxType.MinusToken;
                case SyntaxType.StarEqualsToken:
                    return SyntaxType.StarToken;
                case SyntaxType.SlashEqualsToken:
                    return SyntaxType.SlashToken;
                case SyntaxType.AmpersandEqualsToken:
                    return SyntaxType.AmpersandToken;
                case SyntaxType.PipeEqualsToken:
                    return SyntaxType.PipeToken;
                case SyntaxType.HatEqualsToken:
                    return SyntaxType.HatToken;
                default:
                    throw new Exception($"Unexpected syntax: '{kind}'");
            }
        }

        public static bool IsTrivia(this SyntaxType type)
        {
            switch (type)
            {
                case SyntaxType.SkippedTextTrivia:
                case SyntaxType.LineBreakTrivia:
                case SyntaxType.WhitespaceTrivia:
                case SyntaxType.SingleLineCommentTrivia:
                case SyntaxType.MultiLineCommentTrivia:
                    return true;
                default:
                    return false;
            }
        }
        public static bool IsKeyword(this SyntaxType type) => type.ToString().EndsWith("Keyword");
        public static bool IsComment(this SyntaxType type) => type == SyntaxType.SingleLineCommentTrivia || type == SyntaxType.MultiLineCommentTrivia;
        public static bool IsToken(this SyntaxType type) => !type.IsTrivia() && (type.IsKeyword() || type.ToString().EndsWith("Token"));
    }
}
