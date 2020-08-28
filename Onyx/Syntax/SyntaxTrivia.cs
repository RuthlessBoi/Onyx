using Onyx.Text;

namespace Onyx.Syntax
{
    public sealed class SyntaxTrivia
    {
        public SyntaxTree SyntaxTree { get; }
        public SyntaxType Type { get; }
        public int Position { get; }
        public TextSpan Span => new TextSpan(Position, Text?.Length ?? 0);
        public string Text { get; }

        internal SyntaxTrivia(SyntaxTree syntaxTree, SyntaxType type, int position, string text)
        {
            SyntaxTree = syntaxTree;
            Type = type;
            Position = position;
            Text = text;
        }
    }
}
